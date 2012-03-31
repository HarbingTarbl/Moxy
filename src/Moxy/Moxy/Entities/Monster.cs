using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moxy.GameStates;

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

		public override void Update(GameTime gameTime)
		{
			if (Target == null)
				return;

			// Calculate the best way to move and angle difference is high enough
			if (Vector2.DistanceSquared(Target.Location, Location + new Vector2(16, 16)) < 10)
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

			//if (Dudes[0].Location.Y - monster.Location.Y < 0 || Dudes[0].Location.X - monster.Location.X < 0)
				//monster.Rotation = -monster.Rotation;
		}

		public override void Draw(SpriteBatch batch)
		{
			batch.Draw(Texture, Location, Animations.Bounding, Color.White, Rotation - MathHelper.PiOver2, new Vector2(32, 32), 1, SpriteEffects.None, 0);
		}

		public EventHandler OnDeath;
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
