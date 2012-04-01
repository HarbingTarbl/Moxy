using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Moxy.Entities
{
	public class EyeBall
		: Monster
	{
		public EyeBall()
		{
			Texture = Moxy.ContentManager.Load<Texture2D>("MonsterEyeSpriteSheet");
			Animations = new AnimationManager(Texture, new Animation[] 
			{
				new Animation("Idle", new Rectangle[] {
					new Rectangle(0, 0, 64, 64),
					new Rectangle(64, 0, 64, 64),
					new Rectangle(128, 0, 64, 64)
				})
			});

			Animations.SetAnimation("Idle");
			Health = 10;
			TurnSpeed = MathHelper.Pi / 7f;
			MovementSpeed = 150f;
			Collision = new Rectangle(0, 0, 96, 96);
			Origin = new Vector2(42, 54);
			CollisionRadius = 48;
			OnCollisionWithPlayer += EyeBall_OnCollisionWithPlayer;
		}

		public float EyeBallDamage = 10f;

		void EyeBall_OnCollisionWithPlayer(object sender, Events.GenericEventArgs<Player> e)
		{
			e.Data.Damage(EyeBallDamage * (float)Moxy.GameTime.ElapsedGameTime.TotalSeconds);
		}

	}
}
