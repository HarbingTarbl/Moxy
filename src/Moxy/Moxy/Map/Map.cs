using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moxy.Entities;

namespace Moxy
{
	public class Map
	{
		public Map(int Width, int Height, int TileWidth, int TileHeight, Texture2D texture)
		{
			Layers = new MapLayer[3];
			Dimensions = new Size(Width, Height);
			TileDimensions = new Size(TileWidth, TileHeight);
			CollidableID = new HashSet<int> ();
			Texture = texture;

			CreateBoundings ();
			CreateLayers();
			CreateSpawns();
			CreateLights();
		}
		

		public Map(string File)
		{
			throw new NotImplementedException();
		}

		public readonly Texture2D Texture;
		public readonly MapLayer[] Layers;
		public readonly Size Dimensions;
		public readonly Size TileDimensions;
		public readonly HashSet<int> CollidableID; 

		public Color AmbientColor;
		public Vector2 LocationOffset;
		public Rectangle[] Boundings;

		public Vector2 TeamRedSpawn;
		public Vector2 TeamBlueSpawn;
		public Vector2[] PowerGeneratorSpawns;
		public Vector2[] GunnerSpawns;

		public List<MonsterSpawner> MonsterSpawners;
		public List<Light> PointLights;

		public Size TilesToPixel
		{
			get { return new Size(Dimensions.Width * TileDimensions.Width, Dimensions.Height * TileDimensions.Height); }
		}

		public void Draw(SpriteBatch batch, Rectangle bounds)
		{
			Layers[(int)MapLayerType.Base].Draw (batch, bounds);
			Layers[(int)MapLayerType.Decal].Draw (batch, bounds);
			//Layers[(int)MapLayerType.Collision].Draw(batch);
		}

		public void Update(GameTime gameTime)
		{


		}

		public bool CheckCollision(int X, int Y)
		{
			return CollidableID.Contains ((int)Layers[(int)MapLayerType.Collision].Tiles[X, Y]);
		}

		public bool CheckCollision(Point P)
		{
			return CheckCollision(P.X, P.Y);
		}

		public bool CheckCollision(Vector2 V)
		{
			return CheckCollision(new Point((int)V.X, (int)V.Y));
		}

		public bool CheckCollision(Rectangle R)
		{
			return (CheckCollision(R.Left, R.Top)
				|| CheckCollision(R.Left, R.Bottom)
				|| CheckCollision(R.Right, R.Top)
				|| CheckCollision(R.Right, R.Bottom)
				|| CheckCollision(R.Center));
		}

		private void CreateBoundings()
		{
			var xTiles = (int)(Texture.Width / TileDimensions.Width);
			var yTiles = (int)(Texture.Height / TileDimensions.Height);

			Boundings = new Rectangle[xTiles * yTiles];
			for (var y = 0; y < yTiles; y++)
			{
				for (var x = 0; x < xTiles; x++)
				{
					Boundings[(int)(x + (y * xTiles))] = new Rectangle((int)(x * TileDimensions.Width), (int)(y * TileDimensions.Height), (int)TileDimensions.Width, (int)TileDimensions.Height);
				}
			}
		}

		private void CreateLayers()
		{
			Layers[(int)MapLayerType.Base] = new MapLayer(this, MapLayerType.Base);
			Layers[(int)MapLayerType.Collision] = new MapLayer(this, MapLayerType.Collision);
			Layers[(int)MapLayerType.Decal] = new MapLayer(this, MapLayerType.Decal);
		}

		private void CreateSpawns()
		{
			GunnerSpawns = new Vector2[2];
			PowerGeneratorSpawns = new Vector2[2];
			MonsterSpawners = new List<MonsterSpawner>();
		}

		private void CreateLights()
		{
			PointLights = new List<Light>();
		}

	}


}
