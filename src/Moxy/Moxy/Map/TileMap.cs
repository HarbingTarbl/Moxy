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

namespace Moxy.Map
{
	public partial class TileMap
	{
		public Texture2D Texture;
		public Vector2 PositionOffset;
		public Rectangle[] TilesSources;
		public int[,] Tiles;
		public Vector2 TileSize;
		public Vector2 MapSize;
		public Color AmbientLight;

		public TileMap()
		{
			AmbientLight = Color.White;

		}

		public void Update(GameTime gameTime)
		{
			UpdateEditor(gameTime);
		}


		public void SetTileAtPoint(Vector2 Location, int Value)
		{
			var xTile = Location.X / TileSize.X;
			var yTile = Location.Y / TileSize.Y;
			if(xTile < MapSize.X && yTile < MapSize.Y)
				Tiles[(int)xTile, (int)yTile] = Value;
		}

		public void Draw(SpriteBatch batch)
		{
			var drawLocation = PositionOffset;
			for (var y = 0; y < MapSize.Y; y++)
			{
				drawLocation.X = PositionOffset.X;
				for (var x = 0; x < MapSize.X; x++)
				{

					batch.Draw(Texture, drawLocation, TilesSources[Tiles[x, y]], AmbientLight);
					drawLocation.X += TileSize.X;
				}
				drawLocation.Y += TileSize.Y;

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
