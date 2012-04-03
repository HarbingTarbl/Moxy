using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moxy.EventHandlers;

namespace Moxy.Entities
{
	public class Player
		: Entity
	{
		public Player ()
		{
			texture = new Texture2D (Moxy.Graphics, 1, 1);
			texture.SetData (new [] { Color.White });
			OnDeath += new EventHandler(Player_OnDeath);
		}

		void Player_OnDeath(object sender, EventArgs e)
		{
			//TODO: Remove on death
			//Moxy.StateManager.Set("MainMenu");
		}


		public string Animation
		{
			get
			{
				return animation;
			}

			set
			{
				animation = value;
				Animations.SetAnimation(animation + "_" + level.ToString());
			}
		}
		public int Level
		{
			get
			{
				return level;
			}
			set
			{
				level = value;
				Animations.SetAnimation(animation + "_" + level.ToString());
			}
		}


		protected string animation;
		protected int level = 1;
		public PlayerIndex PadIndex;
		public float PlayerScore;
		public float Speed;
		public float Defence;
		public Color Color;
		public Team Team;
		public Light Light;

		public bool MovementDisabled;

		public void Damage(float amount)
		{
			GamePad.SetVibration(PadIndex, 0, MathHelper.Lerp(amount, 100, 1));
			Health -= Math.Max (0, (amount - Defence));
			GamePad.SetVibration(PadIndex, 0, 0);
		}

		public override void Update (GameTime gameTime)
		{
			HandleInput (gameTime);

			Light.Location = Location + new Vector2(32, 32);
			if (Health <= 0 && OnDeath != null)
				OnDeath(this, null);

			CollisionCenter = Location;
			Collision = new Rectangle((int)CollisionCenter.X, (int)CollisionCenter.Y, 1, 1);
			CollisionRadius = 32f;
			Collision.Inflate((int)CollisionRadius, (int)CollisionRadius);
		}

		public override void Draw (SpriteBatch batch)
		{
			batch.Draw (texture, new Rectangle((int)Location.X, (int)Location.Y, 64, 64), null, Color);
		}

		private void HandleInput (GameTime gameTime)
		{
			GamePadState currentPad = GamePad.GetState (PadIndex);

			Vector2 moveVector = currentPad.ThumbSticks.Left;
			if (moveVector.Length() > 0)
				moveVector.Normalize();

			moveVector.Y *= -1;

			if (lastMovement == Vector2.Zero && moveVector != Vector2.Zero)
				Animation = "Walk";
			else if (moveVector == Vector2.Zero)
				Animation = "Idle";

			Health = MathHelper.Clamp(Health, 0, MaxHealth);

			var playerMoveEventArgs = new PlayerMovementEventArgs()
			{
				NewLocation = this.Location + moveVector * Speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds,
				Handled = false,
				Player = this,
				CurrentLocation = this.Location
			};

			if (OnMovement != null)
				OnMovement(this, playerMoveEventArgs);

			if (!playerMoveEventArgs.Handled)
				base.Location += moveVector * Speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
			else
				base.Location = playerMoveEventArgs.NewLocation;

			if (moveVector.Length() != 0)
				base.Rotation = (float)Math.Atan2 (moveVector.Y, moveVector.X);

			lastMovement = moveVector;
			oldPad = currentPad;
		}



		public event EventHandler OnDeath;
		public event EventHandler<PlayerMovementEventArgs> OnMovement;

		private Vector2 lastMovement;
		private GamePadState oldPad;
		private Texture2D texture;
	}
}
