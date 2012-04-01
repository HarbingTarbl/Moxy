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
					new Rectangle(0, 0, 98, 98),
					new Rectangle(98, 0, 98, 98),
					new Rectangle(196, 0, 98, 98)
				})
			});

			Animations.SetAnimation("Idle");
			Health = 10;
			TurnSpeed = MathHelper.Pi / 7f;
			LowSpeed = 150;
			HighSpeed = 300f;
			Collision = new Rectangle(0, 0, 98, 98);
			Origin = new Vector2(49, 50);
			CollisionRadius = 60;
			OnCollisionWithPlayer += EyeBall_OnCollisionWithPlayer;
			ScoreGiven = 56;
		}

		public float EyeBallDamage = 10f;

		void EyeBall_OnCollisionWithPlayer(object sender, Events.GenericEventArgs<Player> e)
		{
			e.Data.Damage(EyeBallDamage * (float)Moxy.GameTime.ElapsedGameTime.TotalSeconds);
		}

	}
}
