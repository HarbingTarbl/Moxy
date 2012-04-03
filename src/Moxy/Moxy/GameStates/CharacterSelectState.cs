using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moxy.Interface;

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
			batch.Draw (panelTexture, new Vector2 (0, frames[0].FrameTexture.Height), Color.White);

			foreach (CharacterFrame frame in frames)
				frame.Draw (batch);

			foreach (ControllerSelector controller in selecters)
				controller.Draw (batch);

			batch.End();
		}

		public override void Load()
		{
			checkTexture = Moxy.ContentManager.Load<Texture2D> ("Interface//checkmark");
			panelTexture = Moxy.ContentManager.Load<Texture2D> ("Interface//cspanel");
			lockTexture = Moxy.ContentManager.Load<Texture2D> ("Interface//lock");

			acceptSound = Moxy.ContentManager.Load<SoundEffect> ("Sounds//accept");
			declineSound = Moxy.ContentManager.Load<SoundEffect> ("Sounds//decline");
			moveSound = Moxy.ContentManager.Load<SoundEffect> ("Sounds//move");

			frames = new CharacterFrame[4];
			selecters = new ControllerSelector[4];

			for (int i = 0; i < 4; i++)
				frames[i] = new CharacterFrame
				{
					FrameTexture = Moxy.ContentManager.Load<Texture2D> ("Interface//cf" + i),
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

		private Texture2D lockTexture;
		private Texture2D panelTexture;
		private Texture2D checkTexture;
		private CharacterFrame[] frames;
		private ControllerSelector[] selecters;
		private SoundEffect acceptSound;
		private SoundEffect declineSound;
		private SoundEffect moveSound;
	}
}
