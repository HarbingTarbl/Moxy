using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Moxy.Entities
{
	public class Demon
		: Monster
	{
		public Demon()
		{
			Texture = Moxy.ContentManager.Load<Texture2D>("demonSpriteSheet");
			Animations = new AnimationManager(Texture, new Animation[]
			{
				new Animation("Idle", new Rectangle[]
				{
					new Rectangle(0, 0, 98, 64),
					new Rectangle(98, 0, 98, 64),
					new Rectangle(196, 0, 98, 64)
				})

			});


			Animations.SetAnimation("Idle");
			Health = 80;
			TurnSpeed = MathHelper.Pi / 14f;
			MovementSpeed = 70f;
			Collision = new Rectangle(0, 0, 96, 96);
			Origin = new Vector2(42, 54);
			CollisionRadius = 48;


			OnCollisionWithPlayer += (Demon_OnCollisionWithPlayer);
		}

		public override void Update(GameTime gameTime)
		{
			attackCounter += gameTime.ElapsedGameTime;
			base.Update(gameTime);
		}

		public float DemonDamage = 50f;

		void Demon_OnCollisionWithPlayer(object sender, Events.GenericEventArgs<Player> e)
		{
			if (attackCounter > attackRate)
			{
				e.Data.Damage(DemonDamage);
				attackCounter = TimeSpan.Zero;
			}
		}


		private TimeSpan attackRate = new TimeSpan(0, 0, 1);
		private TimeSpan attackCounter;

	}
}
