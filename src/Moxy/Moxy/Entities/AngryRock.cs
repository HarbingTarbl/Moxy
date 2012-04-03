using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Moxy.Entities
{
	public class AngryRock
		: Monster
	{
		public AngryRock()
		{
			Texture = Moxy.ContentManager.Load<Texture2D>("Characters//EnemySpriteSheet");
			Animations = new AnimationManager(Texture, new Animation[]
			{
				new Animation("Idle_Ice", new Rectangle[]
				{
					new Rectangle(0, 0, 96, 96),
					new Rectangle(96, 0, 96, 96),
					new Rectangle(192, 0, 96, 96)
				}),
				new Animation("Idle_Fire", new Rectangle[]
				{
					new Rectangle(0, 96, 96, 96),
					new Rectangle(96, 96, 96, 96),
					new Rectangle(192, 96, 96, 96)
				}),
				new Animation("Idle_Rock", new Rectangle[]
				{
					new Rectangle(0, 192, 96, 96),
					new Rectangle(96, 192, 96, 96),
					new Rectangle(192, 192, 96, 96)
				}),

			});
			Animations.SetAnimation("Idle_Rock");
			Health = 17;
			TurnSpeed = MathHelper.Pi / 14f;
			LowSpeed = 100;
			HighSpeed = 150;
			Collision = new Rectangle(0, 0, 96, 96);
			Origin = new Vector2(42, 54);
			CollisionRadius = 48;
			ScoreGiven = 56;

			EntityType = global::Moxy.EntityType.Slime;

			OnCollisionWithPlayer += new EventHandler<Events.GenericEventArgs<Player>>(AngryRock_OnCollisionWithPlayer);
		}

		public override void  Update(GameTime gameTime)
		{
			currentTime += gameTime.ElapsedGameTime;
 			 base.Update(gameTime);
		}

		void AngryRock_OnCollisionWithPlayer(object sender, Events.GenericEventArgs<Player> e)
		{
			if (currentTime >= RockAttackSpeed)
			{
				currentTime = TimeSpan.Zero;
				e.Data.Damage(RockRamage);
			}
		}

		public float RockRamage = 20;
		public TimeSpan RockAttackSpeed = new TimeSpan(0, 0, 0, 100);
		

		private TimeSpan currentTime;

	}
}
