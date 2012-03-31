using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Moxy.GameStates;
using System.Reflection;

namespace Moxy
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Moxy
		: Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		public static Moxy Instance;
		public static int ScreenWidth;
		public static int ScreenHeight;
		public static GameStateManager StateManager;
		public static ContentManager ContentManager;
		public static GraphicsDevice Graphics;
		public static GamePadState CurrentGamePad;
		public static GamePadState OldGamePad;

		public Moxy()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			Instance = this;
		}

		protected override void LoadContent()
		{
			IsMouseVisible = true;
			spriteBatch = new SpriteBatch(GraphicsDevice);

			Moxy.ContentManager = Content;
			Moxy.ScreenHeight = graphics.PreferredBackBufferHeight;
			Moxy.ScreenWidth = graphics.PreferredBackBufferWidth;
			Moxy.StateManager = new GameStateManager();
			Moxy.Graphics = GraphicsDevice;

			Moxy.StateManager.Load (Assembly.GetExecutingAssembly());
			Moxy.StateManager.Set ("MenuState");
		}

		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
				this.Exit();

			Moxy.StateManager.Update (gameTime);

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear (Color.CornflowerBlue);

			Moxy.StateManager.Draw (spriteBatch);

			base.Draw(gameTime);
		}
	}
}
