using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moxy.Entities;
using Microsoft.Xna.Framework.Graphics;

namespace Moxy.Skills
{
	public abstract class GeneratorSkill
	{
		public GeneratorSkill(PowerGenerator Gen)
		{
			MatchArray = new ItemID[4];
			this.Gen = Gen;
		}

		public PowerGenerator Gen;
		public Texture2D Texture;
		public ItemID[] MatchArray;

		public void Activate(IEnumerable<ItemID> CurrentItems)
		{
			int currentId = 0;
			foreach(var id in CurrentItems)
			{
				if(MatchArray[currentId] != id)
					return;
				currentId++;
			}
			OnSkillUsed(Gen);
		}

		public abstract bool OnSkillUsed(PowerGenerator Gen);
	}

	public class DerpHerp
		: GeneratorSkill
	{
		public DerpHerp(PowerGenerator Gen)
			: base(Gen)
		{
			MatchArray[0] = ItemID.None;
			MatchArray[1] = ItemID.None;
			MatchArray[2] = ItemID.None;
			MatchArray[3] = ItemID.None;
		}

		public override bool OnSkillUsed(PowerGenerator Gen)
		{
			throw new NotImplementedException();
		}
	}
}
