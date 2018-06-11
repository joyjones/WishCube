using System;
using UnityEngine;

namespace WishCube.CubeComponents
{
	public class EmbedBox
	{
		public EmbedBox (Vector3 pos)
		{
			Pos = pos;
		}

		public Vector3 Pos {
			get;
			set;
		}
	}
}
