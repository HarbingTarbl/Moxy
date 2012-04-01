using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Moxy.EventHandlers
{
	public class GunnerFireEventArgs
		: EventArgs
	{
		public GunnerFireEventArgs(Vector2 fireVector)
		{
			this.FireVector = fireVector;
		}

		public bool Handled;
		public Vector2 FireVector;

	}
}
