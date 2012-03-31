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
		public float Health;
		public float MaxHealth = 100;
		public Vector2 Location;
		public Texture2D Texture;
		public PlayerIndex playerIndex;
		public EntityType EntityType;
		public float Rotation;

		public abstract void Update(GameTime gameTime);
		public abstract void Draw(SpriteBatch batch);


		public AnimationManager Animations;
	}
}
