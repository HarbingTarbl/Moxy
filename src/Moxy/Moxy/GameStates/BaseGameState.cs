using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Moxy.GameStates
{
	public abstract class BaseGameState
	{
		public BaseGameState (string name, bool isOverlay = false, bool acceptsInput = true)
		{
			this.Name = name;
			this.isOverlay = isOverlay;
			this.acceptsInput = acceptsInput;
		}

		public readonly string Name;
		public abstract void Update(GameTime gameTime);
		public abstract void Draw(SpriteBatch batch);

		public bool IsOverlay
		{
			get { return isOverlay; }
			protected set { isOverlay = value; }
		}

		public bool AcceptsInput
		{
			get { return acceptsInput; }
			protected set { acceptsInput = value; }
		}

		public virtual void Load()
		{
		}

		public virtual void PostLoad()
		{
		}

		public virtual void HandleInput(GameTime gameTime)
		{
		}

		public virtual void OnFocus()
		{
		}

		public virtual void OnLostFocus()
		{
		}

		private bool isOverlay;
		private bool acceptsInput;
	}
}
