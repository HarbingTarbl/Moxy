using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moxy.Map;

namespace Moxy.GameStates
{
	public class MenuState
		: BaseGameState
	{
		public MenuState()
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
			derp.Draw(batch);
		}

		public override void Load()
		{
			derp.Texture = Moxy.ContentManager.Load<Texture2D>("lofi_tiles");
			derp.MapSize = new Vector2(25, 25);
			derp.TileSize = new Vector2(8, 8);
			derp.CreateTiles();
		}


		TileMap derp;
		Texture2D textureTest;
	}
}
