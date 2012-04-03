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
		public BigBadBoss(Vector2 Location)
		{
			Texture = Moxy.ContentManager.Load<Texture2D>("Characters//BossSpriteSheet");
			Animations = new AnimationManager(Texture, new Animation[]
			{
				new Animation("Spawn", new Rectangle[]
				{
					new Rectangle(0, 0, 384, 384),
					new Rectangle(384, 0, 384, 384),
					new Rectangle(768, 0, 384, 384), 
					new Rectangle(1152, 0, 384, 384),
					new Rectangle(1536, 0, 384, 384)
				}, new TimeSpan(0, 0, 0, 0, 150), false),
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
			Rotation = 0;
			Health = 200;
			this.Location = Location - new Vector2(184, 500);
			Animations.SetAnimation("Spawn");
		}

		public override void Draw(SpriteBatch batch)
		{
			batch.Draw(Texture, Location, Animations.Bounding, Color.White); 
		}

		public override void Update(GameTime gameTime)
		{
			Animations.Update(gameTime);
			base.Update(gameTime);
			Rotation = 0;
			
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
