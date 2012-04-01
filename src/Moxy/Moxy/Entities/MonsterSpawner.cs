using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace Moxy.Entities
{
	public class MonsterSpawner
	{
		public TileMap Map;
		public Vector2 Location;
		public string MonsterType;
		public int MonsterCount;

		public MonsterSpawner(TileMap Map)
		{
			this.Map = Map;

		}

		public Monster Spawn(GameTime gameTime)
		{
			Monster monster = null;
			var range = MonsterSpawnerOptions.Randomizer.Next(0, 101);


			///SOMETHING WITH STUFF
			lastSpawn += gameTime.ElapsedGameTime;
			if (MonsterCount < MonsterSpawnerOptions.MaxSpawns && MonsterSpawnerOptions.SpawnRange[0] < range && MonsterSpawnerOptions.SpawnRange[1] > range
				&& (lastSpawn > MonsterSpawnerOptions.SpawnRate))
			{
				lastSpawn = TimeSpan.Zero;
				switch (MonsterType)
				{
					case "Slime":
						MonsterCount++;
						monster = new Slime();
						monster.OnDeath += OnMonsterDeath;
						break;
				}
			}


			return monster;
		}

		[Conditional("DEBUG")]
		public void Draw(SpriteBatch batch)
		{
			Monster mon = null;
			switch (MonsterType)
			{
				case "Slime":
					mon = new Slime();
					mon.Location = this.Location;
					break;
			}

			mon.Draw(batch);
		}

		public void OnMonsterDeath(object sender, EventArgs e)
		{
			MonsterCount--;
		}

		private TimeSpan lastSpawn;

	}
}
