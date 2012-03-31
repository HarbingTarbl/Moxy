using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moxy.Entities;
using Moxy.Map;

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
			particleManager = new ParticleManager();
		}

		public override void Update(GameTime gameTime)
		{
			camera.Update (Moxy.Graphics);
			map.Update (gameTime);
			
			foreach (Player player in players)
				player.Update (gameTime);

			foreach (var monster in monsters)
				monster.Update (gameTime);

			CalculateEnergyRate();
			GenerateEnergy (gameTime);
			GenerateParticles (gameTime);
			FindMonsterTargets (gameTime);
			particleManager.Update (gameTime);

			// Spawn Monsters
			foreach (var spawner in map.MonsterSpawners)
			{
				var monster = spawner.Spawn (gameTime);
				if (monster != null)
					monsters.Add (monster);
			}
		}

		public override void Draw(SpriteBatch batch)
		{
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
			camera = new DynamicCamera();
			camera.MinimumSize = new Size(600, 600);
			camera.UseBounds = true;

			map = new TileMap ();
			map.AmbientLight = new Color (255, 255, 255, 10);
			map.CreateTiles ("Content/map.bin");

			gameTarget = new RenderTarget2D (Moxy.Graphics, Moxy.ScreenWidth, Moxy.ScreenHeight);
			lightTarget = new RenderTarget2D (Moxy.Graphics, Moxy.ScreenWidth, Moxy.ScreenHeight);

			lightingEffect = Moxy.ContentManager.Load<Effect> ("lighting");
			lightTexture = Moxy.ContentManager.Load<Texture2D> ("light");
			radiusTexture = Moxy.ContentManager.Load<Texture2D> ("Radius");
			particleTexture = Moxy.ContentManager.Load<Texture2D> ("EnergyParticle");
			texture = new Texture2D (Moxy.Graphics, 1, 1);
			texture.SetData (new [] { new Color(0, 0, 0, map.AmbientLight.A) });

			radiusOrigin = new Vector2 (radiusTexture.Width / 2, radiusTexture.Height / 2);

			LoadPlayers();

			lights.Add (gunner1.Light);
			lights.Add (powerGenerator1.Light);

			players.Add (gunner1);
			players.Add (powerGenerator1);

			camera.ViewTargets.Add (gunner1);
			camera.ViewTargets.Add (powerGenerator1);

			uiOverlay = new UIOverlay(this);
			uiOverlay.ActivePlayers = players;
		}

		public override void OnFocus()
		{
			Moxy.StateManager.Push(uiOverlay);

		}

		private Gunner gunner1;
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
		private List<Monster> monsters;
		private RenderTarget2D gameTarget;
		private RenderTarget2D lightTarget;
		private Effect lightingEffect;
		private Vector2 radiusOrigin;
		private ParticleManager particleManager;
		private UIOverlay uiOverlay;

		// Energy generation
		private float maxParticleDelay = 0.6f;
		private float minParticleDelay = 0.24f;
		private float maxPowerGeneration = 7;
		private float minPowerGeneration = 0;
		private float minPowerRange = 100;
		private float maxPowerRange = 342;
		private float powerGenerateInterval = 1f;
		private float powerGeneratePassed = 0;
		
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
			batch.Draw (radiusTexture, gunner1.Location, null, Color.White, 0f, radiusOrigin, 1f, SpriteEffects.None, 1f);

			foreach (Player player in players)
				player.Draw (batch);

			foreach (Monster monster in monsters)
				monster.Draw (batch);

			particleManager.Draw (batch);

			batch.End ();
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
			float gunnerSpeed = 0.1f;
			float enchanterSpeed = 0.3f;

			gunner1 = new Gunner
			{
				PadIndex = PlayerIndex.One,
				Color = Color.White,
				Location = new Vector2 (200, 0),
				Speed = gunnerSpeed,
				Light = new Light (Color.White, lightTexture)
			};

			powerGenerator1 = new PowerGenerator
			{
				PadIndex = PlayerIndex.Two,
				Color = Color.White,
				Location = new Vector2 (400, 0),
				Speed = enchanterSpeed,
				Light = new Light (Color.White, lightTexture)
			};
		}

		private void CalculateEnergyRate()
		{
			float distance = Vector2.Distance (gunner1.Location, powerGenerator1.Location);
			float lerp = MathHelper.Clamp ((distance - minPowerRange) / maxPowerRange, 0, 1);

			gunner1.EnergyRate = MathHelper.Lerp (minPowerGeneration, maxPowerGeneration, lerp);
			powerGenerator1.ParticleDelay = MathHelper.SmoothStep (minParticleDelay, maxParticleDelay, lerp);
			powerGenerator1.PowerDisabled = distance > maxPowerRange;
		}

		private void GenerateParticles(GameTime gameTime)
		{
			powerGenerator1.ParticleTimePassed += (float)gameTime.ElapsedGameTime.TotalSeconds;

			if (powerGenerator1.ParticleTimePassed > powerGenerator1.ParticleDelay && !powerGenerator1.PowerDisabled)
			{
				var particle = new Particle (powerGenerator1.Location, particleTexture, 1f, 1f) { Target = gunner1 };
				particleManager.StartParticle (particle);
				powerGenerator1.ParticleTimePassed = 0;
			}
		}

		private void FindMonsterTargets(GameTime gameTime)
		{
			foreach (Monster monster in monsters)
				monster.Target = gunner1;
		}

		private void GenerateEnergy(GameTime gameTime)
		{
			powerGeneratePassed += (float)gameTime.TotalGameTime.TotalSeconds;

			if (powerGeneratePassed > powerGenerateInterval && !powerGenerator1.PowerDisabled)
			{
				gunner1.Energy += gunner1.EnergyRate;
				powerGeneratePassed = 0;
			}
		}
	}
}
