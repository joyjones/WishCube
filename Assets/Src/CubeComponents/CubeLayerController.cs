using System;
using System.Collections.Generic;
using UnityEngine;

namespace WishCube.CubeComponents
{
	public class CubeLayerController
	{
		public CubeLayerController (int sliceCount)
		{
			SliceCount = sliceCount;
		}

		public int SliceCount {
			get { return (int)Math.Pow (boxes.Count, 1 / 3.0); }
			private set {
				if (SliceCount != value && value > 1) {
					Initialize (value);
				}
			}
		}

		public EmbedBox GetBox(int x, int y, int z) {
			int index = z + y * SliceCount + x * SliceCount * SliceCount;
			if (index >= 0 && index < boxes.Count)
				return boxes [index];
			return null;
		}

		private void Initialize(int sliceCount) {
			layers.Clear ();
			boxes.Clear ();

			for (int x = 0; x < sliceCount; ++x) {
				for (int y = 0; y < sliceCount; ++y) {
					for (int z = 0; z < sliceCount; ++z) {
						var box = new EmbedBox (new Vector3(x, y, z));
						boxes.Add (box);
					}
				}
			}

			foreach (RubicLayer.AxisType t in Enum.GetValues(RubicLayer.AxisType)) {
				var ls = new List<RubicLayer> ();
				for (int i = 0; i < sliceCount; ++i) {
					var layer = new RubicLayer (t, i);
					var bs = new List<EmbedBox> ();
					for (int j = 0; j < sliceCount; ++j) {
						for (int k = 0; k < sliceCount; ++k) {
							if (t == RubicLayer.AxisType.X)
								bs.Add (GetBox (i, j, k));
							else if (t == RubicLayer.AxisType.Y)
								bs.Add (GetBox (j, i, k));
							else if (t == RubicLayer.AxisType.Z)
								bs.Add (GetBox (j, k, i));
						}
					}
					layer.Boxes = bs.ToArray ();
					ls.Add (layer);
				}
				layers [t] = ls.ToArray ();
			}
		}

		private Dictionary<RubicLayer.AxisType, RubicLayer[]> layers = new Dictionary<RubicLayer.AxisType, RubicLayer[]>();
		private List<EmbedBox> boxes = new List<EmbedBox>();
	}
}
