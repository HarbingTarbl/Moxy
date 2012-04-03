using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Moxy.Interface
{
	public class ControllerSelector
	{
		public ControllerSelector(PlayerIndex index, CharacterFrame[] frames, ControllerSelector[] selecters, int defaultFrame)
		{
			this.PlayerIndex = index;
			this.frames = frames;
			this.selecters = selecters;
			this.texture = Moxy.ContentManager.Load<Texture2D> ("Interface//controller");
			this.origin = new Vector2 (texture.Width / 2, texture.Height / 2);
			this.SelectedIndex = defaultFrame;

			CalculateLocation ();
		}

		public int SelectedIndex;
		public PlayerIndex PlayerIndex;
		public bool IsConnected;
		public bool IsReady;
		public SoundEffect AcceptSound;
		public SoundEffect DeclineSound;
		public SoundEffect MoveSound;

		public void Draw(SpriteBatch batch)
		{
			if (!IsReady && IsConnected)
				batch.Draw (texture, location, null, Color.White, 0f, origin, scale, SpriteEffects.None, 1f);
		}

		public void Update(GameTime gameTime)
		{
			GamePadState padState = Moxy.CurrentPadStates[PlayerIndex];
			GamePadState lastState = Moxy.LastPadStates[PlayerIndex];
			IsConnected = padState.IsConnected;

			if (padState.Buttons.A.WasButtonPressed (lastState.Buttons.A) && !frames[SelectedIndex].IsReady && !frames[SelectedIndex].IsLocked)
			{
				frames[SelectedIndex].IsReady = true;
				frames[SelectedIndex].Selecter = this;
				IsReady = true;
				AcceptSound.Play (0.8f, 0f, 0f);
			}

			if (padState.Buttons.B.WasButtonPressed (lastState.Buttons.B) && frames[SelectedIndex].IsReady)
			{
				frames[SelectedIndex].IsReady = false;
				IsReady = false;
				DeclineSound.Play (0.6f, 0f, 0f);
			}

			if (!IsReady)
			{
				int index = SelectedIndex;
				bool moved = false;

				if (padState.DPad.Right.WasButtonPressed (lastPadState.DPad.Right))
				{
					index++;
					moved = true;
					MoveSound.Play (1f, 0f, 0f);
				}
				else if (padState.DPad.Left.WasButtonPressed (lastPadState.DPad.Left))
				{
					index--;
					moved = true;
					MoveSound.Play (1f, 0f, 0f);
				}
				SelectedIndex = Helpers.Clamp (index, 0, 3);

				if (moved)
					CalculateLocation ();
			}

			lastPadState = padState;
		}

		private Vector2 location;
		private Texture2D texture;
		private CharacterFrame[] frames;
		private ControllerSelector[] selecters;
		private float scale = 1.2f;
		private Vector2 origin;
		private GamePadState lastPadState;

		private void CalculateLocation()
		{
			int count = 0;
			foreach (var selecter in selecters)
				if (selecter != null && selecter != this)
					count += selecter.SelectedIndex == SelectedIndex ? 1 : 0;

			var frame = frames[SelectedIndex];
			location = frame.Location + new Vector2 (frame.FrameTexture.Width / 2,
				frame.Location.Y + frame.FrameTexture.Height + 25 + ((int)PlayerIndex * (texture.Height * scale)));
		}
	}
}
