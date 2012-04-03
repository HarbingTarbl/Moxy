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
		public Rectangle Collision;
		public Vector2 CollisionCenter;
		public float CollisionRadius;
		public Texture2D Texture;
		public PlayerIndex playerIndex;
		public EntityType EntityType;
		public float Rotation;

		public bool Alive = true;

		public abstract void Update(GameTime gameTime);
		public abstract void Draw(SpriteBatch batch);
		public virtual void Draw(SpriteBatch batch, Rectangle ViewFrustrum)
		{
			if(ViewFrustrum.Contains(Collision))
				Draw(batch);
		}


		public AnimationManager Animations;
	}
}
