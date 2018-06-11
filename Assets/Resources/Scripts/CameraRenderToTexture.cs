using System;
using UnityEngine;

public class CameraRenderToTexture : MonoBehaviour
{
	void Start()
	{
	}

	void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		if (WishCube.CubeWorld.Instance.HandCursor.CVTracker.Enabled) {
			if (texture == null) {
				texture = new Texture2D (src.width, src.height, TextureFormat.ARGB32, false);
			}
			RenderTexture.active = src;
			texture.ReadPixels (new Rect (0, 0, src.width, src.height), 0, 0);
			texture.Apply ();
		}

		Graphics.Blit (src, dest);
	}

	private Texture2D texture;
}
