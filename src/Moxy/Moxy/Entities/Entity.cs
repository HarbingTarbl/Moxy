using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Moxy.Entities
{
	public abstract class Entity
	{
		protected Entity(Texture2D Texture)
		{
			this.Texture = Texture;
		}

		public float Health;

		public Rectangle Location;

		public Texture2D Texture;

		public abstract void Update(GameTime gameTime);
		public abstract void Draw(SpriteBatch batch);
	}
}
