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
			Dudes.Add(new Gunner()
			{
				Team = Team.Blue,
				Location = new Vector2(0, 0),
				Color = Color.White,
				Speed = .5f,
				PadIndex = PlayerIndex.One
			});
			Camera = new Camera(Moxy.ScreenWidth, Moxy.ScreenHeight);
			Monsters = new List<Monster>();
			Map.Camera = Camera;
			//Camera.UseBounds = true;
			Camera.Bounds = new Rectangle(0, 0, 1600, 1600);
			
		}


		public override void Update(GameTime gameTime)
		{
			Map.Update(gameTime);
			Dudes.ForEach(dude => dude.Update(gameTime));
			if (!Map.EditActive)
				Camera.CenterOnPoint(Dudes[0].Location);
			foreach (var spawner in Map.MonsterSpawners)
			{
				var monster = spawner.Spawn(gameTime);
				if(monster != null)
					Monsters.Add(monster);
			}

			foreach (var monster in Monsters)
			{
				monster.Rotation = (float)Math.Atan2(Dudes[0].Location.Y - monster.Location.Y, Dudes[0].Location.X - monster.Location.X);
				if(Dudes[0].Location.Y - monster.Location.Y < 0 || Dudes[0].Location.X - monster.Location.X < 0)
					monster.Rotation = -monster.Rotation;


				monster.Update(gameTime);
			}
		}

		public override void Draw(SpriteBatch batch)
		{
			batch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, Camera.Transformation);
			Map.Draw(batch);
			batch.End();
			batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, Camera.Transformation);
			Dudes.ForEach(dude => dude.Draw(batch));
			Monsters.ForEach(monster => monster.Draw(batch));
			if (Map.EditActive)
				Map.DrawEditor(batch);
			batch.End();
		}

		public override void OnFocus()
		{
			Moxy.StateManager.Push(UI);
			base.OnFocus();
		}

		public override void Load()
		{
			Map.Texture = Moxy.ContentManager.Load<Texture2D>("lofi_tiles");
			Map.MapSize = new Vector2(50, 50);
			Map.TileSize = new Vector2(8, 8);
			Map.CreateTiles();
			//UI = (UIOverlay)Moxy.StateManager["UIOverlay"];
			//UI.ActivePlayers = Dudes;
		}

		public List<Monster> Monsters;
		public UIOverlay UI;
		public TileMap Map;
		public List<Player> Dudes;
		public Camera Camera;
	}
}
