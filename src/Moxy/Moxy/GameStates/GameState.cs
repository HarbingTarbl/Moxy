using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moxy.Entities;
using Moxy.Map;
using Moxy.ParticleSystems;
using Moxy.Events;

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
			monsterPurgeList = new List<Monster>();
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

			while (itemPurgeQueue.Count > 0)
				items.Remove (itemPurgeQueue.Dequeue ());

			foreach(var item in items)
			{
				item.Update(gameTime);
				item.CheckCollision(gunner1);
				item.CheckCollision(powerGenerator1);
			}

			foreach (var monster in monsterPurgeList)
			{
				monsters.Remove(monster);
			}
			monsterPurgeList.Clear();

			foreach (var monster in monsters)
			{
				monster.Update(gameTime);
				FireballEmitter.CheckCollision(monster);
				monster.CheckCollide(gunner1);
				monster.CheckCollide(powerGenerator1);
			}

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
					monster.OnDeath += new EventHandler(monster_OnDeath);
					monsters.Add(monster);
				}
			}
		}

		public void monster_OnDeath(object sender, EventArgs e)
		{
			var monster = sender as Monster;
			monster.OnDeath -= monster_OnDeath;
			monsterPurgeList.Add(monster);
			var item = monster.DropItem();
			if (item != null)
			{
				item.OnPickup += new EventHandler<GenericEventArgs<Player>>(item_OnPickup);
				items.Add(item);
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
				LoadMap();
				LoadPlayers();
				characterSelectState.CharactersSelected = false;
			}
			else
				LoadMap();

			isLoaded = true;
			Moxy.StateManager.Push(uiOverlay);
		}

		private Gunner gunner1;
		private FireballEmitter FireballEmitter;
		private Gunner gunner2;
		private PowerGenerator powerGenerator1;
		private PowerGenerator powerGenerator2;
		private List<Player> players;
		public DynamicCamera camera;
		private TileMap map;
		private Texture2D lightTexture;
		private Texture2D texture;
		private Texture2D radiusTexture;
		private Texture2D particleTexture; 
		private List<Light> lights;
		private Queue<Item> itemPurgeQueue;
		private List<Item> items;
		private List<Monster> monsterPurgeList;
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

			batch.Begin (SpriteSortMode.Texture, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None,
				RasterizerState.CullCounterClockwise, null, camera.GetTransformation (Moxy.Graphics));

			map.Draw (batch);
			batch.End();
			batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None,
				RasterizerState.CullCounterClockwise, null, camera.GetTransformation(Moxy.Graphics));
			
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

			foreach (Light light in lights)
				light.Draw (batch);

			batch.End ();
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
					Light = new Light (Color.White, lightTexture),
					Team = Team.Red,
					FireballEmitter = FireballEmitter
				};

				powerGenerator1 = new PowerGenerator
				{
					PadIndex = characterSelectState.PowerGenerator1,
					Color = Color.White,
					Location = new Vector2 (780, 700),
					Speed = enchanterSpeed,
					Light = new Light (Color.White, lightTexture),
					Team = Team.Red,
					Gunner = gunner1,
				};

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

		private void LoadMap()
		{
			camera = new DynamicCamera ();
			camera.MinimumSize = new Size (600, 600);
			camera.UseBounds = true;

			map = new TileMap ();
			map.CreateTiles ("Content/map" + Moxy.CurrentLevelIndex + ".bin");
			map.AmbientLight = new Color (255, 255, 255, 10);

			texture = new Texture2D (Moxy.Graphics, 1, 1);
			texture.SetData (new[] { new Color (0, 0, 0, map.AmbientLight.A) });
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
