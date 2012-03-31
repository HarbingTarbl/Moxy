using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Moxy.Entities;

namespace Moxy.ParticleSystems
{
	public class FireballEmitter
		: ParticleManager
	{
		public static Texture2D ParticleTexture = Moxy.ContentManager.Load<Texture2D>("Fireball");

		public Gunner Gunner;

		// Energy generation
		public TimeSpan particleDelay = new TimeSpan(0, 0, 0, 0, 0);
		public float particleDamage = 20;
		public float maxParticleRange = 1000;

		public override void Update(GameTime gameTime)
		{
			foreach (Particle particle in particles)
			{
				particle.TimePassed += (float)gameTime.ElapsedGameTime.TotalSeconds;

				particle.CurrentLocation = Vector2.Lerp(particle.Original, particle.EndLocation,
					MathHelper.Clamp(particle.TimePassed / particle.Time, 0, 1));

				if (particle.IsDead)
					removeQueue.Enqueue(particle);
			}

			while (removeQueue.Count > 0)
				particles.Remove(removeQueue.Dequeue());
		}

		public void GenerateParticles(GameTime gameTime, Vector2 Direction)
		{	
			timeSinceLast += gameTime.ElapsedGameTime;
			if(timeSinceLast > particleDelay)
			{
				timeSinceLast = TimeSpan.Zero;
				var particle = new Particle(Gunner.Location, ParticleTexture, 1f, 1f)
				{
					EndLocation = Gunner.Location + (Direction * maxParticleRange),
				};
				base.StartParticle(particle);
			}
		}

		private TimeSpan timeSinceLast;
	}
}
