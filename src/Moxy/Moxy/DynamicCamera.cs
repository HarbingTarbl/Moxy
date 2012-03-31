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

			//Location = Vector2.Lerp (Location, desiredLocation, 0.01f);
			//Scale = MathHelper.Lerp (Scale, desiredScale, 0.01f);
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
		private Size currentSize;
		private float minScale = 1;
		private float maxScale = 0.1f;

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

			Size screenSize = originalSize;//new Size(highX - lowX, highY - lowY);
			
			var centerPoint = new Vector2 (((highX + lowX) / 2), ((highY + lowY) / 2));
			var origin = new Vector2 (centerPoint.X - (screenSize.Width / 2), centerPoint.Y - (screenSize.Height / 2));

			Location = origin;
			Scale = MathHelper.Clamp (originalSize.Height / (screenSize.Height + 64), minScale, maxScale);
			//currentSize = new Size(screenSize.Width * Scale, screenSize.Height * Scale);
			
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
