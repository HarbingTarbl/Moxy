using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;

namespace Moxy.GameStates
{
	public class GameStateManager
	{
		public GameStateManager()
		{
			states = new List<BaseGameState>();
			stateMap = new Dictionary<string, BaseGameState>();
			markToPush = new Queue<BaseGameState>();
		}

		public BaseGameState CurrentState
		{
			get
			{
				if (states.Count <= 0)
					return null;
				
				return states[states.Count - 1]; 
			}
		}

		public BaseGameState this[string moduleName]
		{
			get
			{
				if (!stateMap.ContainsKey(moduleName))
					throw new ArgumentOutOfRangeException ("module does not exist");

				return stateMap[moduleName];
			}
		}

		public void Set (string name, ITransition transOut = null, ITransition transIn = null)
		{
			pushNextState = false;

			nextState = stateMap[name];
			TransitionToNextState (transOut, transIn);
		}

		public void Push (BaseGameState state, ITransition transOut = null, ITransition transIn = null)
		{
			pushNextState = true;

			pushBuffer = state;
			TransitionToNextState(transOut, transIn);
		}

		public void Push (string name, ITransition transOut = null, ITransition transIn = null)
		{
			pushNextState = true;

			pushBuffer = stateMap[name];
			TransitionToNextState (transOut, transIn);
		}
		
		public void Pop ()
		{
			markToPopCount++;

			if (markToPopCount > states.Count)
				throw new Exception ("There aren't enough states to pop.");
		}

		public void Update (GameTime gameTime)
		{
			// TODO: Find a better way to handle this
			latestInputReceiver = FindInputReceiver();
			if (latestInputReceiver != null && Mocha.IsActive)
				latestInputReceiver.HandleInput (gameTime);

			foreach (var state in states)
				state.Update (gameTime);
			
			if (currentTransition != null)
				UpdateTransition(gameTime);

			//  Pop all marked states, you can't do this during Update()
			if (markToPopCount > 0)
			{
				for (int i=0; i < markToPopCount; i++)
					PopState();

				markToPopCount = 0;
			}

			// Set state
			if (nextState != null)
			{
				foreach (var state in states)
					state.OnLostFocus ();

				states.Clear ();
				states.Add (nextState);
				nextState.OnFocus ();

				nextState = null;
			}

			// Push all new states on to the stack outside of Update()
			while (markToPush.Count > 0)
			{
				states.Add (markToPush.Dequeue ());
				CurrentState.OnFocus();
			}
		}

		public void Draw (SpriteBatch spriteBatch)
		{
			// Render backwards to test for occlusion
			foreach (BaseGameState state in states)
			{
				state.Draw (spriteBatch);
			}

			if (currentTransition != null)
				currentTransition.Draw (spriteBatch);
		}

		public void Load (params Assembly[] assemblies)
		{
			Logger.LogDebug ("Loading game states");
			LoadAssemblies (assemblies);
		}

		private List<BaseGameState> states;
		private Dictionary<string, BaseGameState> stateMap;
		private BaseGameState latestInputReceiver;
		private Queue<BaseGameState> markToPush;
		private int markToPopCount;
		
		// For transitioning
		private BaseGameState nextState;
		private BaseGameState pushBuffer;
		private ITransition currentTransition;
 		private ITransition nextTransition;
		private bool pushNextState;

		private void TransitionToNextState (ITransition transOut, ITransition transIn)
		{
			currentTransition = transOut;
			nextTransition = transIn;

			// No transition out
			if (transOut == null)
			{
				DoPushOrSet();
				currentTransition = transIn;
				nextTransition = null;
			}

			if (currentTransition != null)
				currentTransition.TriggerStart();
		}
		
		private void ResetTransitions()
		{
			currentTransition = null;
			nextTransition = null;
			nextState = null;
		}

		private void UpdateTransition (GameTime gameTime)
		{
			currentTransition.Update (gameTime);
			
			// Iterate the transitions and move the state if needed
			if (currentTransition.HasFinished)
			{
				// If there is a next state, let's transition to it
				if (nextState != null)
					DoPushOrSet();

				currentTransition = nextTransition;
			}

			// Are we done transitioning?
			bool finishedLastTransition = currentTransition == null && nextTransition == null;
			
			if (currentTransition == null || finishedLastTransition)
				ResetTransitions();
		}

		private void DoPushOrSet()
		{
			if (pushNextState)
				PushState();
			else
				SetState();
		}

		private void SetState()
		{
			
		}

		private void PushState()
		{
			// No states? set the state
			if (states.Count <= 0 && nextState == null)
			{
				nextState = pushBuffer;
				pushBuffer = null;

				SetState();
				return;
			}

			// Our new state is full screen, the previous states have lost focus
			if (!pushBuffer.IsOverlay)
			{
				foreach (var state in states)
					state.OnLostFocus();
			}
			
			markToPush.Enqueue (pushBuffer);
		}

		private void PopState()
		{
			bool needNewInputReceiver = states[states.Count - 1] == latestInputReceiver;

			states[states.Count-1].OnLostFocus();
			states.RemoveAt (states.Count-1);

			if (needNewInputReceiver)
				latestInputReceiver = FindInputReceiver ();
		}

		private BaseGameState FindInputReceiver()
		{
			// Find a new input receiver by iterating backwards through the states
			for (int i = states.Count - 1; i >= 0; i--)
			{
				if (states[i].AcceptsInput)
					return states[i];
			}

			return null;
		}

		private void LoadAssemblies(Assembly[] assemblies)
		{
			foreach (var assembly in assemblies)
			{
				var types = assembly.GetTypes ().Where (t => t.IsSubclassOf (typeof(BaseGameState)));

				foreach  (var type in types)
				{
					if (type.GetConstructor (Type.EmptyTypes) == null)
						continue;

					BaseGameState state = (BaseGameState)Activator.CreateInstance (type);
					stateMap.Add (state.Name, state);
				}
			}

			foreach (var state in stateMap.Values)
			{
				state.Load();
				Logger.LogInfo ("State " + state.Name + " loaded.");
			}

			foreach (var state in stateMap.Values)
				state.PostLoad();
		}
	}
}
