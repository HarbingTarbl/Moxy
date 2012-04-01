using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Moxy.Levels
{
	public class LevelTwo
		: BaseLevel
	{
		public LevelTwo()
		{
			AmbientLight = new Color (0, 0, 0, 10);
			WaveLength = 120;
			MaxMonsters = 10;
			SpawnIntervalLow = 0.5f;
			SpawnIntervalHigh = 0.8f;

			AddMonster (1, "Slime");

		}
	}
}
