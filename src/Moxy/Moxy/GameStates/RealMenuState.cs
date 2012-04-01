using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Moxy.GameStates
{
	class RealMenuState
		: BaseGameState
	{
		public RealMenuState()
			: base("RealMenu", false, true)
		{


		}

		public override void Load()
		{
			Background = Moxy.ContentManager.Load<Texture2D>("ArcanaBG");
			Credits = Moxy.ContentManager.Load<Texture2D>("Credits");
			Play = Moxy.ContentManager.Load<Texture2D>("PlayMenu");
			Exit = Moxy.ContentManager.Load<Texture2D>("Exit");
			Title = Moxy.ContentManager.Load<Texture2D>("Arcana");
			Selector = Moxy.ContentManager.Load<Texture2D>("MasterRune");
			AcceptSound = Moxy.ContentManager.Load<SoundEffect>("Sounds/accept");


			BackgroundRect = new Rectangle(0, 0, 800, 600);
			TitleLocation = BackgroundRect.Center.ToVector2() - new Vector2(Title.Width / 2, Title.Height / 2);
			PlayLocation = BackgroundRect.Center.ToVector2() - new Vector2(Play.Width / 2, Play.Height / 2);
			ExitLocation = BackgroundRect.Center.ToVector2() - new Vector2(Exit.Width / 2, Exit.Height / 2);
			CreditsLocation = BackgroundRect.Center.ToVector2() - new Vector2(Credits.Width / 2, Credits.Height / 2);

			TitleOffset = new Vector2(0, -200);
			PlayOffset = new Vector2(0, -50);
			CreditsOffset = new Vector2(0, 70);
			ExitOffset = new Vector2(0, 190);

			Locations = new[]
			{
				PlayOffset + PlayLocation,
				CreditsOffset + CreditsLocation,
				ExitOffset + ExitLocation
			};

			SelectorOffset = new Vector2(-50, 0);
			
		}

		public override void Draw(SpriteBatch batch)
		{
			batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);

			
			batch.Draw(Background, BackgroundRect, Color.White);
			batch.Draw(Title, TitleLocation + TitleOffset, Color.White);
			batch.Draw(Play, PlayLocation + PlayOffset, Color.White);
			batch.Draw(Exit, ExitLocation + ExitOffset, Color.White);
			batch.Draw(Credits, CreditsLocation + CreditsOffset, Color.White);
			batch.Draw(Selector, Locations[SelectedItem] + SelectorOffset, Color.White);


			batch.End();
		}

		public override void Update(GameTime gameTime)
		{
			var newState = GamePad.GetState(PlayerIndex.One);
			if (newState.DPad.Up == ButtonState.Pressed &&
				oldState.DPad.Up == ButtonState.Released)
				SelectedItem--;

			if (newState.DPad.Down == ButtonState.Pressed &&
				oldState.DPad.Down == ButtonState.Released)
				SelectedItem++;

			if (SelectedItem < 0)
				SelectedItem = 0;
			else if (SelectedItem >= 3)
				SelectedItem = 2;

			if (newState.Buttons.A == ButtonState.Pressed &&
				oldState.Buttons.A == ButtonState.Released)
			{
				AcceptSound.Play();
				switch (SelectedItem)
				{
					case 0:
						Moxy.StateManager.Set ("CharacterSelect");
						break;
					case 1:
						Moxy.StateManager.Set("MainMenu");
						break;
					case 2:
						System.Environment.Exit(0);
						break;
				}
			}
			oldState = newState;
		}

		public Vector2[] Locations;
		
		public int SelectedItem;

		public Vector2 SelectorOffset;

		public Vector2 CreditsOffset;
		public Vector2 PlayOffset;
		public Vector2 ExitOffset;
		public Vector2 TitleOffset;

		public Vector2 CreditsLocation;
		public Vector2 PlayLocation;
		public Vector2 ExitLocation;
		public Rectangle BackgroundRect;
		public Vector2 TitleLocation;

		public Texture2D Selector;
		public Texture2D Title;
		public Texture2D Background;
		public Texture2D Credits;
		public Texture2D Play;
		public Texture2D Exit;

		public SoundEffect AcceptSound;

		public Song Music;


		public GamePadState oldState;
	}
}
