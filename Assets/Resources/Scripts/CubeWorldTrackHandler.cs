using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WishCube;

public class CubeWorldTrackHandler : DefaultTrackableEventHandler
{
	protected override void Start()
    {
		base.Start ();

        CubeWorld.Instance.InitForFaceMode(gameObject);
	}
	
	private void Update()
    {
		CubeWorld.Instance.Update ();

		if (Input.GetMouseButtonDown (0) || CubeWorld.Instance.HandCursor.IsFingerPicking) {
			var face = CubeWorld.Instance.FocusFace;
			var pos = CubeWorld.Instance.FocusPosition;
			if (face != null && pos.HasValue) {
				face.InsertBrick (pos.Value);
			}
		}
	}

    protected override void OnTrackingFound()
    {
        base.OnTrackingFound();
    }

    protected override void OnTrackingLost()
    {
        base.OnTrackingLost();
    }

	public int SliceCount
	{
		get { return CubeWorld.Instance.FaceSliceCount; }
		set { CubeWorld.Instance.FaceSliceCount = value; }
	}
}
