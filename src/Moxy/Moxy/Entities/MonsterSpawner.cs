using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moxy;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace Moxy.Entities
{
	public class MonsterSpawner
	{
		public Vector2 Location;
		public string MonsterType;
		public int MonsterCount;
		public float TimeSinceLastSpawn;
		public float SpawnDelay;
		public float MaxSpawns;

		public bool NeedsSpawn
		{
			get { return TimeSinceLastSpawn > SpawnDelay && MonsterCount < MaxSpawns; }
		}

		public Monster Spawn(GameTime gameTime)
		{
			Monster monster = null;
			switch (MonsterType)
			{
				case "Slime":
					monster = new Slime ();
					break;
			}

			TimeSinceLastSpawn = 0;
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
