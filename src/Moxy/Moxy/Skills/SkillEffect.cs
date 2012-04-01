using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moxy.GameStates;
using Moxy.Entities;
using Moxy.Events;
using Moxy.EventHandlers;

namespace Moxy.Skills
{
	public abstract class SkillEffect
	{
		public TimeSpan Duration;
		public bool Active;

		public abstract void Draw(SpriteBatch batch);

		public virtual void Update(GameTime gameTime)
		{
			if (!Active)
			{
				OnStart();
				Active = true;
			}

			Duration -= gameTime.ElapsedGameTime;
			
			if (Duration <= TimeSpan.Zero)
			{
				Active = false;
				OnEnd();
			}
		}

		public abstract void OnStart();
		public abstract void OnEnd();
	}

	public class TrishotEffect
		: SkillEffect
	{
		public Gunner Target;

		public TrishotEffect(Gunner Target)
		{
			this.Target = Target;
			Duration = new TimeSpan(0, 5, 0);
		}

		public override void Draw(SpriteBatch batch)
		{
			
		}

		public override void OnStart()
		{
			Target.FireballDamage -= 5;
			Target.OnCastFireball += Target_OnCastFireball;
		}

		public override void OnEnd()
		{
			Target.FireballDamage += 5;
			Target.OnCastFireball -= Target_OnCastFireball;
		}

		void Target_OnCastFireball(object sender, GunnerFireEventArgs e)
		{
			var rads = Math.Atan2(e.FireVector.Y, e.FireVector.X);
			var second = new Vector2((float)Math.Cos(rads + MathHelper.PiOver4/2), (float)Math.Sin(rads + MathHelper.PiOver4/2));
			var third = new Vector2((float)Math.Cos(rads - MathHelper.PiOver4/2), (float)Math.Sin(rads - MathHelper.PiOver4/2));
			e.Handled = true;
			Target.FireballEmitter.GenerateParticles(Moxy.GameTime, e.FireVector);
			Target.FireballEmitter.GenerateParticles(Moxy.GameTime, second);
			Target.FireballEmitter.GenerateParticles(Moxy.GameTime, third);
		}
	}


	public class ProtectionEffect
		: SkillEffect
	{
		public PowerGenerator Gen;

		public ProtectionEffect(PowerGenerator Gen)
		{
			this.Gen = Gen;
			Duration = new TimeSpan(0, 0, 10);
		}

		public override void Draw(SpriteBatch batch)
		{
			
		}

		public override void OnStart()
		{
			Gen.Defence = 5;
			Gen.Gunner.Defence = 5;
		}

		public override void OnEnd()
		{
			Gen.Defence = 0;
			Gen.Gunner.Defence = 0;
		}
	}

	public class RageEffect
		: SkillEffect
	{
		public PowerGenerator Gen;

		public RageEffect(PowerGenerator Gen)
		{
			this.Gen = Gen;
			Duration = new TimeSpan(0, 0, 10);

		}

		public override void Draw(SpriteBatch batch)
		{

		}

		public override void OnEnd()
		{
			Gen.Defence += 5;
			Gen.Gunner.Defence += 5;
			Gen.Gunner.FireballDamage -= 5;
		}

		public override void OnStart()
		{
			Gen.Defence -= 5;
			Gen.Gunner.Defence -= 5;
			Gen.Gunner.FireballDamage += 5;
		}

	}

	public class PowerEffect
		: SkillEffect
	{
		public PowerGenerator Gen;

		public PowerEffect(PowerGenerator Gen)
		{
			this.Gen = Gen;
			Duration = new TimeSpan(0, 0, 10);

		}

		public override void Draw(SpriteBatch batch)
		{

		}

		public override void OnEnd()
		{
			Gen.Gunner.EnergyRate -= 10f;
			Gen.Speed += .2f;
		}

		public override void OnStart()
		{
			Gen.Speed -= .2f;
			Gen.Gunner.EnergyRate += 10f;
		}

	}
}
