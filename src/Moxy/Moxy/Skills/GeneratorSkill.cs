using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moxy.Entities;
using Microsoft.Xna.Framework.Graphics;
using Moxy.Skills;

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

	public class TriShotSkill
		: GeneratorSkill
	{
		public TriShotSkill(PowerGenerator Gen)
			: base(Gen)
		{
			MatchArray[0] = ItemID.FireRune;
			MatchArray[1] = ItemID.FireRune;
			MatchArray[2] = ItemID.FireRune;
			MatchArray[3] = ItemID.FireRune;
		}

		public override bool OnSkillUsed(PowerGenerator Gen)
		{
			var newSkill = new TrishotEffect(Gen.Gunner);
			foreach(var skill in Gen.ActiveSkills)
			{
				if (skill.SkillID == newSkill.SkillID)
				{
					skill.Duration = newSkill.Duration;
					return false;
				}

			}

			Gen.ActiveSkills.Add(newSkill);

			return true;
		}
	}

	public class ProtectionSkill
		: GeneratorSkill
	{
		public ProtectionSkill(PowerGenerator Gen)
			: base(Gen)
		{
			MatchArray[0] = ItemID.WaterRune;
			MatchArray[1] = ItemID.EarthRune;
			MatchArray[2] = ItemID.EarthRune;
			MatchArray[3] = ItemID.FireRune;
		}



		public override bool OnSkillUsed(PowerGenerator Gen)
		{
			var newSkill = new ProtectionEffect(Gen);
			foreach(var skill in Gen.ActiveSkills)
			{
				if (skill.SkillID == newSkill.SkillID)
				{
					skill.Duration = newSkill.Duration;
				}
				return false;
			}

			Gen.ActiveSkills.Add(newSkill);

			return true;
		}
	}

	public class RageSkill
		: GeneratorSkill
	{
		public RageSkill(PowerGenerator Gen)
			: base(Gen)
		{
			MatchArray[0] = ItemID.WaterRune;
			MatchArray[1] = ItemID.WindRune;
			MatchArray[2] = ItemID.EarthRune;
			MatchArray[3] = ItemID.EarthRune;
		}

		public override bool OnSkillUsed(PowerGenerator Gen)
		{
			var newSkill = new RageEffect(Gen);
			foreach(var skill in Gen.ActiveSkills)
			{
				if (skill.SkillID == newSkill.SkillID)
				{
					skill.Duration = newSkill.Duration;
				}
				return false;
			}

			Gen.ActiveSkills.Add(newSkill);

			return true;
		}
	}

	public class PowerSKill
		: GeneratorSkill
	{
		public PowerSKill(PowerGenerator Gen)
			: base(Gen)
		{
			MatchArray[0] = ItemID.WindRune;
			MatchArray[1] = ItemID.EarthRune;
			MatchArray[2] = ItemID.FireRune;
			MatchArray[3] = ItemID.WaterRune;
		}


		public override bool OnSkillUsed(PowerGenerator Gen)
		{
			var newSkill = new PowerEffect(Gen);
			foreach(var skill in Gen.ActiveSkills)
			{
				if (skill.SkillID == newSkill.SkillID)
				{
					skill.Duration = newSkill.Duration;
				}
				return false;
			}

			Gen.ActiveSkills.Add(newSkill);

			return true;
		}

	}
}
