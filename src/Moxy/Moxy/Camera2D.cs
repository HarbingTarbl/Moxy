using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Moxy
{
	public class Camera2D
	{
		public Camera2D(int width, int height)
		{
			this.width = width;
			this.height = height;
			this.screenHalf = new Vector2 (width / 2, height / 2);
			this.updateTransform = true;
		}

		public Vector2 Location = new Vector2 (0, 0);
		public float Scale = 1f;
		public bool UseBounds;
		public Rectangle Bounds;

		public void CenterOnPoint(Vector2 loc)
		{
			CenterOnPoint (loc.X, loc.Y);
		}

		public void CenterOnPoint(float x, float y)
		{
			float newx = x - (screenHalf.X / Scale);
			float newy = y - (screenHalf.Y / Scale);

			if (UseBounds)
			{
				if (newx < 0) newx = 0;
				if (newy < 0) newy = 0;
				if (newx + width > Bounds.Right) newx = Bounds.Right - width;
				if (newy + height > Bounds.Bottom) newy = Bounds.Bottom - height;
			}

			Location = new Vector2 (newx, newy);
			updateTransform = true;
		}

		public Vector2 ScreenToWorld (Vector2 screenVector)
		{
			return new Vector2 ((screenVector.X / Scale) + Location.X,
				(screenVector.Y / Scale) + Location.Y);
		}

		public Vector2 Origin
		{
			get { return Location - screenHalf; }
		}

		public Matrix Transformation
		{
			get
			{
				if (updateTransform)
					generateTransformation ();

				return transformation;
			}
		}

		private int width;
		private int height;
		private Vector2 screenHalf;
		private bool updateTransform = false;
		private Matrix transformation;

		private void generateTransformation()
		{
			transformation = Matrix.CreateTranslation (new Vector3 (-Location.X, -Location.Y, 0)) *
				Matrix.CreateScale (new Vector3 (Scale, Scale, 0));
		}
	}
}
