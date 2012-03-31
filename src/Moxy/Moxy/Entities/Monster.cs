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

			// Calculate the best way to move
			var destAngle = (float)Math.Atan2 (Target.Location.Y - Location.Y, Target.Location.X - Location.X);
			var minAngle1 = Math.Abs (Math.Min (Rotation - destAngle, destAngle - Rotation));
			var oppositeAngle = (Rotation + MathHelper.Pi) % MathHelper.TwoPi;
			var minAngle2 = Math.Abs (Math.Min (oppositeAngle - destAngle, destAngle - oppositeAngle));

			float newRotation = Rotation + ((minAngle1 < minAngle2) ? TurnSpeed : -TurnSpeed);
			if (minAngle1 < minAngle2 && newRotation > destAngle)
				newRotation = destAngle;
			else if (minAngle1 > minAngle2 && newRotation < destAngle)
				newRotation = destAngle;

			Rotation = newRotation;

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
			batch.Draw(Texture, Location, Animations.Bounding, Color.White);
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
