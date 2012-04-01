using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Moxy.Entities
{
	public class Slime
		: Monster
	{
		public Slime()
		{
			Texture = Moxy.ContentManager.Load<Texture2D>("EnemySpriteSheet");
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
			Animations.SetAnimation("Idle_Ice");
			Health = 17;
			TurnSpeed = MathHelper.Pi/14f;
			LowSpeed = 100;
			HighSpeed = 150;
			Collision = new Rectangle(0, 0, 96, 96);
			Origin = new Vector2(42, 54);
			CollisionRadius = 48;
			Light = new Light(new Color(0f, 0.4f, 0f, 0.3f));
			Light.Scale = .4f;
			ScoreGiven = 56;

			EntityType = global::Moxy.EntityType.Slime;
			OnCollisionWithPlayer += Slime_OnCollisionWithPlayer;
		}

		public float SlimeDamage = 5f;

		private void Slime_OnCollisionWithPlayer(object sender, Events.GenericEventArgs<Player> e)
		{
			e.Data.Damage(SlimeDamage * (float)Moxy.GameTime.ElapsedGameTime.TotalSeconds);
		}
	}
}
