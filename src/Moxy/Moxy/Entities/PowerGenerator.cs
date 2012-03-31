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
			Texture = Moxy.ContentManager.Load<Texture2D>("lofi_tiles");
			Animations = new AnimationManager(Texture, 
				new Animation[] 
				{
					new Animation("Idle", new Rectangle[] 
					{
						new Rectangle(0, 0, 8, 8),
						new Rectangle(8, 0, 8, 8),
						new Rectangle(16, 0, 8, 8),
						new Rectangle(24, 0, 8, 8),
					})

				});
			Animations.SetAnimation("Idle");
		}

		public override void Draw(SpriteBatch batch)
		{

			batch.Draw(Texture, new Rectangle((int)Location.X, (int)Location.Y, 64, 64), Animations.Bounding, Color);
		}

		public override void Update(GameTime gameTime)
		{
			Animations.Update(gameTime);
			base.Update(gameTime);
		}


		public AnimationManager Animations;
	}
}
