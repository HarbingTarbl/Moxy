using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Moxy.Levels
{
	public class LevelSix
		: BaseLevel
	{
		public LevelSix()
		{
			AmbientLight = new Color (10, 10, 10, 200);
			WaveLength = 60;
			MaxMonsters = 33;
			SpawnIntervalLow = 0.7f;
			SpawnIntervalHigh = 0.8f;

			AddMonster (7, "Slime");
			AddMonster (2, "EyeBall");
			AddMonster (4, "Demon");

		}
	}
}

