using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Moxy.Entities;

namespace Moxy
{
	public class DynamicCamera
	{
		public DynamicCamera (Size originalSize, Size minSize, Size maxSize)
		{
			this.originalSize = originalSize;
			this.minSize = minSize;
			this.maxSize = maxSize;
			this.Targets = new List<Entity>();
		}

		public Vector2 Location = new Vector2 (0, 0);
		public float Scale = 1f;
		public bool UseBounds;
		public Rectangle Bounds;
		public List<Entity> Targets;

		public float desiredScale;
		public Vector2 desiredLocation;

		public void Update()
		{
			calculateProperties ();

			Location = Vector2.Lerp (Location, desiredLocation, 0.1f);
			Scale = MathHelper.Lerp (Scale, desiredScale, 0.1f);
		}

		public Vector2 ScreenToWorld(Vector2 screenVector)
		{
			return new Vector2 ((screenVector.X * Scale) + Location.X, (screenVector.Y + Location.Y) * Scale);
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

		private Size originalSize;
		private Size minSize;
		private Size maxSize;
		private Size currentSize;

		private Vector2 screenHalf;
		private bool updateTransform = false;
		private Matrix transformation;

		private void calculateProperties()
		{
			float lowX = float.MaxValue;
			float highX = float.MinValue;
			float lowY = float.MaxValue;
			float highY = float.MinValue;

			foreach (Entity target in Targets)
			{
				if (target.Location.X < lowX)
					lowX = target.Location.X;
				if (target.Location.X > highX)
					highX = target.Location.X;

				if (target.Location.Y < lowY)
					lowY = target.Location.Y;
				if (target.Location.Y > highY)
					highY = target.Location.Y;
			}

			desiredLocation = new Vector2((highX - lowX), (highY - lowY));
			desiredScale = (highX - lowX) / originalSize.Width;

			updateTransform = true;
		}

		private void generateTransformation()
		{
			//Vector2 origin = screenHalf / Scale;

			transformation = Matrix.Identity
							 * Matrix.CreateTranslation (-Location.X, -Location.Y, 0f)
				//* Matrix.CreateTranslation (origin.X, origin.Y, 0f)
							 * Matrix.CreateScale (Scale);
		}
	}
}
