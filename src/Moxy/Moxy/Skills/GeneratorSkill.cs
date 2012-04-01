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
			for (var x = 0; x < 4; x++)
				Gen.CurrentRunes[x] = ItemID.None;
		}

		public abstract bool OnSkillUsed(PowerGenerator Gen);
	}

	public class DerpHerp
		: GeneratorSkill
	{
		public DerpHerp(PowerGenerator Gen)
			: base(Gen)
		{
			MatchArray[0] = ItemID.RedPowerup;
			MatchArray[1] = ItemID.RedPowerup;
			MatchArray[2] = ItemID.RedPowerup;
			MatchArray[3] = ItemID.RedPowerup;
		}

		public override bool OnSkillUsed(PowerGenerator Gen)
		{
			Gen.ActiveSkills.Add(new TrishotEffect(Gen.Gunner));
			return true;
		}
	}
}
