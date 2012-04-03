﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Moxy.GameStates
{
	public class CharacterSelectState
		: BaseGameState
	{
		public CharacterSelectState()
			: base ("CharacterSelect", isOverlay:false, acceptsInput:true)
		{
		}

		public PlayerIndex Gunner1;
		public PlayerIndex Gunner2;
		public PlayerIndex PowerGenerator1;
		public PlayerIndex PowerGenerator2;
		public bool CharactersSelected;

		public override void Update (GameTime gameTime)
		{
			if (Moxy.CurrentPadStates[PlayerIndex.One].Buttons.Back == ButtonState.Pressed)
				Moxy.StateManager.Set ("MainMenu");

			bool allReady = true;
			int readyCount = 0;
			foreach (var selecter in selecters)
			{
				selecter.Update (gameTime);

				if (!selecter.IsReady && selecter.IsConnected)
					allReady = false;
				else if (selecter.IsReady)
					readyCount++;
			}

			if (allReady)
			{
				Gunner1 = frames[0].PlayerIndex;
				PowerGenerator1 = frames[1].PlayerIndex;
				Gunner2 = frames[2].PlayerIndex;
				PowerGenerator2 = frames[3].PlayerIndex;

				CharactersSelected = true;
				Moxy.StateManager.Set ("Game");
			}

			// Lock certain portraits
			int controllerCount = selecters.Where (s => s.IsConnected).Count ();
			for (int i = 0; i < 4; i++)
			{
				if (i < controllerCount)
					frames[i].Unlock ();
				else
					frames[i].Lock ();
			}
		}

		public override void Draw (SpriteBatch batch)
		{
			Moxy.Graphics.Clear (Color.Black);

			batch.Begin();
			batch.Draw (panelTexture, new Vector2 (0, frameTexture.Height), Color.White);

			foreach (CharacterFrame frame in frames)
				frame.Draw (batch);

			foreach (ControllerSelector controller in selecters)
				controller.Draw (batch);

			batch.End();
		}

		public override void Load()
		{
			gameState = (GameState)Moxy.StateManager["Game"];

			frameTexture = Moxy.ContentManager.Load<Texture2D> ("characterFrame");
			gunnerFrame = Moxy.ContentManager.Load<Texture2D> ("gunnerFrame");
			powerFrame = Moxy.ContentManager.Load<Texture2D> ("powerFrame");
			checkTexture = Moxy.ContentManager.Load<Texture2D> ("checkmark");
			panelTexture = Moxy.ContentManager.Load<Texture2D> ("cspanel");
			lockTexture = Moxy.ContentManager.Load<Texture2D> ("lock");

			acceptSound = Moxy.ContentManager.Load<SoundEffect> ("Sounds//accept");
			declineSound = Moxy.ContentManager.Load<SoundEffect> ("Sounds//decline");
			moveSound = Moxy.ContentManager.Load<SoundEffect> ("Sounds//move");

			frames = new CharacterFrame[4];
			selecters = new ControllerSelector[4];

			for (int i = 0; i < 4; i++)
				frames[i] = new CharacterFrame
				{
					FrameTexture = Moxy.ContentManager.Load<Texture2D> ("cf" + i),
					CheckTexture = checkTexture,
					LockTexture = lockTexture,
					Location = new Vector2 (i * 200, 0)
				};

			for (int i = 0; i < 4; i++)
				selecters[i] = new ControllerSelector ((PlayerIndex) i, frames, selecters, i)
				{
					AcceptSound = acceptSound,
					DeclineSound = declineSound,
					MoveSound = moveSound,
				};
		}

		public override void OnFocus()
		{
			foreach (var frame in frames)
			{
				frame.IsReady = false;
				if (frame.Selecter != null)
					frame.Selecter.IsReady = false;
			}

			Gunner1 = (PlayerIndex)5;
			Gunner2 = (PlayerIndex)5;
			PowerGenerator1 = (PlayerIndex)5;
			PowerGenerator2 = (PlayerIndex)5;
		}

		private Texture2D frameTexture;
		private Texture2D gunnerFrame;
		private Texture2D powerFrame;
		private Texture2D lockTexture;
		private Texture2D panelTexture;
		private Texture2D checkTexture;
		private CharacterFrame[] frames;
		private ControllerSelector[] selecters;
		private GameState gameState;
		private SoundEffect acceptSound;
		private SoundEffect declineSound;
		private SoundEffect moveSound;

		private class CharacterFrame
		{
			public Vector2 Location;
			public Texture2D CharacterTexture;
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
				get { return Selecter != null ? Selecter.PlayerIndex : (PlayerIndex) 5; }
			}

			public void Draw(SpriteBatch batch)
			{
				Color color = IsReady || IsLocked ? Color.Gray : Color.White;

				batch.Draw (FrameTexture, Location, color);

				if (Framebounds == Rectangle.Empty)
				{
					Framebounds = new Rectangle ((int) Location.X, (int) Location.Y, FrameTexture.Width, FrameTexture.Height);
					checkOrigin = new Vector2 (CheckTexture.Width / 2, CheckTexture.Height / 2);
					lockOrigin = new Vector2 (LockTexture.Width / 2, LockTexture.Height / 2);
				}

				if (IsReady)
					batch.Draw (CheckTexture, Framebounds.Center.ToVector2(), null, Color.White, 0f, checkOrigin, 1f, SpriteEffects.None, 1f);
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

		private class ControllerSelector
		{
			public ControllerSelector (PlayerIndex index, CharacterFrame[] frames, ControllerSelector[] selecters, int defaultFrame)
			{
				this.PlayerIndex = index;
				this.frames = frames;
				this.selecters = selecters;
				this.texture = Moxy.ContentManager.Load<Texture2D> ("controller");
				this.origin = new Vector2 (texture.Width / 2, texture.Height / 2);
				this.SelectedIndex = defaultFrame;

				CalculateLocation();
			}

			public int SelectedIndex;
			public PlayerIndex PlayerIndex;
			public bool IsConnected;
			public bool IsReady;
			public SoundEffect AcceptSound;
			public SoundEffect DeclineSound;
			public SoundEffect MoveSound;

			public void Draw (SpriteBatch batch)
			{
				if (!IsReady && IsConnected)
					batch.Draw (texture, location, null, Color.White, 0f, origin, scale, SpriteEffects.None, 1f);
			}

			public void Update (GameTime gameTime)
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
						CalculateLocation();
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
				location = frame.Location + new Vector2(frame.FrameTexture.Width / 2,
					frame.Location.Y + frame.FrameTexture.Height + 25 + ((int)PlayerIndex * (texture.Height * scale)));
			}
		}
	}
}
