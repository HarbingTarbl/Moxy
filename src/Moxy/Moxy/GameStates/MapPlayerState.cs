using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moxy.Map;
using Moxy.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Moxy.GameStates
{
	public class MapPlayerState
		: BaseGameState
	{
		public MapPlayerState()
			: base("MapPlayerState")
		{
			Map = new TileMap();
			Dudes = new List<Player>();
			Dudes.Add(new PowerGenerator()
			{
				Team = Team.Blue,
				Location = new Vector2(0, 0),
				Color = Color.White,
				Speed = .5f,
				PadIndex = PlayerIndex.One
			});
			Camera = new Camera(Moxy.ScreenWidth, Moxy.ScreenHeight);
			Map.Camera = Camera;
			//Camera.UseBounds = true;
			//Camera.Bounds = new Rectangle(0, 0, 1600, 1600);
			
		}


		public override void Update(GameTime gameTime)
		{
			Map.Update(gameTime);
			Dudes.ForEach(dude => dude.Update(gameTime));
			if (!Map.EditActive)
				Camera.CenterOnPoint(Dudes[0].Location);
		}

		public override void Draw(SpriteBatch batch)
		{
			batch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, Camera.Transformation);
			Map.Draw(batch);
			batch.End();
			batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, Camera.Transformation);
			Dudes.ForEach(dude => dude.Draw(batch));
			batch.End();
	


		}

		public override void Load()
		{
			Map.Texture = Moxy.ContentManager.Load<Texture2D>("lofi_tiles");
			Map.MapSize = new Vector2(50, 50);
			Map.TileSize = new Vector2(8, 8);
			Map.CreateTiles();
		}

		public TileMap Map;
		public List<Player> Dudes;
		public Camera Camera;
	}
}
