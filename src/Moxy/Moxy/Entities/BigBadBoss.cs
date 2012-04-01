using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Moxy.Entities
{
	public class BigBadBoss
		: Monster
	{
		public BigBadBoss()
		{
			Texture = Moxy.ContentManager.Load<Texture2D>("BossSpriteSheet");
			Animations = new AnimationManager(Texture, new Animation[]
			{
				new Animation("Spawn", new Rectangle[]
				{
					new Rectangle(0, 0, 384, 384),
					new Rectangle(384, 0, 384, 384),
					new Rectangle(786, 0, 384, 384), 
					new Rectangle(1152, 0, 384, 384),
					new Rectangle(1536, 0, 384, 384)
				}),
				new Animation("Idle", new Rectangle[]
				{
					new Rectangle(0, 384, 384, 384),
					new Rectangle(384, 384, 384, 384),
					new Rectangle(786, 384, 384, 384)
				}),
				new Animation("Punch", new Rectangle[]
				{
					new Rectangle(0, 786, 384, 384),
					new Rectangle(384, 786, 384, 384)
				})

			});
			TurnSpeed = 0f;
			LowSpeed = 0f;
			HighSpeed = 0f;
			ScoreGiven = 56;
			Health = 200;
		}

		public override void Draw(SpriteBatch batch)
		{
			base.Draw(batch);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
		}

		public override Item DropItem()
		{
			return base.DropItem();
		}

		public void Ability()
		{
		}
	}
}
