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
		public Rectangle[] Tiles;
		public Vector2 TileSize;
		public Vector2 MapSize;



		public void CreateTiles()
		{
			var xTiles = (int)(Texture.Width / TileSize.X);
			var yTiles = (int)(Texture.Height / TileSize.Y);

			for (var y = 0; y < yTiles; y++)
			{
				for (var x = 0; x < xTiles; x++)
				{
					Tiles[(int)(x + (y * TileSize.X))] = new Rectangle((int)(x * TileSize.X), (int)(y * TileSize.Y), (int)TileSize.X, (int)TileSize.Y);
				}
			}

		}
	}
}
