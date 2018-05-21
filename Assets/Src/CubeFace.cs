using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace WishCube
{
    public class CubeFace
    {
		public CubeFace(CubeFaceSide side, GameObject obj)
        {
            Side = side;
			if (obj.GetComponent<MeshCollider> () == null)
				obj.AddComponent<MeshCollider> ();

			var scale = obj.transform.localScale;
			obj.transform.localScale = new Vector3(scale.x, scale.x, scale.x);
			BindedObj = obj;
        }

		public CubeFaceSide Side
        {
            get; private set;
        }

		public GameObject BindedObj
		{
			get; private set;
		}

		public int SliceCount
		{
			get { return sliceCount; }
			set
			{
				if (value < 0 || value == sliceCount)
					return;
				grids.Clear ();
				for (int i = 0; i < value * value; ++i)
				{
					int ix = i % value;
					int iy = i / value;
					var grid = new CubeFaceGrid (this, ix, iy);
					grids.Add (grid);
				}
				sliceCount = value;
			}
		}

		public float SliceSize
		{
			get { return sliceCount == 0 ? 0 : CubeWorld.Instance.Size / sliceCount; }
		}

		public Vector3 Normal
		{
			get { return BindedObj.transform.up; }
		}

		public void Update()
		{
		}

		public CubeFaceGrid PickGrid(Vector3 worldPos)
		{
			var pos = WorldPositionToPlane (worldPos);
			int ix = (int)(pos.x / SliceSize);
			int iy = (int)(pos.y / SliceSize);
			if (ix < 0 || ix >= SliceCount)
				return null;
			if (iy < 0 || iy >= SliceCount)
				return null;
			return grids [iy * SliceCount + ix];
		}

		public Vector2 WorldPositionToPlane(Vector3 point)
		{
			point = CubeWorld.Instance.Transform.localToWorldMatrix.inverse.MultiplyPoint(point);
			float radius = CubeWorld.Instance.Size * 0.5f;
			switch (Side)
			{
				case CubeFaceSide.Left:
					return new Vector2 (radius - point.z, radius - point.y);
				case CubeFaceSide.Right:
					return new Vector2 (radius + point.z, radius - point.y);
				case CubeFaceSide.Front:
					return new Vector2 (radius + point.x, radius - point.z);
				case CubeFaceSide.Back:
					return new Vector2 (radius - point.x, radius - point.z);
				case CubeFaceSide.Top:
					return new Vector2 (radius - point.x, radius - point.y);
				case CubeFaceSide.Bottom:
					return new Vector2 (radius + point.x, radius - point.y);
			}
			return Vector2.zero;
		}

		public Vector3 PlanePositionToWorld(float x, float y)
		{
			float radius = CubeWorld.Instance.Size * 0.5f;
			x -= radius; y -= radius;
			Vector3 pos = Vector3.zero;
			switch (Side)
			{
				case CubeFaceSide.Left:
					pos = new Vector3 (-radius, -y, -x);
					break;
				case CubeFaceSide.Right:
					pos = new Vector3 (radius, -y, x);
					break;
				case CubeFaceSide.Front:
					pos = new Vector3 (x, radius, -y);
					break;
				case CubeFaceSide.Back:
					pos = new Vector3 (-x, -radius, -y);
					break;
				case CubeFaceSide.Top:
					pos = new Vector3 (-x, -y, radius);
					break;
				case CubeFaceSide.Bottom:
					pos = new Vector3 (x, -y, -radius);
					break;
			}
			pos = CubeWorld.Instance.Transform.localToWorldMatrix.MultiplyPoint(pos);
			return pos;
		}

		public void InsertBrick(Vector3 pos)
		{
			var box = GameObject.CreatePrimitive(PrimitiveType.Cube);
			box.name = "Brick";
			box.GetComponent<MeshRenderer> ().material = Resources.Load ("Materials/brick") as Material;
			box.transform.parent = BindedObj.transform;
			float sliceScale = 1.0f / sliceCount;
			box.transform.localScale = new Vector3(sliceScale, sliceScale, sliceScale);
			box.transform.position = pos;
			box.transform.rotation = BindedObj.transform.rotation;
			subObjects.Add (box);
		}

		private List<GameObject> subObjects = new List<GameObject>();
		private int sliceCount = 0;
		private List<CubeFaceGrid> grids = new List<CubeFaceGrid>();
    }
}