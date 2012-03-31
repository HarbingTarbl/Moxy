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
					new Rectangle(0, 0, 64, 64),
					new Rectangle(64, 0, 64, 64),
					new Rectangle(128, 0, 64, 64)
				}),
				new Animation("Idle_Fire", new Rectangle[]
				{
					new Rectangle(0, 64, 64, 64),
					new Rectangle(64, 64, 64, 64),
					new Rectangle(128, 64, 64, 64)
				}),
				new Animation("Idle_Rock", new Rectangle[]
				{
					new Rectangle(0, 128, 64, 64),
					new Rectangle(64, 128, 64, 64),
					new Rectangle(128, 128, 64, 64)
				}),

			});
			Animations.SetAnimation("Idle_Ice");
			Health = 20;
			TurnSpeed = MathHelper.Pi/14f;
			MovementSpeed = 30f;
		}
	}
}
