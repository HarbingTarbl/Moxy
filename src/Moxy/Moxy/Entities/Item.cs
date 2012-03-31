using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moxy.Events;

namespace Moxy.Entities
{
	public abstract class Item
	{
		public Item()
		{
			Texture = Moxy.ContentManager.Load<Texture2D>("Orb");
		}

		public Vector2 Location;
		public Texture2D Texture;
		public Rectangle Collision;
		public Vector2 CollisionCenter = Vector2.Zero;
		public float CollisionRadius = 10f;
		public Color Color = Color.White;
		public ItemID ItemID;
		public bool IsPowerup = false;

		public void Draw(SpriteBatch batch)
		{
			batch.Draw(Texture, Collision, null, Color, 0f, Vector2.Zero, SpriteEffects.None, 0);
		}

		public void Update(GameTime gameTime)
		{
			CollisionCenter = Location;
			Collision = new Rectangle((int)CollisionCenter.X, (int)CollisionCenter.Y, 1, 1);
			Collision.Inflate((int)CollisionRadius, (int)CollisionRadius);
		}

		public void CheckCollision(Player player)
		{
			if (Vector2.Distance(CollisionCenter, player.CollisionCenter) < (CollisionRadius + player.CollisionRadius))
			{
				if (OnPlayerCollision(player))
					OnPickup(this, new GenericEventArgs<Player>(player));
			}
		}

		public abstract bool OnPlayerCollision(Player p);

		public event EventHandler<GenericEventArgs<Player>> OnPickup;
	}

	public enum ItemID
	{
		None,
		GreenPowerup,
		RedPowerup,
		YellowPowerup,
		BluePowerup,
		HealthPowerup,
		ManaPowerup

	}

}
