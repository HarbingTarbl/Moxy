using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Moxy.Entities
{
	public class PowerGenerator
		: Player
	{
		public PowerGenerator()
		{
			Texture = Moxy.ContentManager.Load<Texture2D>("Team1SpriteSheet");
			Animations = new AnimationManager(Texture, 
				new Animation[] 
				{
					new Animation("Idle", new Rectangle[] 
					{
						new Rectangle(0, 0, 64, 64),
						new Rectangle(64, 0, 64, 64),
						new Rectangle(128, 0, 64, 64),
						new Rectangle(192, 0, 64, 64),
					}, new TimeSpan(0, 0, 0, 0, 200))

				});
			Animations.SetAnimation("Idle");
			Health = 100;
		}

		public override void Draw(SpriteBatch batch)
		{

			batch.Draw(Texture, new Rectangle((int)Location.X, (int)Location.Y, 64, 64), Animations.Bounding, Color, Rotation - MathHelper.PiOver2, new Vector2(32, 32), SpriteEffects.None, 0);
		}

		public override void Update(GameTime gameTime)
		{
			Health = (Health - (1 * (float)gameTime.ElapsedGameTime.TotalSeconds)) % 100;
			Animations.Update(gameTime);
			base.Update(gameTime);
		}


		public AnimationManager Animations;
	}
}
