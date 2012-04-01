using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Moxy.Entities
{
	public class RedSkillUp
		: Item
	{
		public RedSkillUp()
		{
			CollisionRadius = 25;
			base.Color = Color.Red;
			ItemID = ItemID.RedPowerup;
		}

		public override bool OnPlayerCollision(Player p)
		{
			var gen = p as PowerGenerator;
			if (gen == null)
				return false;

			Moxy.ContentManager.Load<SoundEffect> ("Sounds\\PowerupPickup").Play ();
			gen.ApplyPowerup(this);
			return true;
		}
	}

	public class BlueSkillUp
		: Item
	{
		public BlueSkillUp()
		{
			CollisionRadius = 25;
			base.Color = Color.Aquamarine;
			ItemID = ItemID.BluePowerup;
		}

		public override bool OnPlayerCollision(Player p)
		{
			var gen = p as PowerGenerator;
			if (gen == null)
				return false;

			Moxy.ContentManager.Load<SoundEffect> ("Sounds\\PowerupPickup").Play ();
			gen.ApplyPowerup(this);
			return true;
		}
	}

	public class YellowSkillUp
		: Item
	{
		public YellowSkillUp()
		{
			CollisionRadius = 25;
			base.Color = Color.Yellow;
			ItemID = ItemID.YellowPowerup;
		}

		public override bool OnPlayerCollision(Player p)
		{
			var gen = p as PowerGenerator;
			if (gen == null)
				return false;

			Moxy.ContentManager.Load<SoundEffect> ("Sounds\\PowerupPickup").Play ();
			gen.ApplyPowerup(this);
			return true;
		}
	}

	public class GreenSkillUp
		: Item
	{
		public GreenSkillUp()
		{
			CollisionRadius = 25;
			base.Color = Color.Green;
			ItemID = ItemID.GreenPowerup;
		}

		public override bool OnPlayerCollision(Player p)
		{
			var gen = p as PowerGenerator;
			if (gen == null)
				return false;

			Moxy.ContentManager.Load<SoundEffect> ("Sounds\\PowerupPickup").Play ();
			gen.ApplyPowerup(this);
			return true;
		}
	}

	public class HealthItem
		: Item
	{
		public HealthItem()
		{
			CollisionRadius = 25;
			ItemID = ItemID.HealthPowerup;
		}

		public override bool OnPlayerCollision(Player p)
		{
			p.Health += 50;
			return true;
		}
	}

	public class ManaItem
		: Item
	{
		public ManaItem()
		{
			CollisionRadius = 25;
			ItemID = ItemID.ManaPowerup;
		}

		public override bool OnPlayerCollision(Player p)
		{
			var gun = p as Gunner;
			if (gun == null)
				return false;

			gun.Energy += 20;
			return true;
		}
	}
}
