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
			foreach (Player player in players)
				player.Update (gameTime);
		}

		public override void Draw(SpriteBatch batch)
		{
			foreach (Player player in players)
				player.Draw (batch);
		}

		public override void Load()
		{
			gunner1 = new Gunner();
			gunner2 = new Gunner();
			powerGenerator1 = new PowerGenerator();
			powerGenerator2 = new PowerGenerator();

			gunner1.PadIndex = PlayerIndex.One;
			gunner1.Color = Color.Red;

			camera = new Camera (Moxy.ScreenWidth, Moxy.ScreenHeight);
			camera.Targets.AddRange (new Player[] {gunner1});//, gunner2, powerGenerator1, powerGenerator2});
		}

		private Gunner gunner1;
		private Gunner gunner2;
		private PowerGenerator powerGenerator1;
		private PowerGenerator powerGenerator2;
		private List<Player> players;
		private Camera camera;
	}
}
