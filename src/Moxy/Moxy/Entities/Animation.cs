using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace Moxy.Entities
{

	public class Animation
	{
		private static readonly TimeSpan DefaultTimeSpan = new TimeSpan(0, 0, 0, 0, 500);

		public readonly TimeSpan FrameRate;
		public readonly string Name;
		public readonly Rectangle[] Sources;
		public int CurrentSource;

		public Animation(string name, IEnumerable<Rectangle> sources, TimeSpan frameRate = default(TimeSpan))
		{
			Name = name;
			Sources = sources.ToArray();
			FrameRate = frameRate == default(TimeSpan) ? DefaultTimeSpan : frameRate;
		}
	}

	public class AnimationManager
	{
		public AnimationManager(Texture2D texture2D, Rectangle lRectangle, IEnumerable<Animation> animationsList = null)
		{
			animations = animationsList != null ? new Dictionary<string, Animation>(animationsList.ToDictionary((ani) => ani.Name)) : new Dictionary<string, Animation>();
		}

		public AnimationManager(Texture2D texture2D, IEnumerable<Animation> animationsList = null)
		{
			animations = animationsList != null ? new Dictionary<string, Animation>(animationsList.ToDictionary((ani) => ani.Name)) : new Dictionary<string, Animation>();

		}

		public void AddAnimation(Animation animation)
		{
			animations[animation.Name] = animation;
		}

		public bool SetAnimation(string Name)
		{
			Animation animation;
			if (!animations.TryGetValue(Name, out animation))
				return false;

			currentAnimation = animation;
			currentAnimation.CurrentSource = 0;
			Bounding = currentAnimation.Sources[0];
			return true;
		}

		public void Update(GameTime gameTime)
		{
			if (currentAnimation != null)
				playAnimation(gameTime);
		}

		private void playAnimation(GameTime gameTime)
		{
			lastUpdate += gameTime.ElapsedGameTime;
			if (lastUpdate < currentAnimation.FrameRate)
				return;

			currentAnimation.CurrentSource = ((currentAnimation.CurrentSource + 1) % currentAnimation.Sources.Length);
			Bounding = currentAnimation.Sources[currentAnimation.CurrentSource];
			lastUpdate = TimeSpan.Zero;
		}

		public Rectangle Bounding;

		private TimeSpan lastUpdate = TimeSpan.Zero;
		private Animation currentAnimation;
		private readonly Dictionary<string, Animation> animations;
	}
}
