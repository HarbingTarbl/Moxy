using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Moxy
{
	public class DialogRunner
	{
		public DialogRunner()
		{
			messages = new Queue<CharacterMessage> ();
			font = Moxy.ContentManager.Load<SpriteFont> ("Fonts//scorefont");
			chatBox = Moxy.ContentManager.Load<Texture2D> ("dialogBox");

			chatBoxLocation = new Vector2 (0, Moxy.ScreenHeight - chatBox.Height);

			chatboxColor = new Color (255, 255, 255, 240);
			textColor = Color.White;

			characters = new Dictionary<string, Texture2D>
			{
				{"Boss", Moxy.ContentManager.Load<Texture2D> ("BossDialogue")},
			};
		}

		public bool NeedsFocus
		{
			get { return characterDisplayed && !currentMessage.Timed; }
		}

		public void Update(GameTime gameTime)
		{
			if (characterDisplayed && this.currentMessage.Timed && !fading)
			{
				this.currentMessage.displayedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
				if (currentMessage.displayedTime >= currentMessage.Limit)
					StartFade ();
			}

			if (!characterDisplayed && messages.Count > 0)
			{
				currentMessage = messages.Dequeue ();
				characterDisplayed = true;

				InvalidateCharacterLocation ();
			}

			if (fading)
			{
				fadePassed += (float)gameTime.ElapsedGameTime.TotalSeconds;

				if (fadePassed >= fadeTotal)
				{
					characterDisplayed = false;
					fading = false;
				}
			}

			HandleInput ();
		}

		private void StartFade()
		{
			fadePassed = 0;
			fading = true;
		}

		public void Clear()
		{
			fading = false;
			characterDisplayed = false;
			currentMessage = null;
			messages.Clear ();
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			if (!characterDisplayed)
				return;

			var lines = currentMessage.Message.Split ('\n');

			Color color = textColor;
			Color fadeColor = chatboxColor;
			Color characterColor = Color.White;

			if (fading)
			{
				float lerpAmount = fadePassed / fadeTotal;
				color = new Color (textColor.R, textColor.G, textColor.B, (int)MathHelper.Lerp (255, 0, lerpAmount));
				fadeColor = new Color (255, 255, 255, (int)MathHelper.Lerp (200, 0, lerpAmount));
				characterColor = new Color (255, 255, 255, (int)MathHelper.Lerp (255, 0, lerpAmount));
			}

			spriteBatch.Begin (SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
			spriteBatch.Draw (chatBox, chatBoxLocation, fadeColor);
			spriteBatch.Draw (characters[currentMessage.Name], characterLocation, characterColor);

			int offset = 0;
			int subOffset = lines.Length - 1;
			if (subOffset < 0)
				subOffset = 0;

			foreach (var line in lines)
			{
				Vector2 measure = font.MeasureString (line);
				textLocation = new Vector2 ((chatBoxLocation.X + (chatBox.Width / 2)) - (measure.X / 2),
					chatBoxLocation.Y + (chatBox.Height / 2));

				spriteBatch.DrawString (font, line, textLocation + new Vector2 (0, (offset * font.LineSpacing) - (subOffset * font.LineSpacing)), color);
				offset++;
			}
			spriteBatch.End ();
		}

		public void EnqueueMessageBox(string name, string text, Action messageClosed = null)
		{
			CharacterMessage message = new CharacterMessage
			{
				Message = text,
				Action = messageClosed,
				Name = name
			};
			messages.Enqueue (message);
		}

		public void ShowTimed(string name, string text, float time)
		{
			CharacterMessage message = new CharacterMessage
			{
				Message = text,
				Timed = true,
				Limit = time,
				Name = name,
			};

			this.characterDisplayed = true;
			currentMessage = message;
			messages.Clear ();

			InvalidateCharacterLocation ();
		}

		public void EnqueueTimed(string name, string text, float time)
		{
			CharacterMessage message = new CharacterMessage
			{
				Message = text,
				Timed = true,
				Limit = time,
				Name = name
			};

			messages.Enqueue (message);
		}

		private Queue<CharacterMessage> messages;
		private Dictionary<string, Texture2D> characters;
		private SpriteFont font;
		private Texture2D chatBox;
		private bool characterDisplayed;
		private Vector2 characterLocation;
		private Vector2 textLocation;
		private Vector2 chatBoxLocation;
		private CharacterMessage currentMessage;
		private Color textColor;
		private Color chatboxColor;
		private bool fading;
		private float fadeTotal = 0.75f;
		private float fadePassed;

		private void HandleInput()
		{
			if (this.characterDisplayed && !currentMessage.Timed)
			{
				if ((Moxy.CurrentPadStates[PlayerIndex.One].IsButtonDown (Buttons.A) && Moxy.LastPadStates[PlayerIndex.One].IsButtonUp (Buttons.A)))
				{
					if (currentMessage.Action != null)
						currentMessage.Action ();

					this.characterDisplayed = false;
				}
			}
		}

		private void InvalidateCharacterLocation()
		{
			characterLocation = new Vector2 (Moxy.ScreenWidth - characters[currentMessage.Name].Width,
				Moxy.ScreenHeight - characters[currentMessage.Name].Height);
		}

		private class CharacterMessage
		{
			public string Message;
			public Action Action;
			public bool Timed;
			public string Name;
			public float Limit;
			public float displayedTime;
		}
	}
}
