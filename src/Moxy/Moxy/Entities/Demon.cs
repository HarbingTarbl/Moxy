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
			Texture = Moxy.ContentManager.Load<Texture2D>("Characters//demonSpriteSheet");
			Animations = new AnimationManager(Texture, new Animation[]
			{
				new Animation("Idle", new Rectangle[]
				{
					new Rectangle(0, 0, 128, 128),
					new Rectangle(128, 0, 128, 128),
					new Rectangle(256, 0, 128, 128)
				}, new TimeSpan(0, 0, 0, 0, 100))

			});


			Animations.SetAnimation("Idle");
			Health = 80;
			TurnSpeed = MathHelper.Pi / 14f;
			LowSpeed = 60f;
			HighSpeed = 70f;
			Collision = new Rectangle(0, 0, 128, 128);
			Origin = new Vector2(64, 64);
			CollisionRadius = 64;
			ScoreGiven = 56;


			OnCollisionWithPlayer += (Demon_OnCollisionWithPlayer);
		}

		public override void Update(GameTime gameTime)
		{
			attackCounter += gameTime.ElapsedGameTime;
			base.Update(gameTime);
		}

		public float DemonDamage = 30f;

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
