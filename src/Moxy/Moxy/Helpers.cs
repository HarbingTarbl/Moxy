using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Moxy
{
	public static class Helpers
	{
		public static int Clamp(int value, int min, int max)
		{
			if (value < min)
				return min;
			if (value > max)
				return max;

			return value;
		}

		public static int LowerClamp (int value, int min)
		{
			if (value < min)
				return min;

			return value;
		}

		public static bool WasButtonPressed(this ButtonState currentState, ButtonState lastState)
		{
			return currentState == ButtonState.Pressed && lastState == ButtonState.Released;
		}

		public static Vector2 ToVector2(this Point self)
		{
			return new Vector2 (self.X, self.Y);
		}
	}
}
