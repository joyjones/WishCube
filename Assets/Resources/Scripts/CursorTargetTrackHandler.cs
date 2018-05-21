using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WishCube;
using UnityEngine.UI;

public class CursorTargetTrackHandler : DefaultTrackableEventHandler
{

	protected override void Start()
	{
		base.Start ();
	}

	private void Update()
	{
        if (cursorUI != null)
        {
			var percent = CubeWorld.Instance.CursorTargetStayPercent;
			var image = cursorProgress.GetComponent<Image> ();
			image.fillAmount = percent;
			
            Vector3 screenPos;
            if (trackingLosted || cursorObj == null)
            {
                screenPos = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
            }
            else
            {
                screenPos = Camera.main.WorldToScreenPoint(cursorObj.transform.position);
                screenPos.z = 0;
            }
    		cursorUI.transform.position = screenPos;
            CubeWorld.Instance.ScreenPos = screenPos;
        }
	}

	protected override void OnTrackingFound()
	{
		base.OnTrackingFound();
        trackingLosted = false;
    }

	protected override void OnTrackingLost()
	{
		base.OnTrackingLost();
        trackingLosted = true;
	}

    private bool trackingLosted = true;
	public GameObject cursorObj;
	public GameObject cursorUI;
	public GameObject cursorProgress;
}
