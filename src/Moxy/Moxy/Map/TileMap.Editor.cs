using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Moxy.Map
{
	public partial class TileMap
	{
		public bool EditActive = false;
		public void Activate()
		{
			if (EditActive == false)
			{
				EditActive = true;




			}

		}

		private void UpdateEditor(GameTime gameTime)
		{
			if (Keyboard.GetState().IsKeyDown(Keys.F1))
				Activate();
			else if (Keyboard.GetState().IsKeyDown(Keys.F2))
				Deactivate();

			if (EditActive)
			{
				var mouseState = Mouse.GetState();
				var state = Keyboard.GetState();
				var key = 0;

				if (state.IsKeyDown(Keys.NumPad0))
					key = 0;
				else if (state.IsKeyDown(Keys.NumPad1))
					key = 1;
				else if (state.IsKeyDown(Keys.NumPad2))
					key = 2;
				else if (state.IsKeyDown(Keys.NumPad3))
					key = 3;
				else if (state.IsKeyDown(Keys.NumPad4))
					key = 4;
				else if (state.IsKeyDown(Keys.NumPad5))
					key = 5;
				else if (state.IsKeyDown(Keys.NumPad6))
					key = 6;
				else if (state.IsKeyDown(Keys.NumPad7))
					key = 7;
				else if (state.IsKeyDown(Keys.NumPad8))
					key = 8;

				if (mouseState.LeftButton == ButtonState.Pressed)
					SetTileAtPoint(new Vector2(mouseState.X, mouseState.Y), key);

			}
		}

		public void Deactivate()
		{
			if (EditActive == true)
			{
				EditActive = false;





			}
		}


	}
}
