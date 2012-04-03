using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moxy.Entities;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Moxy.GameStates;
using Moxy.Skills;
using Microsoft.Xna.Framework.Input;

namespace Moxy.GameStates
{
	public class UIOverlay
		: BaseGameState
	{
		public UIOverlay()
			: base("UIOverlay", isOverlay:true, acceptsInput:false)
		{
		}

		public override void Load()
		{
			OwningState = (GameState)Moxy.StateManager["Game"];

			StatusBar.UI = this;
			StatusBar.Pixel = new Texture2D (Moxy.Graphics, 1, 1, false, SurfaceFormat.Color);
			StatusBar.Pixel.SetData (new [] { Color.White });

			getReadyTexture = Moxy.ContentManager.Load<Texture2D> ("getready");
			font = Moxy.ContentManager.Load<SpriteFont> ("spookyfont");
			scorefont = Moxy.ContentManager.Load<SpriteFont> ("scorefont");

			EnergyBar.UI = this;
			StatusBars = new List<StatusBar> ();
			ActivePlayers = new List<Player> ();
		}

		public override void Draw(SpriteBatch batch)
		{
			batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp,
				DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Matrix.Identity);

			if (OwningState.boss == null)
			{
				foreach (var bar in StatusBars)
					bar.Draw(batch);

				if (RedEnergyBar != null)
					RedEnergyBar.Draw(batch);
				if (RedRuneBar != null)
					RedRuneBar.Draw(batch);
				if (RedSkillBar != null)
					RedSkillBar.Draw(batch);

				batch.End();

				batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);

				if (OwningState.InbetweenRounds)
					batch.Draw(getReadyTexture, new Vector2(275, 175), new Color(1f, 1f, 1f, (float)Math.Abs(Math.Sin(sinX))));

				if (!OwningState.InbetweenRounds)
				{
					DateTime start = OwningState.StartLevelTime;
					DateTime endTime = start.Add(new TimeSpan(0, 0, 0, (int)OwningState.Level.WaveLength));

					var time = endTime.Subtract(DateTime.Now);
					batch.DrawString(font, string.Format("{0:##00}:{1:##00}", time.Minutes, time.Seconds), new Vector2(300, 30), Color.Purple);
					batch.DrawString (scorefont, "Wave " + (Moxy.CurrentLevelIndex + 1).ToString ("#") + "/6", new Vector2 (280, -5), Color.Purple);

					if (RedEnergyBar != null)
					{
						batch.DrawString(scorefont, "Score", new Vector2(10, 0), Color.Purple);
						batch.DrawString(scorefont, OwningState.Team1Score.ToString(), new Vector2(10, 40), Color.Purple);
					}

					if (BlueEnergyBar != null)
					{
						batch.DrawString(scorefont, "Score", new Vector2(630, 0), Color.Purple);
						batch.DrawString(scorefont, OwningState.Team2Score.ToString(), new Vector2(630, 40), Color.Purple);
					}
				}
			}

			batch.End();
		}

		public override void Update(GameTime gameTime)
		{
			if (RedEnergyBar != null)
				RedEnergyBar.Update(gameTime);

			if (RedSkillBar != null)
				RedSkillBar.Update(gameTime);


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
									Location = new Vector2(10, 160),
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
									Location = new Vector2(20, 220),
									Generator = (PowerGenerator)player
								};
							}
							if (RedSkillBar == null)
							{
								RedSkillBar = new SkillSelection()
								{
									Location = new Vector2(20, 200),
									Player = (PowerGenerator)player,
									UI = this
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

				sinX += (MathHelper.Pi / 3f) * (float)gameTime.ElapsedGameTime.TotalSeconds;
			}
		}

		public GameState OwningState;
		public List<StatusBar> StatusBars;
		public EnergyBar BlueEnergyBar;
		public RuneBar RedRuneBar;
		public SkillSelection RedSkillBar;
		public EnergyBar RedEnergyBar;
		public List<Player> ActivePlayers;
		private Texture2D getReadyTexture;
		private SpriteFont font;
		private SpriteFont scorefont;
		private float sinX;
	}

	public class SkillSelection
	{
		public static Rectangle BoxSize = new Rectangle(0, 0, 150, 150);
		public static Rectangle ItemSize = new Rectangle(0, 0, 50, 50);
		public static Texture2D SkillTextures = Moxy.ContentManager.Load<Texture2D>("icons");
		public static Color ActiveColor = Color.White;
		public static Color InActiveColor = new Color(128, 128, 128, 128);

		public static Rectangle[] Bounding = new Rectangle[] {
			new Rectangle(0, 50, 50, 50),
			new Rectangle(50, 0, 50, 50),
			new Rectangle(0, 0, 50, 50),
			new Rectangle(50, 50, 50, 50)};

		public static Rectangle[] DrawLocations = new Rectangle[] {
			new Rectangle(-25, -70, 50, 50), //Top
			new Rectangle(-75, -20, 50, 50), //Left
			new Rectangle(25, -20, 50, 50), //Right
			new Rectangle(-25, 30, 50, 50)}; //Bottom

		public PowerGenerator Player;
		public GeneratorSkill oldSkill;
		public UIOverlay UI;
		public TimeSpan fadeTime, maxTime = new TimeSpan(0, 0, 1);
		public Vector2 Location;

		public void Draw(SpriteBatch batch)
		{
			var currentIcon = new Rectangle(105, 170, 50, 50);
			batch.Draw(SkillTextures, currentIcon, Bounding[Player.Skills.IndexOf(Player.CurrentSkill)], InActiveColor);
			if (Player.SkillShown || fadeTime.TotalSeconds <= maxTime.TotalSeconds)
			{
				var screenPos = UI.OwningState.camera.WorldToScreen(Player.Location);
				var drawRectangle = new Rectangle((int)screenPos.X, (int)screenPos.Y, ItemSize.Width, ItemSize.Height);

				for (var i = 0; i < DrawLocations.Length; i++)
				{
					drawRectangle.X = DrawLocations[i].X + (int)screenPos.X;
					drawRectangle.Y = DrawLocations[i].Y + (int)screenPos.Y;
					batch.Draw(SkillTextures, drawRectangle, Bounding[i], Color.Lerp(((i == Player.SelectedSkill) ? ActiveColor : InActiveColor), Color.Transparent, (float)(fadeTime.TotalSeconds / maxTime.TotalSeconds)));
				}
			}
		}

		public void Update(GameTime gameTime)
		{
			if(Player.SkillShown)
			{
				fadeTime = TimeSpan.Zero;
			}
			else
			{
				fadeTime += gameTime.ElapsedGameTime;
			}
			
		}

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
		public static Color Inactive = Color.White;

		public static Texture2D GetRuneInSlot(PowerGenerator Generator, int slot)
		{
			switch (Generator.CurrentSkill.MatchArray[slot])
			{
				case ItemID.WaterRune:
					return WaterRune;
				case ItemID.FireRune:
					return FireRune;
				case ItemID.WindRune:
					return AirRune;
				case ItemID.EarthRune:
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
