using System;
using System.Linq;
using System.Collections.Generic;

namespace WishCube.CubeComponents
{
	public class RubicLayer
	{
		public RubicLayer (AxisType at, int index)
		{
			Axis = at;
			Index = index;
		}

		public enum AxisType
		{
			X, Y, Z
		}

		public AxisType Axis {
			get;
			private set;
		}

		public int Index {
			get;
			private set;
		}

		public EmbedBox[] Boxes {
			get { return boxes.ToArray (); }
			set {
				boxes = value.ToList ();
			}
		}

		private List<EmbedBox> boxes = new List<EmbedBox>();
	}
}
