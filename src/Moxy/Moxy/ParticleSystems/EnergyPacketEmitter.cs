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
		public float maxPowerGeneration = 30;
		public float minPowerGeneration = 0;
		public float minPowerRange = 100;
		public float maxPowerRange = 450;

		public void CalculateEnergyRate(GameTime gameTime)
		{
			float distance = Vector2.Distance(Target.Location, Source.Location);
			if (distance < minPowerRange)
			{
				Target.OverloadRate += (MathHelper.Pi * (float)gameTime.ElapsedGameTime.TotalSeconds * (minPowerRange / distance));
			}
			else
				Target.OverloadRate = 0;
			float lerp = MathHelper.Clamp((distance - minPowerRange) / maxPowerRange, 0, 1);

			Source.PowerDisabled = distance > maxPowerRange || OldLocation == Source.Location;
			Target.EnergyRate = MathHelper.Lerp(maxPowerGeneration, minPowerGeneration, lerp);
			Source.ParticleDelay = MathHelper.SmoothStep(minParticleDelay, maxParticleDelay, lerp);

			OldLocation = Source.Location;
		}

		public void GenerateParticles(GameTime gameTime)
		{
			Source.ParticleTimePassed += (float)gameTime.ElapsedGameTime.TotalSeconds;

			if (Source.ParticleTimePassed > Source.ParticleDelay && !Source.PowerDisabled)
			{
				var particle = new Particle(Source.Location, ParticleTexture, 2f, 1f)
				{
					Target = Target,
					Light = new Light(Color.Teal)
					{
						Scale = 1f,
					}
				};
				base.StartParticle(particle);
				Source.ParticleTimePassed = 0;
			}
		}

		private Vector2 OldLocation;


	}
}
