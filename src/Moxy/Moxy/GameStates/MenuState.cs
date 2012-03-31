using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Moxy.GameStates
{
	public class MenuState
		: BaseGameState
	{
		public MenuState()
			: base("MenuState", isOverlay:false, acceptsInput:true)
		{

		
		}
	
		public override void Update(GameTime gameTime)
		{
 			throw new NotImplementedException();
		}

		public override void Draw (SpriteBatch batch)
		{
 			throw new NotImplementedException();
		}

		public override void Load()
		{
			textureTest = Moxy.ContentManager.Load<Texture2D>("SomeTexture");
		}

		Texture2D textureTest;
	}
}
