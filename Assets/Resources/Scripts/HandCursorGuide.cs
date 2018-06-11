using System;
using UnityEngine;
using UnityEngine.UI;

public class HandCursorGuide : MonoBehaviour
{
	void Start() {
		guidePanel.SetActive (false);
		GetComponent<Button> ().onClick.AddListener (StartGuide);
	}

	void Update() {
		if (dispearRestTime > 0) {
			dispearRestTime -= Time.deltaTime;
			if (dispearRestTime < 0) {
				dispearRestTime = 0;
				guidePanel.SetActive (false);
				GetComponent<Button> ().enabled = true;
				GetComponentInChildren<Text> ().text = "开启指套";
			}
		}
	}

	void StartGuide() {
		GetComponent<Button> ().enabled = false;
		guidePanel.SetActive(true);
		GetComponentInChildren<Text> ().text = "请将指套靠近屏幕…";

		dispearRestTime = 60;
	}

	public GameObject guidePanel;
	private float dispearRestTime = 0;
}
