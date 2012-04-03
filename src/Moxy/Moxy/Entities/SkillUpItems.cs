using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Moxy.Entities
{
	public class FireRuneItem
		: Item
	{
		public FireRuneItem()
		{
			CollisionRadius = 25;
			ItemID = ItemID.FireRune;
			Bounds = new Rectangle(132, 100, 24, 24);
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

	public class WaterRuneItem
		: Item
	{
		public WaterRuneItem()
		{
			CollisionRadius = 25;
			ItemID = ItemID.WaterRune;
			Bounds = new Rectangle(132, 4, 24, 24);
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

	public class WindRuneItem
		: Item
	{
		public WindRuneItem()
		{
			CollisionRadius = 25;
			ItemID = ItemID.WindRune;
			Bounds = new Rectangle(292, 68, 24, 24);
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

	public class EarthRuneItem
		: Item
	{
		public EarthRuneItem()
		{
			CollisionRadius = 25;
			ItemID = ItemID.EarthRune;
			Bounds = new Rectangle(292, 4, 24, 24);
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
