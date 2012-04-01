using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Moxy.Entities;
using Moxy.Events;

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

		public event EventHandler<GenericEventArgs<Monster>> OnParticleMonsterCollision;

		public void CheckCollision(Monster entity)
		{
			foreach (var particle in particles)
			{
				if (particle.IsDead)
					continue;

				if (Vector2.Distance(entity.CollisionCenter, particle.CurrentLocation) < (particle.Size + entity.CollisionRadius ))
				{
					if(OnParticleMonsterCollision != null)
						OnParticleMonsterCollision(particle, new GenericEventArgs<Monster>(entity));
					//KEEEL THE PARTICALLL
					particle.TimePassed = particle.Time;

				}
			}
		}

		public override void Update(GameTime gameTime)
		{
			foreach (Particle particle in particles)
			{
				particle.TimePassed += (float)gameTime.ElapsedGameTime.TotalSeconds;

				particle.CurrentLocation = Vector2.Lerp(particle.Original, particle.EndLocation,
					MathHelper.Clamp(particle.TimePassed / particle.Time, 0, 1));
				particle.Light.Location = particle.CurrentLocation;

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
					Rotation = (float)Math.Atan2 (Direction.Y, Direction.X),
					Size = 10f,
					Scale = 0.35f,
					Light = new Light(Color.OrangeRed)
				};
				base.StartParticle(particle);
			}
		}

		private TimeSpan timeSinceLast;
	}
}
