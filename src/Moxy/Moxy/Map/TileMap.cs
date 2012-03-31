using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;
using Moxy.GameStates;
using Moxy.Entities;

namespace Moxy.Map
{
	public partial class TileMap
	{
		public Texture2D Texture;
		public Vector2 PositionOffset;
		public Rectangle[] TilesSources;
		public List<MonsterSpawner> MonsterSpawners;
		public GameState State;
		public int[,] Tiles;
		public Vector2 TileSize;
		public Vector2 MapSize;
		public Color AmbientLight;
		public Camera Camera;

		public TileMap()
		{
			AmbientLight = Color.White;
			MonsterSpawners = new List<MonsterSpawner>();
			MonsterSpawners.Add(new MonsterSpawner(this) { Location = new Vector2(10, 10), MonsterType = "Slime" });

		}

		public void Update(GameTime gameTime)
		{
			UpdateEditor(gameTime);
		}


		public void SetTileAtPoint(Vector2 Location, int Value)
		{
			if (Location.X > 0 && Location.X < Moxy.ScreenWidth && Location.Y > 0 && Location.Y < Moxy.ScreenHeight)
			{
				var worldVec = Camera.ScreenToWorld(Location);
				Tiles[(int)worldVec.X / 64, (int)worldVec.Y / 64] = Value;
			}	
		}

		public void Draw(SpriteBatch batch)
		{

			var drawLocation = new Rectangle((int)PositionOffset.X, (int)PositionOffset.Y, 64, 64);
			for (var y = 0; y < MapSize.Y; y++)
			{
				drawLocation.X = (int)PositionOffset.X;
				for (var x = 0; x < MapSize.X; x++)
				{
					batch.Draw(Texture, drawLocation, TilesSources[Tiles[x, y]], Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
					drawLocation.X += drawLocation.Width;
				}
				drawLocation.Y += drawLocation.Height;

			}
		}

		public void CreateTiles(string Filename)
		{
			if (!File.Exists(Filename))
				CreateTiles();
			else
			{
				BinaryReader reader = new BinaryReader(File.Open(Filename, FileMode.Open));
				using (reader)
				{
					var xTex = reader.ReadInt32();
					var yTex = reader.ReadInt32();

					Color[] cData = new Color[xTex * yTex];
					for (var i = 0; i < cData.Length; i++)
					{
						cData[i].B = reader.ReadByte();
						cData[i].G = reader.ReadByte();
						cData[i].R = reader.ReadByte();
						cData[i].A = reader.ReadByte();
					}

					Texture = new Texture2D(Moxy.Graphics, xTex, yTex, false, SurfaceFormat.Color);
					Texture.SetData<Color>(cData);

					var xTile = reader.ReadSingle();
					var yTile = reader.ReadSingle();

					TileSize.X = xTile;
					TileSize.Y = yTile;

					var xMap = reader.ReadSingle();
					var yMap = reader.ReadSingle();

					MapSize.X = xMap;
					MapSize.Y = yMap;

					var xSize = reader.ReadInt32();
					var ySize = reader.ReadInt32();
					Tiles = new int[xSize, ySize];
					for (var x = 0; x < xSize; x++)
					{
						for (var y = 0; y < ySize; y++)
						{
							Tiles[x, y] = reader.ReadInt32();
						}
					}
					reader.Close();
				}

				var xTiles = (int)(Texture.Width / TileSize.X);
				var yTiles = (int)(Texture.Height / TileSize.Y);

				TilesSources = new Rectangle[xTiles * yTiles];
				for (var y = 0; y < yTiles; y++)
				{
					for (var x = 0; x < xTiles; x++)
					{
						TilesSources[(int)(x + (y * xTiles))] = new Rectangle((int)(x * TileSize.X), (int)(y * TileSize.Y), (int)TileSize.X, (int)TileSize.Y);
					}
				}
			}
		}

		public void CreateTiles()
		{
			var xTiles = (int)(Texture.Width / TileSize.X);
			var yTiles = (int)(Texture.Height / TileSize.Y);

			TilesSources = new Rectangle[xTiles * yTiles];
			for (var y = 0; y < yTiles; y++)
			{
				for (var x = 0; x < xTiles; x++)
				{
					TilesSources[(int)(x + (y * xTiles))] = new Rectangle((int)(x * TileSize.X), (int)(y * TileSize.Y), (int)TileSize.X, (int)TileSize.Y);
				}
			}

			Tiles = new int[(int)MapSize.X, (int)MapSize.Y];
		}
	}
}
