using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Moxy.Map
{
	public struct BoundingRectangle
	{
		float xLeft, xRight;
		float yLeft, yRight;

		public bool Contains(Vector2 Vector)
		{
			return (xLeft < Vector.X && xRight > Vector.X) &&
					(yLeft < Vector.Y && yRight > Vector.Y);

		}
	}
}
