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
			AmbientLight = new Color (255, 255, 255, 200);
			WaveLength = 120;

			AddMonster (1, "Slime");

		}
	}
}
