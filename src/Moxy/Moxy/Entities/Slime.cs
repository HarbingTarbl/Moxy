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
			Health = 20;
			TurnSpeed = MathHelper.Pi/14f;
			MovementSpeed = 100f;
			Collision = new Rectangle(0, 0, 96, 96);
			Origin = new Vector2(42, 54);
			CollisionRadius = 48;
			
			EntityType = global::Moxy.EntityType.Slime;
		}
	}
}
