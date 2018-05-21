using System;
using UnityEngine;

namespace WishCube
{
	public class CubeFaceGrid
	{
		public CubeFaceGrid (CubeFace face, int ix, int iy)
		{
			ParentFace = face;
			IndexX = ix;
			IndexY = iy;

		}

		public CubeFace ParentFace
		{
			get;
			private set;
		}

		public int IndexX
		{
			get; private set;
		}

		public int IndexY
		{
			get; private set;
		}

		public Vector2 LocalPosition
		{
			get
			{
				float x = (IndexX + 0.5f) * ParentFace.SliceSize;
				float y = (IndexY + 0.5f) * ParentFace.SliceSize;
				return new Vector2 (x, y);
			}
		}

		public Vector3 Position
		{
			get
			{
				var local = LocalPosition;
				return ParentFace.PlanePositionToWorld (local.x, local.y);
			}
		}
	}
}