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
		public List<Player> ViewTargets = new List<Player> ();

		public bool UseBounds;
		public Size MinimumSize;

		public Vector2 ScreenToWorld(Vector2 ScreenPos)
		{
			Matrix inverse = Matrix.Invert(GetTransformation(Moxy.Graphics));
			Vector2 mousePos = Vector2.Transform(ScreenPos, inverse);

			return mousePos;
		}

		public Vector2 WorldToScreen(Vector2 WorldPos)
		{
			return Vector2.Transform(WorldPos, GetTransformation(Moxy.Graphics));
		}


		public Matrix GetTransformation(GraphicsDevice graphicsdevice)
		{
			return Matrix.CreateTranslation (new Vector3 (-Position.X, -Position.Y, 0)) *
				Matrix.CreateRotationZ (Rotation) *
				Matrix.CreateScale (new Vector3 (Zoom, Zoom, 0)) *
				Matrix.CreateTranslation (new Vector3 (
				graphicsdevice.Viewport.Width * 0.5f,
				graphicsdevice.Viewport.Height * 0.5f, 0));
		}

		public void Update(GraphicsDevice graphicsdevice)
		{
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
						rect.Inflate ((int)(MinimumSize.Width - rect.Width) / 2, 0);
					
					if (rect.Height < MinimumSize.Height)
						rect.Inflate (0, (int)(MinimumSize.Height - rect.Height) / 2);
				}
				
				rect.Inflate (100, 100);

				desiredPosition = new Vector2 (rect.Center.X, rect.Center.Y);

				float widthdiff = ((float)graphicsdevice.Viewport.Width) / ((float)rect.Width);
				float heightdiff = ((float)graphicsdevice.Viewport.Height) / ((float)rect.Height);
				desiredZoom = Math.Min (widthdiff, heightdiff);
			}

			Position = Vector2.Lerp (Position, desiredPosition, 0.1f);
			Zoom = MathHelper.Lerp (Zoom, desiredZoom, 0.1f);
		}

		private float desiredZoom;
		private Vector2 desiredPosition;

	}
}
