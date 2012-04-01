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
					//return;
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
			MatchArray[0] = ItemID.RedPowerup;
			MatchArray[1] = ItemID.RedPowerup;
			MatchArray[2] = ItemID.RedPowerup;
			MatchArray[3] = ItemID.RedPowerup;
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
			MatchArray[0] = ItemID.BluePowerup;
			MatchArray[1] = ItemID.GreenPowerup;
			MatchArray[2] = ItemID.GreenPowerup;
			MatchArray[3] = ItemID.RedPowerup;
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
			MatchArray[0] = ItemID.BluePowerup;
			MatchArray[1] = ItemID.YellowPowerup;
			MatchArray[2] = ItemID.GreenPowerup;
			MatchArray[3] = ItemID.GreenPowerup;
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
			MatchArray[0] = ItemID.YellowPowerup;
			MatchArray[1] = ItemID.GreenPowerup;
			MatchArray[2] = ItemID.RedPowerup;
			MatchArray[3] = ItemID.BluePowerup;
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
