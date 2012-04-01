using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moxy.Entities;

namespace Moxy.GameStates
{
	public class MapState
		: BaseGameState
	{
		public MapState()
			: base("MapState", isOverlay:false, acceptsInput:true)
		{
		}
	
		public override void Update(GameTime gameTime)
		{
			map.Update(gameTime);

			var mouseState = Mouse.GetState ();
			var state = Keyboard.GetState ();
			
			if (WasKeyPressed (state, Keys.F1))
				currentTool = EditorTool.PlaceTiles;

			HandleCameraControls (state, gameTime);

			if (currentTool == EditorTool.PlaceTiles)
			{
				int tileID = currentTileID;
				if (WasKeyPressed (state, Keys.PageUp))
					tileID++;
				else if (WasKeyPressed (state, Keys.PageDown))
					tileID = Helpers.LowerClamp(tileID - 1, 0);

				currentTileID = tileID;

				if (mouseState.LeftButton == ButtonState.Pressed)
					SetTileAtPoint (new Vector2 (mouseState.X, mouseState.Y), currentTileID);
				if (WasKeyPressed (state, Keys.NumPad1))
					currentLayer = MapLayerType.Base;
				if (WasKeyPressed (state, Keys.NumPad2))
					currentLayer = MapLayerType.Decal;
				if (WasKeyPressed (state, Keys.NumPad3))
					currentLayer = MapLayerType.Collision;

				if (WasKeyPressed (state, Keys.F5))
					ExportMapData ();
			}

			if (currentTool == EditorTool.PlaceMonsterSpawners)
			{
				throw new NotImplementedException();
			}

			oldMouse = mouseState;
			old = state;
		}

		public void SetTileAtPoint (Vector2 Location, int Value)
		{
			if (Location.X > 0 && Location.X < Moxy.ScreenWidth && Location.Y > 0 && Location.Y < Moxy.ScreenHeight)
			{
				var worldVec = camera.ScreenToWorld2 (Location);
				var tileVec = new Vector2 ((int)worldVec.X / 64, (int)worldVec.Y / 64);

				map.Layers[(int)currentLayer].Tiles[(int)tileVec.X, (int)tileVec.Y] = (uint)Value;
			}
		}

		public override void Draw (SpriteBatch batch)
		{
			batch.Begin (SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None,
				RasterizerState.CullCounterClockwise, null, camera.Transformation);

			map.Draw (batch, new Rectangle(0, 0, 128, 128));

			batch.End();

			batch.Begin ();
				if (currentTool == EditorTool.PlaceTiles)
					batch.Draw (map.Texture, new Vector2 (oldMouse.X, oldMouse.Y), map.Boundings[currentTileID], Color.White);
			batch.End();
		}

		public override void OnFocus()
		{
			MapBuilder builder = new Map1Builder();
			map = builder.Build ();
			camera = new Camera2D (800, 600);
		}

		public MapRoot map;
		public Camera2D camera;
		private KeyboardState old;
		private MouseState oldMouse;
		private EditorTool currentTool;
		private MapLayerType currentLayer;
		private int currentTileID;

		private void HandleCameraControls(KeyboardState state, GameTime gameTime)
		{
			// Moving
			if (state.IsKeyDown (Keys.W))
				camera.Location -= new Vector2 (0, 200) * (float)gameTime.ElapsedGameTime.TotalSeconds;
			else if (state.IsKeyDown (Keys.S))
				camera.Location += new Vector2 (0, 200) * (float)gameTime.ElapsedGameTime.TotalSeconds;
			if (state.IsKeyDown (Keys.A))
				camera.Location -= new Vector2 (200, 0) * (float)gameTime.ElapsedGameTime.TotalSeconds;
			else if (state.IsKeyDown (Keys.D))
				camera.Location += new Vector2 (200, 0) * (float)gameTime.ElapsedGameTime.TotalSeconds;

			// Zooming
			if (state.IsKeyDown (Keys.Up))
				camera.Scale -= (float)(0.1 * gameTime.ElapsedGameTime.TotalSeconds);
			else if (state.IsKeyDown (Keys.Down))
				camera.Scale += (float)(0.1 * gameTime.ElapsedGameTime.TotalSeconds);
		}

		private void ExportMapData()
		{
			StringBuilder b = new StringBuilder();

			for (int i = 0; i < 3; i++)
			{
				b.Append ("map.Layers[0].Tiles = new uint[,] {");

				for (int x=0; x < map.Dimensions.Width; x++)
				{
					b.Append ("{");
					for (int y=0; y < map.Dimensions.Height; y++)
					{
						b.Append (map.Layers[i].Tiles[x, y]);

						if (y+1 < map.Dimensions.Height)
							b.Append (",");
					}
					b.Append ("},");

				}
				
				b.Append ("};");

				string code = b.ToString();
				if (code != null)
					code = code;
			}
		}

		private bool WasKeyPressed(KeyboardState current, Keys key)
		{
			return current.IsKeyDown (key) && old.IsKeyUp (key);
		}

		private bool WasLeftMousePressed (MouseState current)
		{
			return current.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released;
		}

		private enum EditorTool
		{
			PlaceTiles,
			PlaceMonsterSpawners
		}
	}
}
