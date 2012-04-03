using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moxy.Entities;

namespace Moxy
{
	public class DynamicCamera
	{
		public Vector2 Position = Vector2.Zero;
		public float Zoom = 1;
		public float Rotation = 0;
		public List<Player> ViewTargets = new List<Player>();

		public bool UseBounds;
		public Size MinimumSize;
		public int InflateAmount = 100;
		public Rectangle PlayerFrustrum;
		public Rectangle ViewFrustrum;

		public Vector2 ScreenToWorld(Vector2 screenVector)
		{
			return new Vector2((screenVector.X/Zoom) + Position.X,
			                   (screenVector.Y/Zoom) + Position.Y);
		}

		public Vector2 WorldToScreen(Vector2 WorldPos)
		{
			return Vector2.Transform(WorldPos, GetTransformation(Moxy.Graphics));
		}


		public Matrix GetTransformation(GraphicsDevice graphicsdevice)
		{
			return Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0))*
			       Matrix.CreateScale(new Vector3(Zoom, Zoom, 0));
		}

		public void MoveDiff(Vector2 Dif)
		{
			desiredPosition += Dif;
		}

		public void ZoomDiff(float Dif)
		{
			desiredZoom += Dif;
		}

		public void Update(GraphicsDevice graphicsdevice)
		{
			var worldAtZero = ScreenToWorld(Vector2.Zero);
			var worldAtView = ScreenToWorld(new Vector2(Moxy.Graphics.Viewport.Width, Moxy.Graphics.Viewport.Height));
			ViewFrustrum = new Rectangle((int)Math.Floor(worldAtZero.X), (int)Math.Floor(worldAtZero.Y), (int)Math.Ceiling(worldAtView.X - worldAtZero.X),
			                             (int)Math.Ceiling(worldAtView.Y - worldAtZero.Y));
			
			if (ViewTargets.Count > 0)
			{
				Vector2 min = ViewTargets[0].Location;
				Vector2 max = ViewTargets[0].Location;

				for (int i = 1; i < ViewTargets.Count; i++)
				{
					if (ViewTargets[i].Location.X < min.X) min.X = ViewTargets[i].Location.X;
					else if (ViewTargets[i].Location.X > max.X) max.X = ViewTargets[i].Location.X;
					if (ViewTargets[i].Location.Y < min.Y) min.Y = ViewTargets[i].Location.Y;
					else if (ViewTargets[i].Location.Y > max.Y) max.Y = ViewTargets[i].Location.Y;
				}

				Rectangle rect = new Rectangle ((int)min.X, (int)min.Y,
					(int)(max.X - min.X), (int)(max.Y - min.Y));

				if (UseBounds)
				{
					if (rect.Width < MinimumSize.Width)
						rect.Inflate((int)(MinimumSize.Width - rect.Width) / 2, 0);

					if (rect.Height < MinimumSize.Height)
						rect.Inflate(0, (int)(MinimumSize.Height - rect.Height) / 2);
				}
				
				rect.Inflate (InflateAmount, InflateAmount);
				PlayerFrustrum = rect;

				desiredPosition = new Vector2(rect.X, rect.Y);

				float widthdiff = ((float)graphicsdevice.Viewport.Width) / ((float)rect.Width);
				float heightdiff = ((float)graphicsdevice.Viewport.Height) / ((float)rect.Height);
				desiredZoom = Math.Min (widthdiff, heightdiff);

			}

			Position = Vector2.Lerp(Position, desiredPosition, 0.1f);
			Zoom = MathHelper.Lerp(Zoom, desiredZoom, 0.1f);
		}

		private float desiredZoom = 1;
		private Vector2 desiredPosition = new Vector2(0, 0);

	}
}
