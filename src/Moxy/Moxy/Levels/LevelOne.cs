using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Moxy.Entities;

namespace Moxy.Levels
{
	public class LevelOne
		: BaseLevel
	{
		public LevelOne()
		{
			AmbientLight = new Color(10, 10, 10, 200);
			WaveLength = 30;
			MaxMonsters = 20;
			SpawnIntervalLow = 1f;
			SpawnIntervalHigh = 2.4f;

			AddMonster (1, "Slime");
		}

	}
}
