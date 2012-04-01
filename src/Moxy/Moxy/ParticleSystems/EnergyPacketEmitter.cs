using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Moxy.Entities;
using Microsoft.Xna.Framework;

namespace Moxy.ParticleSystems
{
	public class EnergyPacketEmitter
		: ParticleManager
	{
		public static Texture2D ParticleTexture = Moxy.ContentManager.Load<Texture2D>("powerparticle");
		public PowerGenerator Source;
		public Gunner Target;

		// Energy generation
		public float maxParticleDelay = 0.4f;
		public float minParticleDelay = 0.1f;
		public float maxPowerGeneration = 20;
		public float minPowerGeneration = 0;
		public float minPowerRange = 100;
		public float maxPowerRange = 342;

		public void CalculateEnergyRate(GameTime gameTime)
		{
			float distance = Vector2.Distance(Target.Location, Source.Location);
			if (distance < minPowerRange)
			{
				Target.OverloadLevel += 2 * Target.OverloadDecayRate * (float)gameTime.ElapsedGameTime.TotalSeconds;
			}
			float lerp = MathHelper.Clamp((distance - minPowerRange) / maxPowerRange, 0, 1);

			Target.EnergyRate = MathHelper.Lerp(maxPowerGeneration, minPowerGeneration, lerp);
			Source.ParticleDelay = MathHelper.SmoothStep(minParticleDelay, maxParticleDelay, lerp);
			Source.PowerDisabled = distance > maxPowerRange;
		}

		public void GenerateParticles(GameTime gameTime)
		{
			Source.ParticleTimePassed += (float)gameTime.ElapsedGameTime.TotalSeconds;

			if (Source.ParticleTimePassed > Source.ParticleDelay && !Source.PowerDisabled)
			{
				var particle = new Particle(Source.Location, ParticleTexture, 2f, 1f) { Target = Target };
				base.StartParticle(particle);
				Source.ParticleTimePassed = 0;
			}
		}


	}
}
