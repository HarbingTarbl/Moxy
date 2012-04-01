using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Moxy.Entities;

namespace Moxy.Levels
{
	public class BaseLevel
	{
		public BaseLevel()
		{
			Monsters = new List<MonsterSpawnInformation>();
		}

		public Color AmbientLight;
		public float WaveLength;
		public float SpawnIntervalLow;
		public float SpawnIntervalHigh;
		public float SpawnDelay;
		public int MaxMonsters;
		public List<MonsterSpawnInformation> Monsters;

		public void AddMonster(int chance, string type)
		{
			Monsters.Add (new MonsterSpawnInformation (chance, type));
			randomTotal += chance;

			CalculateRandomRange();
		}

		public Monster SpawnMonsterRandom()
		{
			var random = Moxy.Random.NextDouble ();

			foreach (var info in Monsters)
			{
				if (info.ValueInRange (random))
					return SpawnMonster (info.MonsterType);
			}

			throw new ArgumentOutOfRangeException("Out of spawn range?");
		}

		public Monster SpawnMonster(string monsterType)
		{
			Monster monster = null;
			switch (monsterType)
			{
				case "Slime":
					monster = new Slime ();
					break;
				case "Demon":
					monster = new Demon();
					break;
				case "EyeBall":
					monster = new EyeBall();
					break;
				case "AngryRock":
					monster = new AngryRock();
					break;
			}

			monster.MovementSpeed = Moxy.Random.Next ((int)monster.LowSpeed, (int)monster.HighSpeed);
			return monster;
		}

		private int randomTotal;

		private void CalculateRandomRange()
		{
			double rangeEnd = 0;
			foreach (MonsterSpawnInformation info in Monsters)
			{
				info.Range = (float)info.SpawnChance / (float)randomTotal;
				info.RandomLow = rangeEnd;
				info.RandomHigh = info.RandomLow + info.Range;
				rangeEnd += info.Range;
			}
		}
	}

	public class MonsterSpawnInformation
	{
		public MonsterSpawnInformation(int spawnChance, string monsterType)
		{
			this.SpawnChance = spawnChance;
			this.MonsterType = monsterType;
		}

		public int SpawnChance;
		public string MonsterType;
		public double Range;
		public double RandomLow;
		public double RandomHigh;

		public bool ValueInRange (double value)
		{
			return value > RandomLow && value < RandomHigh;
		}
	}

}
