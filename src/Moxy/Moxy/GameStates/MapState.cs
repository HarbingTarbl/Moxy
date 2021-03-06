﻿using System;
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
			camera.Update(Moxy.Graphics);
			map.Update(gameTime);

			var mouseState = Mouse.GetState ();
			var state = Keyboard.GetState ();
			
			if (WasKeyPressed (state, Keys.F1))
				currentTool = EditorTool.PlaceTiles;

			HandleCameraControls (state, gameTime);

			if (currentTool == EditorTool.PlaceTiles)
			{
				int tileID = currentTileID;
				if (WasKeyPressed(state, Keys.Right))
					tileID++;// = (int)MathHelper.Min(tileID + 1, map.Boundings.Length - 1);
				else if (WasKeyPressed(state, Keys.Left))
					tileID--;// = Helpers.LowerClamp(tileID - 1, 0);

				if (tileID < 0)
					tileID = map.Boundings.Length + tileID;

				currentTileID = tileID%map.Boundings.Length;
					

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

			// String debugging
			Vector2 mouseVec = new Vector2(mouseState.X, mouseState.Y);
			WorldAtCursor = camera.ScreenToWorld (mouseVec);
			TileAtCursor = new Vector2 ((int)WorldAtCursor.X / 64, (int)WorldAtCursor.Y / 64);

			oldMouse = mouseState;
			old = state;
		}

		public override void Load()
		{
			font = Moxy.ContentManager.Load<SpriteFont> ("Fonts//font");
			highlightTexture = Moxy.ContentManager.Load<Texture2D> ("Interface//highlight");
			texture = new Texture2D (Moxy.Graphics, 1, 1);
			texture.SetData (new[] { new Color (100, 100, 100, 100) });
		}

		public override void Draw (SpriteBatch batch)
		{
			batch.Begin (SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None,
				RasterizerState.CullCounterClockwise, null, camera.GetTransformation(Moxy.Graphics));
		
			map.Draw (batch);

			batch.End();

			batch.Begin ();
			if (currentTool == EditorTool.PlaceTiles)
				batch.Draw (map.Texture, new Vector2 (oldMouse.X, oldMouse.Y), map.Boundings[currentTileID % map.Boundings.Length], Color.White);

			// Render tile helper at top
			batch.Draw (texture, new Rectangle(0, 0, 800, 70), Color.DarkGray);
			int startIndex = Helpers.LowerClamp (currentTileID - 5, 0);
			//int endIndex = (int)MathHelper.Min(currentTileID + 10, )

			for (int i = startIndex; i < startIndex+10; i++)
			{
				int modifier = i - (startIndex + 5);
				batch.Draw (map.Texture, new Vector2(368 + (modifier * 64), 2), map.Boundings[i % map.Boundings.Length], Color.White);

				if (i == currentTileID)
					batch.Draw (highlightTexture, new Vector2 (368 + (modifier * 64), 2), Color.White);
			}
#region "Remove Me"
				var pad = GamePad.GetState(0);
			if(pad.IsConnected)
			{
				var angle = -(float)Math.Atan2(pad.ThumbSticks.Right.Y, pad.ThumbSticks.Right.X);
				var center = (new Vector2(Moxy.ScreenWidth/2, Moxy.ScreenHeight/2));
				batch.Draw(StatusBar.Pixel, new Rectangle((int)center.X, (int)center.Y - 1, 30, 3), null, Color.Red, angle,
						   Vector2.Zero, SpriteEffects.None, 0f);
				batch.DrawString(font, "Angle: " + MathHelper.ToDegrees(angle).ToString(), new Vector2(10, 220), Color.Red);

			}
			#endregion
			batch.DrawString (font, "Tile At Cursor: " + TileAtCursor.ToString (), new Vector2 (10, 80), Color.Red);
			batch.DrawString (font, "Current TileID: " + currentTileID, new Vector2 (10, 100), Color.Red);
			batch.DrawString (font, "Current Layer: " + Enum.GetName (typeof(MapLayerType), currentLayer), new Vector2 (10, 120), Color.Red);
			batch.DrawString (font, "World at Cursor: " + WorldAtCursor.ToString(), new Vector2 (10, 140), Color.Red);
			batch.DrawString (font, "TIles Drawn: " + map.TilesDrawn, new Vector2(10, 180),Color.Red);
			batch.DrawString (font, "FPS: " + Math.Round(1/Moxy.GameTime.ElapsedGameTime.TotalSeconds, 3).ToString(), new Vector2(10, 200),Color.Red) ;

			
			batch.End();
		}

		public override void OnFocus()
		{
			MapBuilder builder = new Map1Builder ();
			map = builder.Build ();
			//map = new MapRoot (64, 64, 64, 64, Moxy.ContentManager.Load<Texture2D> ("tileset"));
			//InitializeBaseLayer (16);

			camera = new DynamicCamera();
			camera.UseBounds = true;
			camera.MinimumSize = new Size(800, 600);
			camera.Position = new Vector2(0, 0);
			map.ViewCamera = camera;
		}

		public MapRoot map;
		public DynamicCamera camera;
		private KeyboardState old;
		private MouseState oldMouse;
		private EditorTool currentTool;
		private MapLayerType currentLayer;
		private int currentTileID;
		private SpriteFont font;
		private Texture2D texture;
		private Texture2D highlightTexture;

		private Vector2 TileAtCursor;
		private Vector2 WorldAtCursor;

		private void HandleCameraControls(KeyboardState state, GameTime gameTime)
		{
			// Moving
			if (state.IsKeyDown(Keys.W))
				camera.MoveDiff(-new Vector2(0, 600) * (float)gameTime.ElapsedGameTime.TotalSeconds);
			else if (state.IsKeyDown(Keys.S))
				camera.MoveDiff(new Vector2(0, 600)*(float) gameTime.ElapsedGameTime.TotalSeconds);
			if (state.IsKeyDown(Keys.A))
				camera.MoveDiff(-new Vector2(600, 0) * (float)gameTime.ElapsedGameTime.TotalSeconds);
			else if (state.IsKeyDown(Keys.D))
				camera.MoveDiff(new Vector2(600, 0)*(float) gameTime.ElapsedGameTime.TotalSeconds);

			// Zooming
			if (state.IsKeyDown(Keys.Up))
				camera.ZoomDiff(0.0075f);
			else if (state.IsKeyDown(Keys.Down))
				camera.ZoomDiff(-0.0075f);
		}

		public void SetTileAtPoint (Vector2 Location, int Value)
		{
			if (Location.X > 0 && Location.X < Moxy.ScreenWidth && Location.Y > 0 && Location.Y < Moxy.ScreenHeight)
			{
				var worldVec = camera.ScreenToWorld (Location);
				var tileVec = new Vector2 ((int)worldVec.X / 64, (int)worldVec.Y / 64);

				if (tileVec.X < 0 || tileVec.Y < 0)
					return;

				map.Layers[(int)currentLayer].Tiles[(int)tileVec.X, (int)tileVec.Y] = (uint)Value;
			}
		}

		private void InitializeBaseLayer (int id)
		{
			for (int x=0; x < map.Dimensions.Width; x++)
					for (int y=0; y < map.Dimensions.Height; y++)
						map.Layers[0].Tiles[x, y] = (uint)id;
		}

		private void ExportMapData()
		{
			StringBuilder b = new StringBuilder();

			for (int i = 0; i < 2; i++)
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
