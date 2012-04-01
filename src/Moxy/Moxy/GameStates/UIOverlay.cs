using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moxy.Entities;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Moxy.GameStates;

namespace Moxy.GameStates
{
	public class UIOverlay
		: BaseGameState
	{
		public UIOverlay(GameState OwningState)
			: base("UIOverlay", isOverlay:true, acceptsInput:false)
		{
			StatusBar.UI = this;
			StatusBar.Pixel = new Texture2D(Moxy.Graphics, 1, 1, false, SurfaceFormat.Color);
			StatusBar.Pixel.SetData<Color>(new Color[] { Color.White });

			EnergyBar.UI = this;
			StatusBars = new List<StatusBar>();
			ActivePlayers = new List<Player>();
			this.OwningState = OwningState;
		}


		public override void Draw(SpriteBatch batch)
		{
			batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp,
				DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Matrix.Identity);
			foreach (var bar in StatusBars)
			{
				bar.Draw(batch);
			}

			if (RedEnergyBar != null)
				RedEnergyBar.Draw(batch);
			if (RedRuneBar != null)
				RedRuneBar.Draw(batch);

			batch.End();
		}

		public override void Update(GameTime gameTime)
		{
			if (RedEnergyBar != null)
				RedEnergyBar.Update(gameTime);


			foreach (var player in ActivePlayers)
			{
				if (player.EntityType == EntityType.Gunner)
				{
					switch (player.Team)
					{
						case Team.Red:
							if (RedEnergyBar == null)
							{
								RedEnergyBar = new EnergyBar()
								{
									Player = (Gunner)player,
									Location = new Vector2(30, 60),
								};
							}
							break;
					}
				}
				else if (player.EntityType == EntityType.Generator)
				{
					switch (player.Team)
					{
						case Team.Red:
							if (RedRuneBar == null)
							{
								RedRuneBar = new RuneBar()
								{
									Location = new Vector2(30, 100),
									Generator = (PowerGenerator)player
								};
							}
							break;
					}
				}


				if (!StatusBars.Any(bar => bar.Player == player))
				{
					var bar = new StatusBar();
					bar.Player = player;
					StatusBars.Add(bar);
				}
			}
		}

		public GameState OwningState;
		public List<StatusBar> StatusBars;
		public EnergyBar BlueEnergyBar;
		public RuneBar RedRuneBar;
		public EnergyBar RedEnergyBar;
		public List<Player> ActivePlayers;
	}


	public class RuneBar
	{
		public static Rectangle BarSize = new Rectangle(0, 0, 100, 440);
		public static Rectangle RuneSize = new Rectangle(0, 0, 100, 50);
		public static Texture2D FireRune = Moxy.ContentManager.Load<Texture2D>("FireRune");
		public static Texture2D EarthRune = Moxy.ContentManager.Load<Texture2D>("EarthRune");
		public static Texture2D WaterRune = Moxy.ContentManager.Load<Texture2D>("WaterRune");
		public static Texture2D AirRune = Moxy.ContentManager.Load<Texture2D>("AirRune");
		public static Color Active = Color.Transparent;
		public static Color Inactive = Color.Red;

		public static Texture2D GetRuneInSlot(PowerGenerator Generator, int slot)
		{
			switch (Generator.CurrentSkill.MatchArray[slot])
			{
				case ItemID.BluePowerup:
					return WaterRune;
				case ItemID.RedPowerup:
					return FireRune;
				case ItemID.YellowPowerup:
					return AirRune;
				case ItemID.GreenPowerup:
					return EarthRune;
			}

			return null;

		}

		public RuneBar()
		{
		}



		public PowerGenerator Generator;
		public Vector2 Location;

		public void Draw(SpriteBatch batch)
		{
			Texture2D[] runes = new Texture2D[4];
			Rectangle drawRectangle = new Rectangle(EnergyBar.BarSize.Width + (int)Location.X, (int)Location.Y, RuneSize.Width, RuneSize.Height);
			for (var i = 0; i < runes.Length; i++)
			{
				runes[i] = GetRuneInSlot(Generator, i);
				batch.Draw(runes[i], drawRectangle,
					(Generator.CurrentRunes[i] == Generator.CurrentSkill.MatchArray[i]) ? Active : Inactive);
				drawRectangle.Y += RuneSize.Height;
			}
		}


	}

	public class EnergyBar
	{
		public static Rectangle BarSize = new Rectangle(0, 0, 64, 256);
		public static Rectangle BarFrame = new Rectangle(0, 2, 31, 158);
		public static Texture2D Texture = Moxy.ContentManager.Load<Texture2D>("powerbarspritesheet");
		public static UIOverlay UI;

		public Gunner Player;
		public Vector2 Location;

		public AnimationManager BubbleAnimation;

		public EnergyBar()
		{
			BubbleAnimation = new AnimationManager(Texture, new Animation[]
			{
				new Animation("Bubbles!", new Rectangle[]
				{
					new Rectangle(36, 14, 24, 143),
					new Rectangle(100, 14, 24, 143),
					new Rectangle(68, 14, 24, 143),
					
				}, new TimeSpan(0, 0, 0, 0, 300))
			});
			BubbleAnimation.SetAnimation("Bubbles!");
		}


		public void Update(GameTime gameTime)
		{
			BubbleAnimation.Update(gameTime);
			
		}

		public void Draw(SpriteBatch batch)
		{
			var outer = new Rectangle((int)Location.X, (int)Location.Y, BarSize.Width, BarSize.Height);
			var height = Math.Min(BarSize.Height, BarSize.Height * Player.Energy / Player.MaxEnergy);
			var inner = new Rectangle((int)Location.X + 1, (int)(Location.Y + (BarSize.Height - height)), BarSize.Width - 2 , ((int)height));
			var innerBound = BubbleAnimation.Bounding;
			innerBound.Height = (int)(inner.Height * (143f /BarSize.Height) - 5);
			batch.Draw(Texture, inner, innerBound, Color.White);
			batch.Draw(Texture, outer, BarFrame, Color.White);

		}


	}

	public class StatusBar
	{
		public static Rectangle BarSize = new Rectangle(0, 0, 90, 20);
		public static Texture2D Pixel;

		public Player Player;
		public Vector2 Location;
		public static UIOverlay UI;

		public void Draw(SpriteBatch batch)
		{
			Location = UI.OwningState.camera.WorldToScreen(Player.Location);
			Location.X -= 40;
			Location.Y -= 40;
			var outerRect = new Rectangle((int)Location.X, (int)Location.Y, BarSize.Width, BarSize.Height);
			var innerRect = new Rectangle((int)Location.X + 3, (int)Location.Y + 3, (int)((BarSize.Width - 6) * (Player.Health / Player.MaxHealth)), BarSize.Height - 6);
			batch.Draw(Pixel, outerRect, Color.DarkGray);
			batch.Draw(Pixel, innerRect, Color.PaleVioletRed);
		}
	}
}
