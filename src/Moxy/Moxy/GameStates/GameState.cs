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
			batch.Begin (SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None,
				RasterizerState.CullCounterClockwise, null, camera.GetTransformation(Moxy.Graphics));

			map.Draw (batch);

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
				Location = new Vector2(200, 0),
				Speed = 0.5f
			};

			gunner2 = new Gunner
			{
				PadIndex = PlayerIndex.Two,
				Color = Color.Blue,
				Location = new Vector2(400, 0),
				Speed = 0.5f
			};

			powerGenerator1 = new PowerGenerator();
			powerGenerator2 = new PowerGenerator();

			players.Add (gunner1);
			players.Add (gunner2);

			Size screenSize = new Size (Moxy.ScreenWidth, Moxy.ScreenHeight);
			camera = new DynamicCamera ();
			camera.ViewTargets.AddRange (new Player[] {gunner1, gunner2});

			map = new TileMap ();
			map.CreateTiles ("Content/map.bin");
		}

		private Gunner gunner1;
		private Gunner gunner2;
		private PowerGenerator powerGenerator1;
		private PowerGenerator powerGenerator2;
		private List<Player> players;
		private DynamicCamera camera;
		private TileMap map;
	}
}
