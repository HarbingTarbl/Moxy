using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moxy.GameStates;
using Moxy.Events;

namespace Moxy.Entities
{
	public class Monster
		: Entity
	{
		public static GameState State;

		public Vector2 MovementDirection;
		public Vector2 TargetDirection;
		public float MovementSpeed;
		public float TurnSpeed;
		public Player Target;
		public ElementalTypeEnum Element;
		public Vector2 Origin;

		public void CheckCollide(Player player)
		{
			var distance = Vector2.Distance(player.CollisionCenter, CollisionCenter);
			if(distance < player.CollisionRadius + CollisionRadius && OnCollisionWithPlayer != null)
				OnCollisionWithPlayer(this, new GenericEventArgs<Player>(player));

		}

		public override void Update(GameTime gameTime)
		{
			if (Target == null)
				return;

			if (Health <= 0 && OnDeath != null)
				OnDeath(this, null);

			// Calculate the best way to move and angle difference is high enough
			if (Vector2.DistanceSquared(Target.Location, Location + new Vector2(32, 32)) < 50)
				return;

			var destAngle = (float)Math.Atan2 (Target.Location.Y - Location.Y - 16, Target.Location.X - Location.X - 16);
			var minAngle1 = Math.Abs(Math.Min(Rotation - destAngle, destAngle - Rotation)) % MathHelper.TwoPi;
			var oppositeAngle = (Rotation + MathHelper.Pi) % MathHelper.TwoPi;
			var minAngle2 = Math.Abs(Math.Min(oppositeAngle - destAngle, destAngle - oppositeAngle)) % MathHelper.TwoPi;

			float newRotation = Rotation + ((minAngle1 < minAngle2) ? TurnSpeed : -TurnSpeed);
			//if (Math.Abs(minAngle1 - minAngle2) < MathHelper.PiOver4 / 4)
			//    newRotation = Rotation;
			if (minAngle1 < minAngle2 && newRotation > destAngle)
				newRotation = destAngle;
			else if (minAngle1 > minAngle2 && newRotation < destAngle)
				newRotation = destAngle;

			Rotation = (float)Math.Round(newRotation, 2);

			// Start moving
			var lookVector = new Vector2 ((float)Math.Cos (Rotation), (float)Math.Sin (Rotation));
			if (lookVector.Length() > 0)
				lookVector.Normalize();

			var moveVector = lookVector * MovementSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
			Location += moveVector;

			CollisionCenter = Location;
			Collision = new Rectangle((int)CollisionCenter.X, (int)CollisionCenter.Y, 1, 1);
			Collision.Inflate((int)CollisionRadius, (int)CollisionRadius);
		}

		public override void Draw(SpriteBatch batch)
		{
			batch.Draw(StatusBar.Pixel, Collision, Color.Red);
			batch.Draw(Texture, Location, Animations.Bounding, Color.White, Rotation - MathHelper.PiOver2, Origin, 1, SpriteEffects.None, 0);
		}

		public event EventHandler OnDeath;
		public event EventHandler<GenericEventArgs<Player>> OnCollisionWithPlayer;

	}

	public enum ElementalTypeEnum
	{
		Fire,
		Water,
		Ice,
		Earth,
		Wind
	}

}
