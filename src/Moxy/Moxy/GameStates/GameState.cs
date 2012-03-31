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
		}

		public override void Update(GameTime gameTime)
		{
			camera.Update (Moxy.Graphics);
			map.Update (gameTime);
			
			foreach (Player player in players)
				player.Update (gameTime);
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
			gameTarget = new RenderTarget2D (Moxy.Graphics, Moxy.ScreenWidth, Moxy.ScreenHeight);
			lightTarget = new RenderTarget2D (Moxy.Graphics, Moxy.ScreenWidth, Moxy.ScreenHeight);

			lightingEffect = Moxy.ContentManager.Load<Effect> ("lighting");
			lightTexture = Moxy.ContentManager.Load<Texture2D> ("light");
			texture = new Texture2D (Moxy.Graphics, 1, 1);
			texture.SetData (new [] { new Color(0, 0, 0, 200) });

			camera = new DynamicCamera ();
			map = new TileMap ();
			map.AmbientLight = new Color(255, 255, 255, 50);
			map.CreateTiles ("Content/map.bin");

			LoadPlayers();

			lights.Add (gunner1.Light);
			lights.Add (powerGenerator1.Light);

			players.Add (gunner1);
			players.Add (powerGenerator1);

			camera.ViewTargets.Add (gunner1);
			camera.ViewTargets.Add (powerGenerator1);
		}

		private Gunner gunner1;
		private Gunner gunner2;
		private PowerGenerator powerGenerator1;
		private PowerGenerator powerGenerator2;
		private List<Player> players;
		private DynamicCamera camera;
		private TileMap map;
		private Texture2D lightTexture;
		private Texture2D texture;
		private List<Light> lights;
		private RenderTarget2D gameTarget;
		private RenderTarget2D lightTarget;
		private Effect lightingEffect;

		private void DrawGame (SpriteBatch batch)
		{
			Moxy.Graphics.SetRenderTarget (gameTarget);
			Moxy.Graphics.Clear (Color.CornflowerBlue);

			batch.Begin (SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None,
				RasterizerState.CullCounterClockwise, null, camera.GetTransformation (Moxy.Graphics));

			map.Draw (batch);

			foreach (Player player in players)
				player.Draw (batch);

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
				Color = Color.Red,
				Location = new Vector2 (200, 0),
				Speed = gunnerSpeed,
				Light = new Light (Color.Red, lightTexture)
			};

			powerGenerator1 = new PowerGenerator
			{
				PadIndex = PlayerIndex.Two,
				Color = Color.Red,
				Location = new Vector2 (400, 0),
				Speed = enchanterSpeed,
				Light = new Light (Color.Red, lightTexture)
			};
		}
	}
}
