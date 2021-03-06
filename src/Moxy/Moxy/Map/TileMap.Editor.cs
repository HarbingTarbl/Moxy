﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Moxy.Entities;

namespace Moxy
{
	public partial class TileMap
	{
		public bool EditActive = false;

		public void Activate()
		{
			if (EditActive == false)
			{
				EditActive = true;
				defaultScale = Camera.Scale;
			}
		}

		public void SaveFile (string FileName = "Content/map4.bin")
		{
			BinaryWriter writer = new BinaryWriter(File.Create(FileName));
			using(writer)
			{
				writer.Write(Texture.Width);
				writer.Write(Texture.Height);
				
				Color[] cData = new Color[Texture.Width * Texture.Height];
				Texture.GetData<Color>(cData);
				for (var i = 0; i < cData.Length; i++)
				{
					writer.Write(cData[i].B);
					writer.Write(cData[i].G);
					writer.Write(cData[i].R);
					writer.Write(cData[i].A);
				}

				writer.Write(TileSize.X);
				writer.Write(TileSize.Y);

				writer.Write(MapSize.X);
				writer.Write(MapSize.Y);

				writer.Write(Tiles.GetLength(0));
				writer.Write(Tiles.GetLength(1));
				for (var x = 0; x < Tiles.GetLength(0); x++)
				{
					for (var y = 0; y < Tiles.GetLength(1); y++)
					{
						writer.Write(Tiles[x, y]);
					}
				}
				writer.Close();
			}
		}

		private void UpdateEditor(GameTime gameTime)
		{
			if (Keyboard.GetState().IsKeyDown(Keys.F1))
				Activate();
			else if (Keyboard.GetState().IsKeyDown(Keys.F2))
				Deactivate();

			if (EditActive)
			{
				var mouseState = Mouse.GetState();
				var state = Keyboard.GetState();
				var key = 0;

				if (state.IsKeyDown(Keys.NumPad0))
					key = 0;
				else if (state.IsKeyDown(Keys.NumPad1))
					key = 1;
				else if (state.IsKeyDown(Keys.NumPad2))
					key = 2;
				else if (state.IsKeyDown(Keys.NumPad3))
					key = 3;
				else if (state.IsKeyDown(Keys.NumPad4))
					key = 4;
				else if (state.IsKeyDown(Keys.NumPad5))
					key = 5;
				else if (state.IsKeyDown(Keys.NumPad6))
					key = 6;
				else if (state.IsKeyDown(Keys.NumPad7))
					key = 7;
				else if (state.IsKeyDown(Keys.NumPad8))
					key = 8;

				if (state.IsKeyDown(Keys.A))
					Camera.Location -= new Vector2(200, 0) * (float)gameTime.ElapsedGameTime.TotalSeconds;
				else if (state.IsKeyDown(Keys.D))
					Camera.Location += new Vector2(200, 0) * (float)gameTime.ElapsedGameTime.TotalSeconds;

				if (state.IsKeyDown(Keys.W))
					Camera.Location -= new Vector2(0, 200) * (float)gameTime.ElapsedGameTime.TotalSeconds;
				else if (state.IsKeyDown(Keys.S))
					Camera.Location += new Vector2(0, 200) * (float)gameTime.ElapsedGameTime.TotalSeconds;

				if (state.IsKeyDown(Keys.Up))
					Camera.Scale -= (float)(0.1 * gameTime.ElapsedGameTime.TotalSeconds);
				else if (state.IsKeyDown(Keys.Down))
					Camera.Scale += (float)(0.1 * gameTime.ElapsedGameTime.TotalSeconds);

				if (state.IsKeyDown(Keys.Back) && old.IsKeyUp(Keys.Back) && MonsterSpawners.Count > 0
					
					
					)
					MonsterSpawners.RemoveAt(MonsterSpawners.Count - 1);


				if (mouseState.LeftButton == ButtonState.Pressed)
					SetTileAtPoint(new Vector2(mouseState.X, mouseState.Y), key);
				else if (mouseState.RightButton == ButtonState.Pressed && oldMouse.RightButton == ButtonState.Released)
					MonsterSpawners.Add(new MonsterSpawner()
					{
						Location = Camera.ScreenToWorld(new Vector2(mouseState.X, mouseState.Y)),
						MonsterType = "Slime"
					});
				oldMouse = mouseState;
				old = state;

			}
		}

		public void DrawEditor(SpriteBatch batch)
		{
			foreach (var spawner in MonsterSpawners)
			{
				spawner.Draw(batch);

			}
		}

		public void Deactivate()
		{
			if (EditActive == true)
			{
				EditActive = false;
				SaveFile();
				Camera.Scale = defaultScale;
			}
		}

		private float defaultScale;
		private KeyboardState old;
		private MouseState oldMouse;
	}
}
