using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Moxy;

namespace Moxy
{
	public class MapLayer
	{
		public MapLayer(MapRoot Parent, MapLayerType Type)
		{
			LayerType = Type;
			this.Parent = Parent;
			Tiles = new uint[(int)Parent.Dimensions.Width, (int)Parent.Dimensions.Height];

		}

		public readonly MapRoot Parent;
		public readonly MapLayerType LayerType;
		public uint[,] Tiles;

		public void Draw(SpriteBatch batch, Rectangle bounds)
		{
			var drawLocation = new Rectangle(0, 0, (int)Parent.TileDimensions.Width, (int)Parent.TileDimensions.Height);
			for (var x = 0; x < Parent.Dimensions.Width; x++)
			{
				drawLocation.Y = bounds.Y * (int)Parent.TileDimensions.Height ;
				for (var y = 0; y < Parent.Dimensions.Width; y++)
				{
					batch.Draw(Parent.Texture, drawLocation, Parent.Boundings[Tiles[x, y]], Color.White);
					drawLocation.Y += (int)Parent.TileDimensions.Height;
				}
				drawLocation.X += (int)Parent.TileDimensions.Width;

			}
		}

		public void Update(GameTime gameTime)
		{


		}
	}

	public enum MapLayerType
	{
		Base,
		Decal,
		Collision
	}
}
