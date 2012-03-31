using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moxy.Entities;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Moxy.GameStates
{
	public class UIOverlay
		: BaseGameState
	{
		public UIOverlay(GameState OwningState)
			: base("UIOverlay", isOverlay:true, acceptsInput:false)
		{
			StatusBar.UI = this;
			StatusBar.Pixel = new Texture2D(Moxy.Graphics, 1, 1, false, SurfaceFormat.Color);
			StatusBar.Pixel.SetData<Color>(new Color[] { Color.White });
			StatusBars = new List<StatusBar>();
			ActivePlayers = new List<Player>();
			this.OwningState = OwningState;
		}


		public override void Draw(SpriteBatch batch)
		{
			batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp,
				DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Matrix.Identity);
			foreach (var bar in StatusBars)
			{
				bar.Draw(batch);
			}
			batch.End();
		}

		public override void Update(GameTime gameTime)
		{
			foreach (var player in ActivePlayers)
			{
				if (!StatusBars.Any(bar => bar.Player == player))
				{
					var bar = new StatusBar();
					bar.Player = player;
					switch (player.playerIndex)
					{
						case PlayerIndex.One:
							bar.Location = new Vector2(30, 30);
							break;
						case PlayerIndex.Two:
							bar.Location = new Vector2(Moxy.ScreenWidth - 120, 30);
							break;
					}
					StatusBars.Add(bar);
				}
			}
		}

		public GameState OwningState;
		public List<StatusBar> StatusBars;
		public List<Player> ActivePlayers;
	}


	public class StatusBar
	{
		public static Rectangle BarSize = new Rectangle(0, 0, 90, 20);
		public static Texture2D Pixel;

		public Player Player;
		public Vector2 Location;
		public static UIOverlay UI;

		public void Draw(SpriteBatch batch)
		{
			Location = UI.OwningState.camera.WorldToScreen(Player.Location);
			Location.X -= 40;
			Location.Y -= 30;
			var outerRect = new Rectangle((int)Location.X, (int)Location.Y, BarSize.Width, BarSize.Height);
			var innerRect = new Rectangle((int)Location.X + 3, (int)Location.Y + 3, (int)((BarSize.Width - 6) * (Player.Health / 100f)), BarSize.Height - 6);
			batch.Draw(Pixel, outerRect, Color.DarkGray);
			batch.Draw(Pixel, innerRect, Color.PaleVioletRed);
		}
	}
}
