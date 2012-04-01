using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Moxy
{
	public class ParticleManager
	{
		public ParticleManager()
		{
			particles = new List<Particle>();
			removeQueue = new Queue<Particle>();
		}

		public virtual void StartParticle (Particle particle)
		{
			particles.Add (particle);
		}

		public virtual void Update (GameTime gameTime)
		{
			foreach (Particle particle in particles)
			{
				particle.TimePassed += (float)gameTime.ElapsedGameTime.TotalSeconds;

				if (particle.Target != null)
				{
					particle.CurrentLocation = Vector2.SmoothStep (particle.Original, particle.Target.Location,
						MathHelper.Clamp (particle.TimePassed / particle.Time, 0, 1));
					particle.Light.Location = particle.CurrentLocation;
				}

				if (particle.IsDead)
					removeQueue.Enqueue (particle);
			}

			while (removeQueue.Count > 0)
				particles.Remove (removeQueue.Dequeue ());
		}

		public void Draw (SpriteBatch batch)
		{
			foreach (Particle particle in particles)
				particle.Draw (batch);
		}

		public List<Particle> particles;
		public Queue<Particle> removeQueue; 
	}
}
