using System;
using System.Collections.Generic;
using UnityEngine;
using WishCube.OpenCV;

namespace WishCube
{
	public class HandCursor
	{
		public HandCursor ()
		{
			CVTracker = new ArucoCursorTracker ();
		}

		public bool IsFingerPicking
		{
			get
			{
				bool clicking = isFingerClicking;
				isFingerClicking = false;
				return clicking;
			}
		}

		public ArucoCursorTracker CVTracker {
			get;
			private set;
		}

		public void Update()
		{
//			elapsedTime += Time.deltaTime;
			var pos = CVTracker.CursorPosition;
			timestamps.Add (Time.time);
			positions.Add (pos);

			bool clicking = ParseRecords ();
			if (clicking) {
				isFingerClicking = true;
				timestamps.Clear ();
				positions.Clear ();
			}
		}

		private bool ParseRecords()
		{
			if (positions.Count < 3)
				return false;
			float lastTime = timestamps[timestamps.Count - 1];
			int removeCount = 0;
			int i = 0;
			while (i < positions.Count) {
				var p = positions [i];
				float t = timestamps [i];
				if (lastTime - t <= MaxClickTimespan)
					break;
				++i;
				++removeCount;
			}
			if (removeCount > 0) {
				positions.RemoveRange (0, removeCount);
				timestamps.RemoveRange (0, removeCount);
			}

			i = positions.Count - 1;
			var pos2 = positions [i];
			var tick2 = timestamps [i];
			float stayEndTime = tick2;

			// hold position for more than 0.1 seconds
			while (--i >= 0) {
				var pos1 = positions [i];
				var tick1 = timestamps [i];
				if ((pos1 - pos2).magnitude > 2) {
					if (stayEndTime - tick1 <= 0.1f)
						return false;
					++i;
					break;
				}
				pos2 = pos1;
				tick2 = tick1;
			}
			if (i < 0)
				return false;

			// click upward for some ticks (pos y decrease)
			int ups = 0;
			float topy = pos2.y;
			while (--i >= 0) {
				var pos1 = positions [i];
				var tick1 = timestamps [i];
				if (pos1.y > pos2.y) {
					if (ups == 0)
						return false;
					++i;
					break;
				}
				if (Mathf.Abs(pos1.x - pos2.x) > 5)
					return false;
				++ups;
				pos2 = pos1;
				tick2 = tick1;
			}
			if (i < 0)
				return false;
			// click downward for some ticks (pos y increase)
			int downs = 0;
			float bottomy = pos2.y;
			if (Mathf.Abs (topy - bottomy) < 10)
				return false;
			while (--i >= 0) {
				var pos1 = positions [i];
				var tick1 = timestamps [i];
				if (pos1.y < pos2.y) {
					if (downs == 0)
						return false;
					++i;
					break;
				}
				if (Mathf.Abs(pos1.x - pos2.x) > 5)
					return false;
				++downs;
				pos2 = pos1;
				tick2 = tick1;
			}
			if (i < 0)
				i = 0;
			if (lastTime - timestamps[i] < 0.4f)
				return false;
			Debug.Log ("clicked!");
			return true;
		}

		public static readonly float MaxClickDist = 20;
		public static readonly float MaxClickTimespan = 1;
		public static readonly Rect ClickGestureRect = new Rect (0, 0, 5, 10);
		private List<Vector2> positions = new List<Vector2>();
		private List<float> timestamps = new List<float>();
		private bool isFingerClicking = false;
	}
}