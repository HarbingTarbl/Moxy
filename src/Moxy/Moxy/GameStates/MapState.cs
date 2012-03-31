using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moxy.Map;

namespace Moxy.GameStates
{
	public class MapState
		: BaseGameState
	{
		public MapState()
			: base("MenuState", isOverlay:false, acceptsInput:true)
		{
			derp = new TileMap();
		
		}
	
		public override void Update(GameTime gameTime)
		{
			derp.Update(gameTime);
		}

		public override void Draw (SpriteBatch batch)
		{
			batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
			derp.Draw(batch);
			batch.End();
		}

		public override void OnFocus()
		{
			derp.Texture = Moxy.ContentManager.Load<Texture2D>("lofi_tiles");
			derp.MapSize = new Vector2(5, 5);
			derp.TileSize = new Vector2(8, 8);

			derp.CreateTiles("map.bin");
		}

		public override void Load()
		{

		}


		public TileMap derp;
		Texture2D textureTest;
	}
}
