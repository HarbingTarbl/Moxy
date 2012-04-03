using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moxy.GameStates;

namespace Moxy.Interface
{
	public class CharacterFrame
	{
		public Vector2 Location;
		public Texture2D FrameTexture;
		public Texture2D CheckTexture;
		public Texture2D LockTexture;
		public Rectangle Framebounds;
		public Vector2 checkOrigin;
		public Vector2 lockOrigin;
		public bool IsReady;
		public bool IsLocked;
		public ControllerSelector Selecter;

		public PlayerIndex PlayerIndex
		{
			get { return Selecter != null ? Selecter.PlayerIndex : (PlayerIndex)5; }
		}

		public void Draw(SpriteBatch batch)
		{
			Color color = IsReady || IsLocked ? Color.Gray : Color.White;

			batch.Draw (FrameTexture, Location, color);

			if (Framebounds == Rectangle.Empty)
			{
				Framebounds = new Rectangle ((int)Location.X, (int)Location.Y, FrameTexture.Width, FrameTexture.Height);
				checkOrigin = new Vector2 (CheckTexture.Width / 2, CheckTexture.Height / 2);
				lockOrigin = new Vector2 (LockTexture.Width / 2, LockTexture.Height / 2);
			}

			if (IsReady)
				batch.Draw (CheckTexture, Framebounds.Center.ToVector2 (), null, Color.White, 0f, checkOrigin, 1f, SpriteEffects.None, 1f);
			if (IsLocked)
				batch.Draw (LockTexture, Framebounds.Center.ToVector2 (), null, Color.White, 0f, lockOrigin, 1f, SpriteEffects.None, 1f);
		}

		public void Lock()
		{
			IsLocked = true;
			if (Selecter != null)
				Selecter.IsReady = false;
		}

		public void Unlock()
		{
			IsLocked = false;

		}
	}
}
