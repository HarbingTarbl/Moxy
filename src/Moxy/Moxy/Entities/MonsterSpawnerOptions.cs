using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Moxy.Entities
{
	public static class MonsterSpawnerOptions
	{
		public static TimeSpan SpawnRate = new TimeSpan(0, 0, 2);
		public static int[] SpawnRange = new[] { 0, 100 };
		public static int MaxSpawns = 30;
		public static float SpawnDistance = 30;
		public static Random Randomizer = new Random();
	}
}
