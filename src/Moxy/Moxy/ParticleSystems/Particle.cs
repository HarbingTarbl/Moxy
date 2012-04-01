using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moxy.Entities;

namespace Moxy
{
	public class Particle
	{
		public Particle(Vector2 location, Texture2D texture, float scale, float time)
		{
			this.Original = CurrentLocation = location;
			this.Texture = texture;
			this.Scale = scale;
			this.origin = new Vector2((texture.Width * Scale) / 2, (texture.Height * Scale) / 2);
			this.Time = time;
			this.Color = Color.White;
		}

		public bool IsDead
		{
			get { return TimePassed > Time; }
		}

		public Vector2 Original;
		public Vector2 CurrentLocation;
		public float Size;
		public Texture2D Texture;
		public float Scale;
		public float Rotation;
		public Color Color;
		public float Time;
		public float TimePassed;
		public Player Target;
		public Vector2 EndLocation;
		public Light Light;

		public void Draw(SpriteBatch batch)
		{
			batch.Draw (Texture, CurrentLocation, null, Color, Rotation, origin, Scale, SpriteEffects.None, 1f);
		}

		private Vector2 origin;
	}
}
