using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moxy.Entities;
using Moxy.ParticleSystems;
using Moxy.Events;
using Microsoft.Xna.Framework.Audio;
using Moxy.EventHandlers;

namespace Moxy.GameStates
{
	public class GameState
		: BaseGameState
	{
		public GameState()
			: base ("Game", isOverlay: false, acceptsInput: true)
		{
			players = new List<Player> (4);
			lights = new List<Light>();
			monsters = new List<Monster>();
			monsterPurgeQueue = new Queue<Monster>();
			items = new List<Item>();
			itemPurgeQueue = new Queue<Item>();
			redPacketEmitter = new EnergyPacketEmitter();
			FireballEmitter = new FireballEmitter();
			FireballEmitter.OnParticleMonsterCollision += OnBulletCollision;
		}

		public override void Update(GameTime gameTime)
		{
			if (!isLoaded)
				return;

			camera.Update (Moxy.Graphics);
			map.Update (gameTime);

			foreach (Player player in players)
				player.Update (gameTime);

			foreach(var item in items)
			{
				item.CheckCollision(gunner1);
				item.CheckCollision(powerGenerator1);
				item.Update(gameTime);
			}

			while (itemPurgeQueue.Count > 0)
				items.Remove(itemPurgeQueue.Dequeue());


			foreach (var monster in monsters)
			{
				FireballEmitter.CheckCollision(monster);
				monster.CheckCollide(gunner1);
				monster.CheckCollide(powerGenerator1);
				monster.Update(gameTime);
			}

			while (monsterPurgeQueue.Count > 0)
				monsters.Remove (monsterPurgeQueue.Dequeue ());

			redPacketEmitter.CalculateEnergyRate(gameTime);
			//GenerateEnergy (gameTime);
			redPacketEmitter.GenerateParticles(gameTime);
			FindMonsterTargets (gameTime);
			redPacketEmitter.Update (gameTime);
			FireballEmitter.Update(gameTime);

			// Spawn Monsters
			foreach (var spawner in map.MonsterSpawners)
			{
				var monster = spawner.Spawn (gameTime);
				if (monster != null)
				{
					monster.OnDeath += (monster_OnDeath);
					monsters.Add(monster);
				}
			}
		}

		public void monster_OnDeath(object sender, EventArgs e)
		{
			var monster = sender as Monster;
			monster.OnDeath -= monster_OnDeath;
			monsterPurgeQueue.Enqueue (monster);

			var item = monster.DropItem();
			if (item != null)
			{
				item.OnPickup += item_OnPickup;
				items.Add(item);
				Console.WriteLine("Dropping " + Enum.GetName(typeof(ItemID), item.ItemID));
			}
		}

		public void item_OnPickup(object sender, GenericEventArgs<Player> e)
		{
			var item = sender as Item;
			if (item != null)
			{
				item.OnPickup -= item_OnPickup;
				itemPurgeQueue.Enqueue (item);
			}
		}

		public override void Draw(SpriteBatch batch)
		{
			if (!isLoaded)
				return;
			
			DrawGame (batch);
			DrawLights (batch);

			Moxy.Graphics.SetRenderTarget (null);

			// Draw composite
			batch.Begin (SpriteSortMode.Immediate, BlendState.AlphaBlend);
			
			lightingEffect.Parameters["lightMask"].SetValue (lightTarget);
			lightingEffect.CurrentTechnique.Passes[0].Apply ();


			batch.Draw (gameTarget, Vector2.Zero, Color.White);
			batch.End();
		}

		public override void Load()
		{
			gameTarget = new RenderTarget2D (Moxy.Graphics, Moxy.ScreenWidth, Moxy.ScreenHeight);
			lightTarget = new RenderTarget2D (Moxy.Graphics, Moxy.ScreenWidth, Moxy.ScreenHeight);

			lightingEffect = Moxy.ContentManager.Load<Effect> ("lighting");
			lightTexture = Moxy.ContentManager.Load<Texture2D> ("light");
			radiusTexture = Moxy.ContentManager.Load<Texture2D> ("Radius");
			particleTexture = Moxy.ContentManager.Load<Texture2D> ("powerparticle");

			radiusOrigin = new Vector2 (radiusTexture.Width / 2, radiusTexture.Height / 2);

			uiOverlay = (UIOverlay)Moxy.StateManager["UIOverlay"];
			characterSelectState = (CharacterSelectState)Moxy.StateManager["CharacterSelect"];
		}

		public override void OnFocus()
		{
			if (characterSelectState.CharactersSelected)
			{
				Reset ();
				LoadMap();
				LoadPlayers();
				characterSelectState.CharactersSelected = false;
			}
			else
				LoadMap();
			if(gunner1 != null)
				gunner1.Location = map.GunnerSpawns[0];
			if (powerGenerator1 != null)
				powerGenerator1.Location = map.PowerGeneratorSpawns[0];
			if (gunner2 != null)
				gunner2.Location = map.GunnerSpawns[1];
			if (powerGenerator2 != null)
				powerGenerator2.Location = map.PowerGeneratorSpawns[1];

			isLoaded = true;
			Moxy.StateManager.Push(uiOverlay);
		}

		private float MaxPlayerDistance = 1000;
		private Gunner gunner1;
		private FireballEmitter FireballEmitter;
		private Gunner gunner2;
		private PowerGenerator powerGenerator1;
		private PowerGenerator powerGenerator2;
		private List<Player> players;
		public DynamicCamera camera;
		private MapRoot map;
		private Texture2D lightTexture;
		private Texture2D texture;
		private Texture2D radiusTexture;
		private Texture2D particleTexture; 
		private List<Light> lights;
		private Queue<Item> itemPurgeQueue;
		private List<Item> items;
		private Queue<Monster> monsterPurgeQueue;
		private List<Monster> monsters;
		private RenderTarget2D gameTarget;
		private RenderTarget2D lightTarget;
		private Effect lightingEffect;
		private Vector2 radiusOrigin;
		private EnergyPacketEmitter redPacketEmitter;
		private UIOverlay uiOverlay;
		private CharacterSelectState characterSelectState;
		private bool isLoaded;
		
		private void DrawGame (SpriteBatch batch)
		{
			Moxy.Graphics.SetRenderTarget (gameTarget);
			Moxy.Graphics.Clear (Color.CornflowerBlue);

			batch.Begin (SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None,
				RasterizerState.CullCounterClockwise, null, camera.GetTransformation (Moxy.Graphics));

			map.Draw(batch, new Rectangle(0, 0, 64, 64));
			
			foreach (Player player in players)
				player.Draw (batch);

			foreach (Monster monster in monsters)
				monster.Draw (batch);

			foreach (var item in items)
				item.Draw(batch);

			batch.End ();

			batch.Begin (SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default,
				RasterizerState.CullCounterClockwise, null, camera.GetTransformation (Moxy.Graphics));

			redPacketEmitter.Draw (batch);
			FireballEmitter.Draw (batch);
			batch.End();
		}

		private void DrawLights(SpriteBatch batch)
		{
			Moxy.Graphics.SetRenderTarget (lightTarget);
			Moxy.Graphics.Clear (Color.CornflowerBlue);

			batch.Begin();
			batch.Draw (texture, new Rectangle (0, 0, 800, 600), new Color (0, 0, 0, 255));
			batch.End();

			batch.Begin (SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None,
				RasterizerState.CullCounterClockwise, null, camera.GetTransformation (Moxy.Graphics));

			foreach (Particle part in FireballEmitter.particles)
				part.Light.Draw(batch);

			foreach (var monster in monsters)
				if (monster.Light != null)
					monster.Light.Draw(batch);

			foreach (var item in items)
				if (item.Light != null)
					item.Light.Draw(batch);

			foreach (Light light in lights)
				light.Draw (batch);

			foreach (Light light in map.PointLights)
				light.Draw (batch);

			foreach (var part in redPacketEmitter.particles)
				part.Light.Draw(batch);

			batch.End ();
		}

		private void Reset()
		{
			lights.Clear();
			players.Clear();
			monsters.Clear();

			uiOverlay.ActivePlayers.Clear();
			uiOverlay.StatusBars.Clear();
			uiOverlay.RedEnergyBar = null;
			uiOverlay.RedSkillBar = null;
			uiOverlay.RedRuneBar = null;
			uiOverlay.BlueEnergyBar = null;
			//TODO: Add blue removable here
		}

		private void LoadPlayers()
		{
			float gunnerSpeed = 0.3f;//0.1f;
			float enchanterSpeed = 0.3f;
			PlayerIndex invalidPlayerIndex = (PlayerIndex)5;

			if (characterSelectState.Gunner1 != invalidPlayerIndex
				|| characterSelectState.PowerGenerator1 != invalidPlayerIndex)
			{
				gunner1 = new Gunner
				{
					PadIndex = characterSelectState.Gunner1,
					Color = Color.White,
					Location = new Vector2 (700, 700),
					Speed = gunnerSpeed,
					Light = new Light (Color.White, lightTexture) { Scale = 1.5f },
					Team = Team.Red,
					FireballEmitter = FireballEmitter
				};

				powerGenerator1 = new PowerGenerator
				{
					PadIndex = characterSelectState.PowerGenerator1,
					Color = Color.White,
					Location = new Vector2 (780, 700),
					Speed = enchanterSpeed,
					Light = new Light (Color.White, lightTexture) { Scale = 1.5f },
					Team = Team.Red,
					Gunner = gunner1,
				};

				gunner1.OnMovement += (Player_OnMovement);
				powerGenerator1.OnMovement += (Player_OnMovement);

				gunner1.Generator = powerGenerator1;

				redPacketEmitter.Target = gunner1;
				redPacketEmitter.Source = powerGenerator1;

				lights.Add (gunner1.Light);
				lights.Add (powerGenerator1.Light);

				players.Add (gunner1);
				players.Add (powerGenerator1);

				camera.ViewTargets.Add (gunner1);
				camera.ViewTargets.Add (powerGenerator1);
			}

			if (characterSelectState.Gunner2 != invalidPlayerIndex
				|| characterSelectState.PowerGenerator2 != invalidPlayerIndex)
			{
				gunner2 = new Gunner
				{
					PadIndex = characterSelectState.Gunner2,
					Color = Color.White,
					Location = new Vector2 (200, 0),
					Speed = gunnerSpeed,
					Light = new Light (Color.White, lightTexture),
					Team = Team.Red,
					FireballEmitter = FireballEmitter
				};

				powerGenerator2 = new PowerGenerator
				{
					PadIndex = characterSelectState.PowerGenerator2,
					Color = Color.White,
					Location = new Vector2 (400, 0),
					Speed = enchanterSpeed,
					Light = new Light (Color.White, lightTexture),
					Team = Team.Red,
					Gunner = gunner2,
				};

				gunner2.Generator = powerGenerator2;

				redPacketEmitter.Target = gunner2;
				redPacketEmitter.Source = powerGenerator2;

				lights.Add (gunner2.Light);
				lights.Add (powerGenerator2.Light);

				players.Add (gunner2);
				players.Add (powerGenerator2);

				camera.ViewTargets.Add (gunner2);
				camera.ViewTargets.Add (powerGenerator2);
			}

			uiOverlay.ActivePlayers = players;
		}

		void Player_OnMovement(object sender, EventHandlers.PlayerMovementEventArgs e)
		{
			var distance = 0f;
			switch (e.Player.EntityType)
			{
				case EntityType.Generator:
					var gen = (PowerGenerator)e.Player;
					distance = Vector2.Distance(e.NewLocation, gen.Gunner.Location);
					break;
				case EntityType.Gunner:
					var gun = (Gunner)e.Player;
					distance = Vector2.Distance(e.NewLocation, gun.Generator.Location);
					break;
			}

			if (distance > MaxPlayerDistance)
			{
				e.Handled = true;
				e.NewLocation = e.CurrentLocation;
			}
		}

		private void LoadMap()
		{
			camera = new DynamicCamera ();
			camera.MinimumSize = new Size (600, 600);
			camera.UseBounds = true;

			map = new MapRoot(128, 128, 64, 64, Moxy.ContentManager.Load<Texture2D>("tileset"));
			map = Moxy.Maps[0].Build ();

			texture = new Texture2D (Moxy.Graphics, 1, 1);
			texture.SetData (new[] { new Color (0, 0, 0, map.AmbientColor.A) });
		}

		private void OnBulletCollision(object sender, GenericEventArgs<Monster> e)
		{

		}

		private void FindMonsterTargets(GameTime gameTime)
		{
			foreach (Monster monster in monsters)
				monster.Target = gunner1;
		}
	}
}
