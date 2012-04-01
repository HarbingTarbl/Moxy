using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Moxy.Levels
{
	public class LevelOne
		: BaseLevel
	{
		public LevelOne()
		{
			AmbientLight = new Color(0, 0, 0, 200);
			WaveLength = 3;
			MaxMonsters = 10;
			SpawnIntervalLow = 0.5f;
			SpawnIntervalHigh = 0.8f;
			
			AddMonster (1, "Slime");

		}
	}
}
