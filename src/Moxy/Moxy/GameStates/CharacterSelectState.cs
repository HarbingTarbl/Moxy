using System;
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

			if (allReady && readyCount >= 2)
				Moxy.StateManager.Set ("Game");
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

			acceptSound = Moxy.ContentManager.Load<SoundEffect> ("Sounds//accept");
			declineSound = Moxy.ContentManager.Load<SoundEffect> ("Sounds//decline");

			frames = new CharacterFrame[4];
			selecters = new ControllerSelector[4];

			for (int i = 0; i < 4; i++)
			{
				frames[i] = new CharacterFrame
				{
					FrameTexture = Moxy.ContentManager.Load<Texture2D> ("cf" + i),
					//CharacterTexture = (i == 0 || i == 2) ? gunnerFrame : powerFrame,
					CheckTexture = checkTexture,
					Location = new Vector2 (i * 200, 0)
				};
			}

			for (int i = 0; i < 4; i++)
				selecters[i] = new ControllerSelector ((PlayerIndex)i, frames, selecters, 0)
					{
						AcceptSound = acceptSound,
						DeclineSound = declineSound
					};
		}

		private Texture2D frameTexture;
		private Texture2D gunnerFrame;
		private Texture2D powerFrame;
		private Texture2D panelTexture;
		private Texture2D checkTexture;
		private CharacterFrame[] frames;
		private ControllerSelector[] selecters;
		private GameState gameState;
		private SoundEffect acceptSound;
		private SoundEffect declineSound;

		private class CharacterFrame
		{
			public Vector2 Location;
			public Texture2D CharacterTexture;
			public Texture2D FrameTexture;
			public Texture2D CheckTexture;
			public Rectangle Framebounds;
			public Vector2 checkOrigin;
			public bool IsReady;
			public ControllerSelector Selecter;

			public void Draw(SpriteBatch batch)
			{
				Color color = IsReady ? Color.Gray : Color.White;

				batch.Draw (FrameTexture, Location, color);

				if (Framebounds == Rectangle.Empty)
				{
					Framebounds = new Rectangle ((int) Location.X, (int) Location.Y, FrameTexture.Width, FrameTexture.Height);
					checkOrigin = new Vector2 (CheckTexture.Width / 2, CheckTexture.Height / 2);
				}

				if (IsReady)
					batch.Draw (CheckTexture, Framebounds.Center.ToVector2(), null, Color.White, 0f, checkOrigin, 1f, SpriteEffects.None, 1f);
			}
		}

		private class ControllerSelector
		{
			public ControllerSelector (PlayerIndex index, CharacterFrame[] frames, ControllerSelector[] selecters, int defaultFrame)
			{
				this.playerIndex = index;
				this.frames = frames;
				this.selecters = selecters;
				this.texture = Moxy.ContentManager.Load<Texture2D> ("controller");
				this.origin = new Vector2 (texture.Width / 2, texture.Height / 2);
				this.SelectedIndex = defaultFrame;

				CalculateLocation();
			}

			public int SelectedIndex;
			public bool IsConnected;
			public bool IsReady;
			public SoundEffect AcceptSound;
			public SoundEffect DeclineSound;

			public void Draw (SpriteBatch batch)
			{
				if (!IsReady && IsConnected)
					batch.Draw (texture, location, null, Color.White, 0f, origin, scale, SpriteEffects.None, 1f);
			}

			public void Update (GameTime gameTime)
			{
				GamePadState padState = Moxy.CurrentPadStates[playerIndex];
				GamePadState lastState = Moxy.LastPadStates[playerIndex];
				IsConnected = padState.IsConnected;

				if (padState.Buttons.A.WasButtonPressed (lastState.Buttons.A) && !frames[SelectedIndex].IsReady)
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
					}
					else if (padState.DPad.Left.WasButtonPressed (lastPadState.DPad.Left))
					{
						index--;
						moved = true;
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
			private PlayerIndex playerIndex;
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
					frame.Location.Y + frame.FrameTexture.Height + 25 + ((int)playerIndex * (texture.Height * scale)));
			}
		}
	}
}
