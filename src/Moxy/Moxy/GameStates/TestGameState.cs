using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moxy.Entities;

namespace Moxy.GameStates
{
	public class TestGameState
		: BaseGameState
	{
		public TestGameState()
			: base ("Test", isOverlay: false, acceptsInput: true)
		{
			players = new List<Player> (4);
		}

		public override void Update(GameTime gameTime)
		{
			camera.Update();

			foreach (Player player in players)
				player.Update (gameTime);
		}

		public override void Draw(SpriteBatch batch)
		{
			batch.Begin (SpriteSortMode.Texture, BlendState.NonPremultiplied, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, camera.Transformation);

			foreach (Player player in players)
				player.Draw (batch);

			batch.End();
		}

		public override void Load()
		{
			gunner1 = new Gunner
			{
				PadIndex = PlayerIndex.One,
				Color = Color.Red,
				Location = new Vector2(200, 200),
				Speed = 0.5f
			};

			gunner2 = new Gunner
			{
				PadIndex = PlayerIndex.Two,
				Color = Color.Blue,
				Location = new Vector2(800, 800),
				Speed = 0.5f
			};

			powerGenerator1 = new PowerGenerator();
			powerGenerator2 = new PowerGenerator();

			players.Add (gunner1);
			players.Add (gunner2);

			Size screenSize = new Size (Moxy.ScreenWidth, Moxy.ScreenHeight);
			camera = new DynamicCamera (screenSize, screenSize, new Size(Moxy.ScreenWidth + 400, Moxy.ScreenHeight + 400));
			camera.Targets.AddRange (new Player[] {gunner1, gunner2});
		}

		private Gunner gunner1;
		private Gunner gunner2;
		private PowerGenerator powerGenerator1;
		private PowerGenerator powerGenerator2;
		private List<Player> players;
		private DynamicCamera camera;
	}
}
