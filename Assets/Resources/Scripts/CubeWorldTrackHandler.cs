using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WishCube;
using Vuforia;

public class CubeWorldTrackHandler : DefaultTrackableEventHandler
{
	void Awake()
	{
		Application.targetFrameRate = 100;
		var dsload = VuforiaConfiguration.Instance.DatabaseLoad;
		Debug.LogFormat ("ds act {0}: {1}", dsload.DataSetsToActivate.Length, string.Join(",", dsload.DataSetsToActivate));
		Debug.LogFormat ("ds load {0}: {1}", dsload.DataSetsToLoad.Length, string.Join(",", dsload.DataSetsToLoad));
//		VuforiaConfiguration.Instance.DatabaseLoad.DataSetsToActivate = new string[]{};

//		CameraDevice.Instance.SetFocusMode (CameraDevice.FocusMode.FOCUS_MODE_TRIGGERAUTO);
	}

	protected override void Start()
    {
		base.Start ();

        CubeWorld.Instance.InitForFaceMode(gameObject);

		lastUpdateShowTime = Time.realtimeSinceStartup;
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

		frames++;
		if (Time.realtimeSinceStartup - lastUpdateShowTime >= updateShowDeltaTime)
		{
			fps = frames / (Time.realtimeSinceStartup - lastUpdateShowTime);
			frames = 0;
			lastUpdateShowTime = Time.realtimeSinceStartup;
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

	void OnGUI()
	{
		GUI.Label(new Rect(Screen.width / 2, 0, 100, 100), "FPS: " + fps);
	}

	public int SliceCount
	{
		get { return CubeWorld.Instance.FaceSliceCount; }
		set { CubeWorld.Instance.FaceSliceCount = value; }
	}

	private float lastUpdateShowTime = 0f;  //上一次更新帧率的时间;
	private float updateShowDeltaTime = 0.1f;//更新帧率的时间间隔;
	private int frames = 0;//帧数
	private float fps = 0;
}
