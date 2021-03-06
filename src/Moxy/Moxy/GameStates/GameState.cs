﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moxy.Entities;
using Moxy.Levels;
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

			if (boss != null)
				boss.Update(gameTime);

			if (boss == null)
			{
				foreach (var player in players)
					player.Update(gameTime);
			}

			if (!InbetweenRounds)
			{
				foreach (var item in items)
				{
					item.Update (gameTime);
					item.CheckCollision (gunner1);
					item.CheckCollision (powerGenerator1);
				}

				while (itemPurgeQueue.Count > 0)
					items.Remove (itemPurgeQueue.Dequeue());

				foreach (var monster in monsters)
				{
					monster.Update (gameTime);
					FireballEmitter.CheckCollision (monster);
					monster.CheckCollide (gunner1);
					monster.CheckCollide (powerGenerator1);
				}

				while (monsterPurgeQueue.Count > 0)
					monsters.Remove (monsterPurgeQueue.Dequeue());

				// Spawn Monsters
				spawnPassed += (float)gameTime.ElapsedGameTime.TotalSeconds;

				if (spawnPassed > Level.SpawnDelay && monsterCount < Level.MaxMonsters)
				{
					var bounds = camera.PlayerFrustrum;
					bounds.Inflate (200, 200);

					int x = 0;
					int y = 0;

					var verticalOrHorizontal = GetRandomDouble ();
					if (verticalOrHorizontal == 0)
					{
						var topBottom = GetRandomDouble ();
						x = Moxy.Random.Next (bounds.X, bounds.X + bounds.Width);
						y = bounds.Y + (bounds.Height * topBottom);
					}
					else
					{
						var leftRight = GetRandomDouble ();
						x = bounds.X + (bounds.Width * leftRight);
						y = Moxy.Random.Next (bounds.Y, bounds.Y + bounds.Height);
					}

					Monster monster = Level.SpawnMonsterRandom ();
					if (monster != null)
					{
						monster.Location = new Vector2(x, y);
						monsterCount++;
						monster.OnDeath += monster_OnDeath;
						monsters.Add(monster);

						var chanceToAttackGen = Moxy.Random.NextDouble ();
						monster.Target = (chanceToAttackGen <= 0.30f) ? (Player)powerGenerator1 : (Player)gunner1;

						Level.SpawnDelay = Moxy.Random.Next((int)Level.SpawnIntervalLow, (int)Level.SpawnIntervalHigh);
						spawnPassed = 0;
					}
				}
			}

			redPacketEmitter.CalculateEnergyRate(gameTime);
			//GenerateEnergy (gameTime);
			redPacketEmitter.GenerateParticles(gameTime);
			FindMonsterTargets (gameTime);
			redPacketEmitter.Update (gameTime);
			FireballEmitter.Update(gameTime);

			// Should we fade the lights?
			if (fadingLight)
			{
				fadePassed += (float)gameTime.ElapsedGameTime.TotalSeconds;
				float lerp = MathHelper.Clamp (fadePassed / fadeTotal, 0, 1f);

				var color = startFadeColor.ToVector4();
				var colorTo = Level.AmbientLight.ToVector4();

				float lerpValue = MathHelper.Lerp (startFadeColor.A, Level.AmbientLight.A, fadePassed / fadeTotal);
				map.AmbientColor = new Color (10, 10, 10, (int)lerpValue);
				texture.SetData (new [] { new Color(0, 0, 0, map.AmbientColor.A)});

				if (lerp >= 1)
					fadingLight = false;
			}
			
			// Check if the level timer has expired
			if (Level != null && !InbetweenRounds && DateTime.Now.Subtract (StartLevelTime).TotalSeconds > Level.WaveLength)
			{
				HealPlayers ();
				CheckPlayerLevels ();
				waveDoneSound.Play (0.8f, 0.1f, 0f);
				LoadNextLevel();
			}
		}

		public void monster_OnDeath(object sender, EventArgs e)
		{
			monsterCount--;

			var monster = sender as Monster;
			monster.OnDeath -= monster_OnDeath;
			monsterPurgeQueue.Enqueue (monster);

			//TODO: Make it get experience for the right team
			Team1Score += monster.ScoreGiven;

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

			waveDoneSound = Moxy.ContentManager.Load<SoundEffect> ("Sounds\\waveComplete");
			levelUpSound = Moxy.ContentManager.Load<SoundEffect> ("Sounds\\LevelUp");

			radiusOrigin = new Vector2 (radiusTexture.Width / 2, radiusTexture.Height / 2);

			uiOverlay = (UIOverlay)Moxy.StateManager["UIOverlay"];
			characterSelectState = (CharacterSelectState)Moxy.StateManager["CharacterSelect"];

			gamePauseTimer = new Timer (timer_StartNextRound, null, Timeout.Infinite, Timeout.Infinite);

			ExperienceTable = new int[]
			{
				0,
				1000,
				2000,
				4000
			};
		}

		public override void OnFocus()
		{
			if (characterSelectState.CharactersSelected)
			{
				Reset ();
				LoadMap();
				LoadPlayers();
				LoadNextLevel ();
				characterSelectState.CharactersSelected = false;
			}

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


		public BigBadBoss boss;
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
		private List<Item> items;
		private List<Monster> monsters;
		private Queue<Item> itemPurgeQueue;
		private Queue<Monster> monsterPurgeQueue;
		private RenderTarget2D gameTarget;
		private RenderTarget2D lightTarget;
		private Effect lightingEffect;
		private Vector2 radiusOrigin;
		private EnergyPacketEmitter redPacketEmitter;
		private UIOverlay uiOverlay;
		private CharacterSelectState characterSelectState;
		private MonsterSpawner lastWinner;
		private Random random;
		private int monsterCount;
		public BaseLevel Level;
		private float spawnPassed;
		private bool isLoaded;
		private Timer gamePauseTimer;
		public bool InbetweenRounds = true;
		public DateTime StartLevelTime;
		private int timeBetweenRounds = 1;
		private SoundEffect waveDoneSound;
		private SoundEffect levelUpSound;
		public int Team1Score;
		public int Team2Score;
		public int[] ExperienceTable;
		private int lastAIID = 6;

		private bool fadingLight;
		private Color startFadeColor;
		private float fadePassed;
		private float fadeTotal = 2f;
		
		private void DrawGame (SpriteBatch batch)
		{
			Moxy.Graphics.SetRenderTarget (gameTarget);
			Moxy.Graphics.Clear (Color.CornflowerBlue);

			batch.Begin (SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None,
				RasterizerState.CullCounterClockwise, null, camera.GetTransformation (Moxy.Graphics));


			map.Draw(batch);

			//if (boss == null)
			{
				foreach (Player player in players)
					player.Draw(batch, camera.ViewFrustrum);
			}

			foreach (Monster monster in monsters)
				monster.Draw (batch, camera.ViewFrustrum);

			foreach (var item in items)
				item.Draw(batch, camera.ViewFrustrum);

			if (boss != null)
				boss.Draw(batch);
			

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

			boss = null;
			Moxy.CurrentLevelIndex = -1;
			//TODO: Add blue removable here
		}

		private void LoadPlayers()
		{
			float gunnerSpeed = 0.2f;//0.1f;
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
					FireballEmitter = FireballEmitter,
					AIControlled = characterSelectState.Gunner1 == invalidPlayerIndex,
				};

				// Gunner 1 is controlled by AI
				if (gunner1.PadIndex == invalidPlayerIndex)
				{
					gunner1.PadIndex = GenerateAIIndex();
					gunner1.AIControlled = true;
				}

				powerGenerator1 = new PowerGenerator
				{
					PadIndex = characterSelectState.PowerGenerator1,
					Color = Color.White,
					Location = new Vector2 (780, 700),
					Speed = enchanterSpeed,
					Light = new Light (Color.White, lightTexture) { Scale = 1.5f },
					Team = Team.Red,
					Gunner = gunner1,
					AIControlled = characterSelectState.PowerGenerator1 == invalidPlayerIndex,
				};

				// Power gen 1 is controlled by AI
				if (powerGenerator1.PadIndex == invalidPlayerIndex)
				{
					powerGenerator1.PadIndex = GenerateAIIndex ();
					powerGenerator1.AIControlled = true;
				}

				gunner1.OnMovement += (Player_OnMovement);
				powerGenerator1.OnMovement += (Player_OnMovement);
				gunner1.OnDeath += (Player_OnDeath);
				

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


		private PlayerIndex GenerateAIIndex()
		{
			PlayerIndex index = (PlayerIndex)(++lastAIID);
			Moxy.CurrentPadStates.Add (index, default (GamePadState));
			Moxy.LastPadStates.Add (index, default(GamePadState));

			return index;
		}

		void Player_OnDeath(object sender, EventArgs e)
		{
			if (boss == null)
			{
				gamePauseTimer.Change (Timeout.Infinite, Timeout.Infinite);
				boss = new BigBadBoss (gunner1.Location);
				boss.Animations.SetAnimation ("Spawn");

				Moxy.Dialog.EnqueueMessageBox ("Boss", "Your deaths were\nin vain.", () => Moxy.StateManager.Set ("MainMenu"));
			}
			//Moxy.StateManager.Pop();
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
			camera.MinimumSize = new Size (800, 600);
			camera.UseBounds = true;

			map = new MapRoot(128, 128, 64, 64, Moxy.ContentManager.Load<Texture2D>("tileset"), camera);
			map = Moxy.Maps[0].Build ();
		

			texture = new Texture2D (Moxy.Graphics, 1, 1);
			texture.SetData (new[] { new Color (0, 0, 0, map.AmbientColor.A) });
		}

		private void OnBulletCollision(object sender, GenericEventArgs<Monster> e)
		{

		}

		private void FindMonsterTargets (GameTime gameTime)
		{
			// TODO: Do this!
		}

		private void LoadNextLevel()
		{
			Moxy.CurrentLevelIndex++;

			// We beat the game! // TODO: Add win screen
			if (Moxy.CurrentLevelIndex >= Moxy.Levels.Length)
			{
				Moxy.CurrentLevelIndex = -1;
				Moxy.StateManager.Set ("MainMenu");
				return;
			}

			monsters.Clear();
			monsterCount = 0;

			Level = Moxy.Levels[Moxy.CurrentLevelIndex];
			InbetweenRounds = true;
			gamePauseTimer.Change (new TimeSpan (0, 0, 0, timeBetweenRounds), new TimeSpan (0, 0, 0, timeBetweenRounds));

			if (Moxy.CurrentLevelIndex == 0)
			{
				Moxy.Dialog.EnqueueTimed ("Boss", "You think you can \n defeat me? Fools!", 3f);
				//() => Moxy.StateManager.Set ("MainMenu")
			}

			// Only fade after the first level
			if (Moxy.CurrentLevelIndex > 0)
			{
				startFadeColor = map.AmbientColor;
				fadePassed = 0;
				fadingLight = true;
			}
		}

		private int GetRandomDouble()
		{
			double random = Moxy.Random.NextDouble ();
			if (random < 0.5)
				return 0;
			else
				return 1;
		}

		private void timer_StartNextRound (object state)
		{
			gamePauseTimer.Change (Timeout.Infinite, Timeout.Infinite);
			StartLevelTime = DateTime.Now;
			InbetweenRounds = false;
		}

		private void HealPlayers()
		{
			if (gunner1 != null)
				gunner1.Health = gunner1.MaxHealth;
			if (powerGenerator1 != null)
				powerGenerator1.Health = powerGenerator1.MaxHealth;
			if (gunner2 != null)
				gunner2.Health = gunner2.MaxHealth;
			if (powerGenerator2 != null)
				powerGenerator2.Health = powerGenerator2.MaxHealth;
		}

		private bool strongerMessage = false;
		private void CheckPlayerLevels()
		{
			if (gunner1 != null && gunner1.Level < ExperienceTable.Length && Team1Score >= ExperienceTable[gunner1.Level])
			{
				if (!strongerMessage)
				{
					Moxy.Dialog.EnqueueTimed ("Boss", "So you've gotten stronger?\nSo what!", 3f);
					strongerMessage = true;
				}

				levelUpSound.Play (1.0f, 0f, 0f);
				gunner1.Level++;
				powerGenerator1.Level++;
			}

			if (gunner2 != null && gunner2.Level < ExperienceTable.Length && Team2Score >= ExperienceTable[gunner2.Level])
			{
				levelUpSound.Play (1.0f, 0f, 0f);
				gunner2.Level++;
				powerGenerator2.Level++;
			}
		}
	}
}
