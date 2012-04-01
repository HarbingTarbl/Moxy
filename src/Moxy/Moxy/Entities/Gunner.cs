using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moxy.ParticleSystems;
using Microsoft.Xna.Framework.Input;
using Moxy.Events;
using Moxy.EventHandlers;
using Microsoft.Xna.Framework.Audio;

namespace Moxy.Entities
{
	public class Gunner
		: Player
	{
		public Gunner()
		{
			PowerCircleTexture = Moxy.ContentManager.Load<Texture2D>("Radius");
			Texture = Moxy.ContentManager.Load<Texture2D>("Team1SpriteSheet");
			Animations = new AnimationManager(Texture,
				new Animation[] 
				{
					new Animation("Idle_1", new Rectangle[]
					{
						new Rectangle(0, 0, 64, 64)
					}),
					new Animation("Idle_2", new Rectangle[]
					{
						new Rectangle(0, 64, 64, 64)
					}),
					new Animation("Idle_3", new Rectangle[]
					{
						new Rectangle(0, 128, 64, 64)
					}),
					new Animation("Walk_1", new Rectangle[] 
					{
						new Rectangle(0, 0, 64, 64),
						new Rectangle(64, 0, 64, 64),
						new Rectangle(128, 0, 64, 64),
					}, new TimeSpan(0, 0, 0, 0, 200)),
					new Animation("Walk_2", new Rectangle[] 
					{
						new Rectangle(0, 64, 64, 64),
						new Rectangle(64, 64, 64, 64),
						new Rectangle(128, 64, 64, 64),
					}, new TimeSpan(0, 0, 0, 0, 200)),
					new Animation("Walk_3", new Rectangle[] 
					{
						new Rectangle(0, 128, 64, 64),
						new Rectangle(64, 128, 64, 64),
						new Rectangle(128, 128, 64, 64),
					}, new TimeSpan(0, 0, 0, 0, 200))

				});
			EntityType = global::Moxy.EntityType.Gunner;
			Animations.SetAnimation("Walk_1");
			fireSound = Moxy.ContentManager.Load<SoundEffect>("Sounds//Fire");
			fireSoundInstance = fireSound.CreateInstance();
			Health = 100;
			CircleOrigin = new Vector2(PowerCircleTexture.Width / 2f, PowerCircleTexture.Height / 2f);
		}

		public Texture2D PowerCircleTexture;
		public readonly Vector2 CircleOrigin;

		public FireballEmitter FireballEmitter
		{
			get
			{
				return fireballEmitter;
			}
			set
			{
				value.Gunner = this;
				fireballEmitter = value;
				fireballEmitter.OnParticleMonsterCollision += new EventHandler<Events.GenericEventArgs<Monster>>(fireballEmitter_OnParticleMonsterCollision);
			}
		}

		public float OverloadLevel = 0;
		public float MaxOverloadLevel = 500;
		public float OverloadRate = 1;

		public SoundEffect fireSound;

		public float Energy;
		public float MaxEnergy = 500;
		public float EnergyRate;
		public float ExtraEnergyRate;

		public override void Draw(SpriteBatch batch)
		{
			var redVector = Color.Red.ToVector3();
			var whiteVector = Color.White.ToVector3();
			var interpol = Vector3.SmoothStep(whiteVector, redVector, (float)Math.Sin(OverloadRate));
			var interpolColor = new Color(interpol);

			batch.Draw(PowerCircleTexture, this.CollisionCenter, null, interpolColor, baseRotation, CircleOrigin, 1f, SpriteEffects.None, 0f);
			batch.Draw(Texture, new Rectangle((int)Location.X, (int)Location.Y, 64, 64), Animations.Bounding, Color, Rotation - MathHelper.PiOver2, new Vector2(32, 32), SpriteEffects.None, 0);
		}

		public override void Update(GameTime gameTime)
		{
			Health = Math.Min(Generator.Health, Health);
			HandleInput(gameTime);
			OverloadLevel += OverloadRate;

			if (OverloadLevel >= MaxOverloadLevel)
			{
				Moxy.StateManager.Set("MainMenu");
				MaxOverloadLevel = 0;
				OverloadRate = 0;
				if (OnOverLoadExeeded != null)
					OnOverLoadExeeded(this, null);
			}
			if (!Generator.PowerDisabled)
			{
				Energy += (EnergyRate + ExtraEnergyRate)* (float)gameTime.ElapsedGameTime.TotalSeconds;
			}
			Energy = Math.Min(Energy, MaxEnergy);
			baseRotation += MathHelper.PiOver4 * (float)gameTime.ElapsedGameTime.TotalSeconds;
			baseRotation %= MathHelper.TwoPi;
			Animations.Update(gameTime);

			base.Update(gameTime);
		}

		private void HandleInput(GameTime gameTime)
		{
			var rand = new Random();
			SkillRate += gameTime.ElapsedGameTime;
			var padState = GamePad.GetState(PadIndex);
			if (padState.ThumbSticks.Right !=Vector2.Zero && Energy >= 10 && SkillRate > FireballRate)
			{
				SkillRate = TimeSpan.Zero;
				Energy -= 10;
				var direction = Vector2.Normalize(padState.ThumbSticks.Right);
				direction.Y = -direction.Y;

				var fireEventArgs = new GunnerFireEventArgs (direction);
				fireSound.Play(0.8f, 0f, 0f);
				if (OnCastFireball != null)
					OnCastFireball(this, fireEventArgs);

				if (!fireEventArgs.Handled)
					fireballEmitter.GenerateParticles(gameTime, direction);
			}
		}

		private void fireballEmitter_OnParticleMonsterCollision(object sender, Events.GenericEventArgs<Monster> e)
		{
			e.Data.Health -= FireballDamage;
		}


		private FireballEmitter fireballEmitter;
		public TimeSpan FireballRate = new TimeSpan(0, 0, 0, 0, 500);
		public float FireballDamage = 10f;
		public PowerGenerator Generator;
		SoundEffectInstance fireSoundInstance;
		private TimeSpan SkillRate;
		private float baseRotation;

		public event EventHandler OnOverLoadExeeded;
		public event EventHandler<GunnerFireEventArgs> OnCastFireball;
	}
}
