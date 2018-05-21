using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace WishCube
{
    public class CubeWorld
    {
        private CubeWorld()
        {
			ScreenPos = new Vector2 (0, 0);
			Size = 0;
			CursorTargetStayPercent = 0;
			HandCursor = new HandCursor ();
        }

        public static CubeWorld Instance
        {
            get { return instance; }
        }

		public float Size
		{
			get;
			private set;
		}

        public Vector2 ScreenPos
        {
            get; set;
        }

		public Vector3? FocusPosition
		{
			get { return focusBox.activeSelf ? new Vector3?(focusBox.transform.position) : null; }
		}

		public CubeFace FocusFace
		{
			get;
			private set;
		}

		public Transform Transform
		{
			get { return cubeObj.transform; }
		}

		public float CursorTargetStayPercent
		{
			get;
			private set;
		}

		public HandCursor HandCursor
		{
			get;
			private set;
		}

		public void InitForFaceMode(GameObject obj)
		{
			cubeObj = obj;
			childrenRootObj = cubeObj.transform.Find ("ChildTargets").gameObject;
			uiRootObj = cubeObj.transform.Find ("UI").gameObject;

			float zoom = 1.1f;
			childrenRootObj.transform.localScale = new Vector3 (zoom, zoom, zoom);

			var mat = Resources.Load ("Materials/grass") as Material;

			faces.Clear();
			for (int i = 0; i < childrenRootObj.transform.childCount; ++i)
			{
				var faceObj = childrenRootObj.transform.GetChild(i).gameObject;
				if (Size == 0)
					Size = faceObj.transform.localScale.x * zoom;
				var side = (CubeFaceSide)Enum.Parse (typeof(CubeFaceSide), faceObj.name.Substring (faceObj.name.IndexOf (".") + 1));
				var face = new CubeFace(side, faceObj);
				var mob = faceObj.GetComponent<Vuforia.MaskOutBehaviour> ();
				mob.maskMaterial = mat;
				var renderer = faceObj.GetComponent<MeshRenderer> ();
				renderer.material = mat;
				faces[faceObj.name] = face;
			}

			focusBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
			focusBox.name = "FocusBox";
			GameObject.Destroy (focusBox.GetComponent<BoxCollider> ());
			focusBox.GetComponent<MeshRenderer> ().material = Resources.Load ("Materials/wireframeBold") as Material;

			FaceSliceCount = 10;
		}

		public void Update()
		{
			if (cubeObj == null)
				return;
			foreach (var f in faces.Values)
				f.Update ();
			HandCursor.Update ();

			bool focusShown = false;
			CubeFace face = null;
			GameObject menuItem = null;
			RaycastHit hit;
			var ray = Camera.main.ScreenPointToRay (ScreenPos);
			Debug.DrawLine (ray.origin, ray.origin + ray.direction * 100);
			if (Physics.Raycast (ray, out hit)) {
				var t = hit.transform;
				if (faces.TryGetValue(t.gameObject.name, out face))
				{
					var grid = face.PickGrid (hit.point);
					if (grid != null)
					{
						focusBox.transform.parent = t;
						focusBox.transform.position = grid.Position + face.Normal * face.SliceSize * 0.5f;
						focusShown = true;
						Debug.Log (string.Format ("hit: {0}, pos: {1},{2},{3}", t.gameObject.name, hit.point.x, hit.point.y, hit.point.z));
					}
				}
				else if (t.gameObject.name.StartsWith("MenuItem"))
				{
					menuItem = t.gameObject;
				}
			}

			FocusFace = face;
			focusBox.SetActive (focusShown);

			var rightBar = uiRootObj.transform.Find ("RightBar").transform;
			for (int i = 0; i < rightBar.childCount; ++i) {
				var item = rightBar.GetChild (i).gameObject;
				if (item == menuItem)
					item.GetComponentInChildren<TextMesh> ().color = Color.red;
				else
					item.GetComponentInChildren<TextMesh> ().color = Color.white;
			}
			if (focusMenuItem != menuItem)
				menuItemStayTime = menuItem == null ? float.MinValue : -1;
			focusMenuItem = menuItem;
			CursorTargetStayPercent = 0;
			if (menuItemStayTime != float.MinValue) {
				menuItemStayTime += Time.deltaTime;
				if (menuItemStayTime >= 0) {
					CursorTargetStayPercent = menuItemStayTime / 3.0f;
					if (CursorTargetStayPercent > 1)
						CursorTargetStayPercent = 1;
				}
			}
		}

		public CubeFace GetFace(CubeFaceSide side)
		{
			foreach (var face in faces.Values)
			{
				if (face.Side == side)
					return face;
			}
			return null;
		}

		public int FaceSliceCount
		{
			get { return sliceCount; }
			set
			{
				if (value > 0 && value != sliceCount)
				{
					sliceCount = value;
					foreach (var face in faces.Values)
					{
						face.SliceCount = sliceCount;
					}
					float sliceScale = Size / sliceCount;
					focusBox.transform.localScale = new Vector3(sliceScale, sliceScale, sliceScale);
				}
			}
		}

		private static CubeWorld instance = new CubeWorld();
		private GameObject cubeObj;
		private GameObject uiRootObj;
		private GameObject childrenRootObj;
		private GameObject focusBox;
		private GameObject focusMenuItem;
		private int sliceCount = 0;
		private float menuItemStayTime = float.MinValue;
		private Dictionary<string, CubeFace> faces = new Dictionary<string, CubeFace>();
    }
}