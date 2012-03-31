using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Moxy.Skills
{
	public abstract class SkillEffect
	{
		public abstract void Draw(SpriteBatch batch);
		public abstract void Update(GameTime gameTime);
	}
}
