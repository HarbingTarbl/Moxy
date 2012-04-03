using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Moxy.Levels
{
	public class LevelFour
		: BaseLevel
	{
		public LevelFour()
		{
			AmbientLight = new Color (10, 10, 10, 200);
			WaveLength = 30;
			MaxMonsters = 25;
			SpawnIntervalLow = 0.5f;
			SpawnIntervalHigh = 0.8f;

			AddMonster (5, "Slime");
			AddMonster (2, "EyeBall");

		}
	}
}
