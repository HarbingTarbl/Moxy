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
			: base("MapState", isOverlay:false, acceptsInput:true)
		{
			derp = new TileMap();
		}
	
		public override void Update(GameTime gameTime)
		{
			derp.Update(gameTime);
		}

		public override void Draw (SpriteBatch batch)
		{
			batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default,
				RasterizerState.CullCounterClockwise, null, derp.Camera.Transformation);

			derp.Draw(batch);
			batch.End();
		}

		public override void OnFocus()
		{
			derp.Texture = Moxy.ContentManager.Load<Texture2D> ("tileset");
			derp.MapSize = new Vector2(256, 256);
			derp.TileSize = new Vector2(64, 64);
			derp.Camera = new Camera2D (800, 600);
			derp.Camera.CenterOnPoint (0, 0);
			derp.CreateTiles("Content/map1.bin");
		}

		public TileMap derp;
	}
}
