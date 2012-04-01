using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Moxy
{
	public class Light
	{
		public Light(Color color, Texture2D texture)
		{
			this.Color = color;
			this.Texture = texture;
			this.Origin = new Vector2(texture.Width / 2, texture.Height / 2);
			this.Scale = 0.8f;
		}

		public Light(Color color)
			: this(color, Moxy.ContentManager.Load<Texture2D>("light"))
		{
		}

		public void Draw(SpriteBatch batch)
		{
			batch.Draw (Texture, Location, null, Color, 0f, Origin, Scale, SpriteEffects.None, 1f);
		}

		public Vector2 Location;
		public Color Color;
		public Texture2D Texture;
		public Vector2 Origin;
		public float Scale;
	}
}
