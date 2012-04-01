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
		public static int ScreenWidth = 800;
		public static int ScreenHeight = 600;
		public static GameStateManager StateManager;
		public static ContentManager ContentManager;
		public static GraphicsDevice Graphics;
		public static GameTime GameTime;
		public static Dictionary<PlayerIndex, GamePadState> CurrentPadStates;
		public static Dictionary<PlayerIndex, GamePadState> LastPadStates;

		public Moxy()
		{
			graphics = new GraphicsDeviceManager(this);
			graphics.PreferredBackBufferWidth = Moxy.ScreenWidth;
			graphics.PreferredBackBufferHeight = Moxy.ScreenHeight;
			graphics.ApplyChanges();

			Content.RootDirectory = "Content";

			CurrentPadStates = new Dictionary<PlayerIndex, GamePadState>
			{
				{PlayerIndex.One, GamePad.GetState (PlayerIndex.One)},
				{PlayerIndex.Two, GamePad.GetState (PlayerIndex.Two)},
				{PlayerIndex.Three, GamePad.GetState (PlayerIndex.Three)},
				{PlayerIndex.Four, GamePad.GetState (PlayerIndex.Four)}
			};
			LastPadStates = new Dictionary<PlayerIndex, GamePadState>();
		}

		protected override void LoadContent()
		{
			Instance = this;
			IsMouseVisible = true;
			spriteBatch = new SpriteBatch(GraphicsDevice);

			Moxy.Instance = this;
			Moxy.ContentManager = Content;
			Moxy.StateManager = new GameStateManager();
			Moxy.Graphics = GraphicsDevice;

			Moxy.StateManager.Load (Assembly.GetExecutingAssembly());
			Moxy.StateManager.Set("MainMenu");
		}

		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		protected override void Update(GameTime gameTime)
		{
			Moxy.GameTime = gameTime;

			foreach (PlayerIndex padIndex in CurrentPadStates.Keys.ToArray())
				CurrentPadStates[padIndex] = GamePad.GetState (padIndex);

			Moxy.StateManager.Update (gameTime);

			foreach (PlayerIndex padIndex in CurrentPadStates.Keys.ToArray())
				LastPadStates[padIndex] = CurrentPadStates[padIndex];

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			Moxy.StateManager.Draw (spriteBatch);

			base.Draw(gameTime);
		}
	}
}
