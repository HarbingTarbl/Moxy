using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Moxy.GameStates
{
    public interface ITransition
    {
        event EventHandler OnTransitionStart;
        event EventHandler OnTransitionEnd;

        bool IsRunning { get; }
        bool HasStarted { get; }
		bool HasFinished { get; }

        void TriggerStart();
        void TriggerEnd();

        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch);
        void Cancel();
    }
}
