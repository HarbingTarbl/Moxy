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
		public ElementalTypeEnum Element;

		public override void Update(GameTime gameTime)
		{
			MovementDirection.X = (float)Math.Sin(Rotation);
			MovementDirection.Y = (float)Math.Cos(Rotation);


			Location += MovementSpeed * MovementDirection * (float)gameTime.ElapsedGameTime.TotalSeconds;
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
