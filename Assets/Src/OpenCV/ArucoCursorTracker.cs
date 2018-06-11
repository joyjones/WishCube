using System;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using OpenCVForUnity;

namespace WishCube.OpenCV
{
	public class ArucoCursorTracker
	{
		public ArucoCursorTracker ()
		{
			VuforiaARController.Instance.RegisterTrackablesUpdatedCallback (OnTrackablesUpdated);

			dictionary = Aruco.getPredefinedDictionary (Aruco.DICT_6X6_250);
			detectorParams = DetectorParameters.create ();
			Enabled = true;
		}

		public Vector2 MarkerCenterPos {
			get;
			private set;
		}
		public Vector2 CursorPosition {
			get;
			private set;
		}
		public bool Enabled {
			get;
			set;
		}

		private void OnTrackablesUpdated ()
		{
			if (!Enabled)
				return;
			CameraDevice cam = CameraDevice.Instance;
			if (!formatRegistered) {
				cam.SetFrameFormat (pixelFormat, true);
				formatRegistered = true;
			}

			Image image = cam.GetCameraImage (pixelFormat);
			if (image == null)
				return;

			if (inputMat == null) {
				inputMat = new Mat (image.Height, image.Width, CvType.CV_8UC1);

				float imageSizeScale = 1.0f;
				float widthScale = (float)Screen.width / image.Width;
				float heightScale = (float)Screen.height / image.Height;
				if (widthScale < heightScale) {
					Camera.main.orthographicSize = (image.Width * (float)Screen.height / (float)Screen.width) / 2;
					imageSizeScale = (float)Screen.height / (float)Screen.width;
				} else {
					Camera.main.orthographicSize = image.Height / 2;
				}

				int max_d = (int)Mathf.Max (image.Width, image.Height);
				double fx = max_d;
				double fy = max_d;
				double cx = image.Width / 2.0f;
				double cy = image.Height / 2.0f;
				camMatrix = new Mat (3, 3, CvType.CV_64FC1);
				camMatrix.put (0, 0, fx);
				camMatrix.put (0, 1, 0);
				camMatrix.put (0, 2, cx);
				camMatrix.put (1, 0, 0);
				camMatrix.put (1, 1, fy);
				camMatrix.put (1, 2, cy);
				camMatrix.put (2, 0, 0);
				camMatrix.put (2, 1, 0);
				camMatrix.put (2, 2, 1.0f);
				Debug.Log("camMatrix:" + camMatrix.dump ());
				Debug.Log(string.Format("camImage:w={0},h={1}; screen: w={2},h={3}; inputMat: w={4},h={5}", image.Width, image.Height, Screen.width, Screen.height, inputMat.width(), inputMat.height()));

				distCoeffs = new MatOfDouble (0, 0, 0, 0);

				// calibration camera.
				Size imageSize = new Size (image.Width * imageSizeScale, image.Height * imageSizeScale);
				double apertureWidth = 0;
				double apertureHeight = 0;
				double[] fovx = new double[1];
				double[] fovy = new double[1];
				double[] focalLength = new double[1];
				Point principalPoint = new Point (0, 0);
				double[] aspectratio = new double[1];

				Calib3d.calibrationMatrixValues (camMatrix, imageSize, apertureWidth, apertureHeight, fovx, fovy, focalLength, principalPoint, aspectratio);

				Debug.Log ("imageSize " + imageSize.ToString ());
				Debug.Log ("apertureWidth " + apertureWidth);
				Debug.Log ("apertureHeight " + apertureHeight);
				Debug.Log ("fovx " + fovx [0]);
				Debug.Log ("fovy " + fovy [0]);
				Debug.Log ("focalLength " + focalLength [0]);
				Debug.Log ("principalPoint " + principalPoint.ToString ());
				Debug.Log ("aspectratio " + aspectratio [0]);

				// To convert the difference of the FOV value of the OpenCV and Unity. 
				double fovXScale = (2.0 * Mathf.Atan ((float)(imageSize.width / (2.0 * fx)))) / (Mathf.Atan2 ((float)cx, (float)fx) + Mathf.Atan2 ((float)(imageSize.width - cx), (float)fx));
				double fovYScale = (2.0 * Mathf.Atan ((float)(imageSize.height / (2.0 * fy)))) / (Mathf.Atan2 ((float)cy, (float)fy) + Mathf.Atan2 ((float)(imageSize.height - cy), (float)fy));

				Debug.Log ("fovXScale " + fovXScale);
				Debug.Log ("fovYScale " + fovYScale);

				// Adjust Unity Camera FOV https://github.com/opencv/opencv/commit/8ed1945ccd52501f5ab22bdec6aa1f91f1e2cfd4
				if (widthScale < heightScale) {
					Camera.main.fieldOfView = (float)(fovx [0] * fovXScale);
				} else {
					Camera.main.fieldOfView = (float)(fovy [0] * fovYScale);
				}
			}

			inputMat.put (0, 0, image.Pixels);

			//            Imgproc.putText (inputMat, "CameraImageToMatSample " + inputMat.cols () + "x" + inputMat.rows (), new Point (5, inputMat.rows () - 5), Core.FONT_HERSHEY_PLAIN, 1.0, new Scalar (255, 0, 0, 255));

//			if (outputTexture == null) {
//				outputTexture = new Texture2D (inputMat.cols (), inputMat.rows (), TextureFormat.RGBA32, false);
//			}

			Aruco.detectMarkers (inputMat, dictionary, corners, ids, detectorParams, rejected, camMatrix, distCoeffs);
			if (ids.total () > 0) {
				var c = corners [0];
				//debugInfo1 = c.dump ();
				float tcx = 0, tcy = 0;
				for (int i = 0; i < 4; ++i) {
					var v = c.get (0, i);
					tcx += (float)v [0];
					tcy += (float)v [1];
				}

				MarkerCenterPos = new Vector2(tcx / 4.0f - image.Width * 0.5f, image.Height * 0.5f - tcy / 4.0f);
//				Debug.LogFormat ("cursor center: {0}, {1}", MarkerCenterPos.x, MarkerCenterPos.y);
				if (testObject != null)
					testObject.transform.position = new Vector3(MarkerCenterPos.x, MarkerCenterPos.y, 0);

				UpdateCursorPos (MarkerCenterPos.x, MarkerCenterPos.y);
			}

			//		Utils.matToTexture2D (inputMat, outputTexture);
			//
			//		quad.transform.localScale = new Vector3 ((float)image.Width, (float)image.Height, 1.0f);
			//		quad.GetComponent<Renderer> ().material.mainTexture = outputTexture;
			//
			//		mainCamera.orthographicSize = image.Height / 2;
		}

		private void UpdateCursorPos(float tx, float ty)
		{
			float sw = Screen.width;
			float sh = Screen.height;
			float iw = inputMat.width ();
			float ih = inputMat.height ();
			float aspectScrn = sw / sh;
			float aspectTex = iw / ih;
			float scale = 1;
			if (aspectTex > aspectScrn)
				scale = sh / ih;
			else
				scale = sw / iw;

			float x = sw * 0.5f + tx * scale;
			float y = sh * 0.5f + ty * scale;

			CursorPosition = new Vector2 (x, y);
		}

		private void EstimatePose() {

			Aruco.estimatePoseSingleMarkers (corners, markerLength, camMatrix, distCoeffs, rvecs, tvecs);

			Aruco.drawDetectedMarkers (inputMat, corners, ids, new Scalar (255, 0, 0));

			for (int i = 0; i < ids.total (); i++) {
				Aruco.drawAxis (inputMat, camMatrix, distCoeffs, rvecs, tvecs, markerLength * 0.5f);

				// position
				double[] tvec = tvecs.get (i, 0);
				// rotation
				double[] rv = rvecs.get (i, 0);
				Mat rvec = new Mat (3, 1, CvType.CV_64FC1);
				rvec.put (0, 0, rv [0]);
				rvec.put (1, 0, rv [1]);
				rvec.put (2, 0, rv [2]);
				Calib3d.Rodrigues (rvec, rotMat);

				transM.SetRow (0, new Vector4 ((float)rotMat.get (0, 0) [0], (float)rotMat.get (0, 1) [0], (float)rotMat.get (0, 2) [0], (float)tvec [0]));
				transM.SetRow (1, new Vector4 ((float)rotMat.get (1, 0) [0], (float)rotMat.get (1, 1) [0], (float)rotMat.get (1, 2) [0], (float)tvec [1]));
				transM.SetRow (2, new Vector4 ((float)rotMat.get (2, 0) [0], (float)rotMat.get (2, 1) [0], (float)rotMat.get (2, 2) [0], (float)tvec [2]));
				transM.SetRow (3, new Vector4 (0, 0, 0, 1));

//				// right-handed coordinates system (OpenCV) to left-handed one (Unity)
//				var ARM = invertYM * transM;
//				// Apply Z axis inverted matrix.
//				ARM = ARM * invertZM;
//				ARM = mainCamera.transform.localToWorldMatrix * ARM;
//
//				ARUtils.SetTransformFromMatrix (testObject.transform, ref ARM);
			}
		}

		private Image.PIXEL_FORMAT pixelFormat = Image.PIXEL_FORMAT.GRAYSCALE;
		private bool formatRegistered = false;
		private Mat inputMat;
		private Texture2D outputTexture;

		private Dictionary dictionary;
		private List<Mat> corners = new List<Mat> ();
		private Mat ids = new Mat ();
		private List<Mat> rejected = new List<Mat> ();
		private DetectorParameters detectorParams;
		private Mat camMatrix = new Mat ();
		private MatOfDouble distCoeffs = new MatOfDouble (0, 0, 0, 0);
		public string debugInfo1 = "";
		public string debugInfo2 = "";
		private Mat rvecs = new Mat ();
		private Mat tvecs = new Mat ();
		private Mat rotMat = new Mat (3, 3, CvType.CV_64FC1);
		private Matrix4x4 transM = new Matrix4x4 ();
		private Matrix4x4 invertYM = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, new Vector3 (1, -1, 1));
		private Matrix4x4 invertZM = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, new Vector3 (1, 1, -1));

//		public GameObject quad;
//		public Camera mainCamera;
		public float markerLength = 100;
		public GameObject cursorObject;
		public GameObject testObject;
	}
}