﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Moxy
{
	public class Size
	{
		public Size()
		{
		}

		public Size(float width, float height)
		{
			this.width = width;
			this.height = height;
		}

		public float Width
		{
			get { return width; }
			set { width = value; }
		}

		public float Height
		{
			get { return height; }
			set { height = value; }
		}

		private float width;
		private float height;
	}
}
