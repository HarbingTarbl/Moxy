using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Moxy.Levels
{
	public class LevelFive
		: BaseLevel
	{
		public LevelFive()
		{
			AmbientLight = new Color (10, 10, 10, 200);
			WaveLength = 35;
			MaxMonsters = 20;
			SpawnIntervalLow = 0.5f;
			SpawnIntervalHigh = 0.8f;

			AddMonster (3, "Slime");
			AddMonster (2, "EyeBall");
			AddMonster (1, "Demon");

		}
	}
}
