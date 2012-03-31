using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Moxy.Entities
{
	public class Player
		: Entity
	{
		public Player ()
		{
			texture = new Texture2D (Moxy.Graphics, 1, 1);
			texture.SetData (new [] { Color.White });
		}

		public PlayerIndex PadIndex;
		public float PlayerScore;
		public float Speed;
		public Color Color;
		public Team Team;
		public Light Light;

		public override void Update (GameTime gameTime)
		{
			HandleInput (gameTime);

			Light.Location = Location + new Vector2(32, 32);
		}

		public override void Draw (SpriteBatch batch)
		{
			batch.Draw (texture, new Rectangle((int)Location.X, (int)Location.Y, 64, 64), null, Color);
		}

		private void HandleInput (GameTime gameTime)
		{
			GamePadState currentPad = GamePad.GetState (PadIndex);

			Vector2 moveVector = currentPad.ThumbSticks.Left;
			if (moveVector.Length() > 0)
				moveVector.Normalize();

			moveVector.Y *= -1;

			base.Location += moveVector * Speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
			base.Rotation = (float)Math.Atan2 (moveVector.Y, moveVector.X);

			oldPad = currentPad;
		}

		private GamePadState oldPad;
		private Texture2D texture;
	}
}
