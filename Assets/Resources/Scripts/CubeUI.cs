using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WishCube;

public class CubeUI : MonoBehaviour
{
	// Use this for initialization
	void Start ()
	{
		baseLocation = transform.position;
		baseRotation = transform.rotation;
	}
	
	// Update is called once per frame
	void Update ()
	{
		Camera camera = Camera.main;
		float angle; Vector3 axis;
		camera.transform.rotation.ToAngleAxis (out angle, out axis);
		transform.position = baseLocation;
		transform.rotation = baseRotation;
		transform.RotateAround (CubeWorld.Instance.Transform.position, axis, angle);
	}

	private Vector3 baseLocation;
	private Quaternion baseRotation;
}
