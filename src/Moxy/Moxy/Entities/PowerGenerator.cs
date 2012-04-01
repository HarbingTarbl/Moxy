using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Moxy.Skills;
using Microsoft.Xna.Framework.Input;

namespace Moxy.Entities
{
	public class PowerGenerator
		: Player
	{
		public PowerGenerator()
		{
			Texture = Moxy.ContentManager.Load<Texture2D>("Team1SpriteSheet");
			Animations = new AnimationManager(Texture, 
				new Animation[] 
				{
					new Animation("Idle_1", new Rectangle[]
					{
						new Rectangle(0, 320, 64, 64),
					}),
					new Animation("Idle_2", new Rectangle[]
					{
						new Rectangle(0, 384, 64,64 ),
					}),
					new Animation("Idle_3", new Rectangle[]
					{
						new Rectangle(0, 448, 64, 64),
					}),
					new Animation("Walk_1", new Rectangle[] 
					{
						new Rectangle(0, 320, 64, 64),
						new Rectangle(64, 320, 64, 64),
						new Rectangle(128, 320, 64, 64),
					}, new TimeSpan(0, 0, 0, 0, 200)),
					new Animation("Walk_2", new Rectangle[] 
					{
						new Rectangle(0, 384, 64, 64),
						new Rectangle(64, 384, 64, 64),
						new Rectangle(128, 384, 64, 64),
					}, new TimeSpan(0, 0, 0, 0, 200)),
					new Animation("Walk_3", new Rectangle[] 
					{
						new Rectangle(0, 448, 64, 64),
						new Rectangle(64, 448, 64, 64),
						new Rectangle(128, 448, 64, 64),
					}, new TimeSpan(0, 0, 0, 0, 200))

				});

			Animations.SetAnimation("Idle_1");
			EntityType = global::Moxy.EntityType.Generator;
			Health = 100;
			CurrentRunes = new ItemID[4];
			Skills = new List<GeneratorSkill>();
			Skills.Add(new PowerSKill(this));
			Skills.Add(new ProtectionSkill(this));
			Skills.Add(new TriShotSkill(this));
			Skills.Add(new RageSkill(this));
			CurrentSkill = Skills[3];
			ActiveSkills = new List<SkillEffect>();
			CurrentItem = 0;
		}

		
		public Gunner Gunner;
		public ItemID[] CurrentRunes;
		public List<SkillEffect> ActiveSkills;
		public List<GeneratorSkill> Skills;
		public GeneratorSkill CurrentSkill;
		public int CurrentItem;
		public float ParticleDelay;
		public float ParticleTimePassed;
		public bool PowerDisabled;

		public void ApplyPowerup(Item item)
		{
			if (item.IsPowerup == false)
			{
				if (item.ItemID == CurrentSkill.MatchArray[CurrentItem] && item.Enabled)
				{
					item.Enabled = false;
					CurrentRunes[CurrentItem] = item.ItemID;
					CurrentItem++;
					CurrentItem %= 4;
					Console.WriteLine("Picked up an item {0}", Enum.GetName(typeof(ItemID), item.ItemID));
				}
			}
			//else

		}

		public void SetCurrentSkill(GeneratorSkill NewSkill)
		{
			CurrentSkill = NewSkill;
			for (var x = 0; x < CurrentRunes.Length; x++)
				CurrentRunes[x] = ItemID.None;
			CurrentItem = 0;

		}

		public void HandleInput(GameTime gameTime)
		{
			var gamePad = GamePad.GetState(this.PadIndex);

			if (gamePad.IsButtonDown(Buttons.A))
				CurrentSkill.Activate(this.CurrentRunes);


			if (gamePad.DPad.Left == ButtonState.Pressed)
				SetCurrentSkill(Skills[1]);
			if (gamePad.DPad.Up == ButtonState.Pressed)
				SetCurrentSkill(Skills[0]);
			if (gamePad.DPad.Down == ButtonState.Pressed)
				SetCurrentSkill(Skills[3]);
			if (gamePad.DPad.Right == ButtonState.Pressed)
				SetCurrentSkill(Skills[2]);

		}

		public override void Draw(SpriteBatch batch)
		{
			batch.Draw(Texture, new Rectangle((int)Location.X, (int)Location.Y, 64, 64), Animations.Bounding,
				Color, Rotation - MathHelper.PiOver2, new Vector2(32, 32), SpriteEffects.None, 0);

			for (var x = 0; x < ActiveSkills.Count; x++)
			{
				ActiveSkills[x].Draw(batch);
			}

		}

		public override void Update(GameTime gameTime)
		{
			Health = Math.Min(Gunner.Health, Health);
			HandleInput(gameTime);
			for (var x = 0; x < ActiveSkills.Count; x++)
			{
				ActiveSkills[x].Update(gameTime);
				if (!ActiveSkills[x].Active)
					ActiveSkills.RemoveAt(x--);
			}

			Animations.Update(gameTime);
			base.Update(gameTime);
			oldPosition = Location;
		}

		private Vector2 oldPosition;
	}
}
