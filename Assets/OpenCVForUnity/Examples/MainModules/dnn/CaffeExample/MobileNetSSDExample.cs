﻿#if !UNITY_WEBGL && !UNITY_WSA_10_0

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_5_3 || UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif
using OpenCVForUnity;

namespace OpenCVForUnityExample
{
    /// <summary>
    /// MobileNet SSD Example
    /// This example uses Single-Shot Detector (https://arxiv.org/abs/1512.02325) to detect objects.
    /// Referring to https://github.com/opencv/opencv/blob/master/samples/dnn/mobilenet_ssd_python.py.
    /// </summary>
    public class MobileNetSSDExample : MonoBehaviour
    {
        const float inWidth = 300;
        const float inHeight = 300;
        float WHRatio = inWidth / inHeight;
        float inScaleFactor = 0.007843f;
        float meanVal = 127.5f;
                
        string[] classNames = {"background",
            "aeroplane", "bicycle", "bird", "boat",
            "bottle", "bus", "car", "cat", "chair",
            "cow", "diningtable", "dog", "horse",
            "motorbike", "person", "pottedplant",
            "sheep", "sofa", "train", "tvmonitor"
        };

        // Use this for initialization
        void Start ()
        {
            //if true, The error log of the Native side OpenCV will be displayed on the Unity Editor Console.
            Utils.setDebugMode (true);


            Mat img = Imgcodecs.imread (Utils.getFilePath ("dnn/004545.jpg"));
            #if !UNITY_WSA_10_0
            if (img.empty ()) {
                Debug.LogError ("dnn/004545.jpg is not loaded.The image file can be downloaded here: \"https://github.com/chuanqi305/MobileNet-SSD/blob/master/images/004545.jpg\".Please copy to \"Assets/StreamingAssets/dnn/\" folder. ");
                img = new Mat (375, 500, CvType.CV_8UC3, new Scalar (0, 0, 0));

            }
            #endif

            Size inVideoSize = new Size (img.width (), img.height ());
            Size cropSize;
            if (inVideoSize.width / (float)inVideoSize.height > WHRatio) {
                cropSize = new Size (inVideoSize.height * WHRatio, inVideoSize.height);
            } else {
                cropSize = new Size (inVideoSize.width, inVideoSize.width / WHRatio);
            }
            OpenCVForUnity.Rect crop = new OpenCVForUnity.Rect (new Point ((inVideoSize.width - cropSize.width) / 2, (inVideoSize.height - cropSize.height) / 2), cropSize);


            Net net = null;

            string model_filepath = Utils.getFilePath ("dnn/MobileNetSSD_deploy.caffemodel");
            string prototxt_filepath = Utils.getFilePath ("dnn/MobileNetSSD_deploy.prototxt");
           
            if (string.IsNullOrEmpty (model_filepath) || string.IsNullOrEmpty (prototxt_filepath)) {
                Debug.LogError ("model file is not loaded.The model and prototxt file can be downloaded here: \"https://github.com/chuanqi305/MobileNet-SSD\".Please copy to “Assets/StreamingAssets/dnn/” folder. ");
            } else {
                net = Dnn.readNetFromCaffe (prototxt_filepath, model_filepath);

            }

            if (net == null) {
                img = new Mat (img, crop);

                Imgproc.putText (img, "model file is not loaded.", new Point (5, img.rows () - 30), Core.FONT_HERSHEY_SIMPLEX, 0.7, new Scalar (255, 255, 255), 2, Imgproc.LINE_AA, false);
                Imgproc.putText (img, "Please read console message.", new Point (5, img.rows () - 10), Core.FONT_HERSHEY_SIMPLEX, 0.7, new Scalar (255, 255, 255), 2, Imgproc.LINE_AA, false);

            } else {

                Mat blob = Dnn.blobFromImage (img, inScaleFactor, new Size (inWidth, inHeight), new Scalar (meanVal), false, true);

                net.setInput (blob);


                TickMeter tm = new TickMeter ();
                tm.start ();

                Mat prob = net.forward ();
                prob = prob.reshape (1, (int)prob.total () / 7);

                tm.stop ();
                Debug.Log ("Inference time, ms: " + tm.getTimeMilli ());


                img = new Mat (img, crop);

                float[] data = new float[7];

                float confidenceThreshold = 0.2f;
                for (int i = 0; i < prob.rows (); i++) {

                    prob.get (i, 0, data);

                    float confidence = data [2];

                    if (confidence > confidenceThreshold) {
                        int class_id = (int)(data [1]);

                        float xLeftBottom = data [3] * img.cols ();
                        float yLeftBottom = data [4] * img.rows ();
                        float xRightTop = data [5] * img.cols ();
                        float yRightTop = data [6] * img.rows ();

                        Debug.Log ("class_id: " + class_id);
                        Debug.Log ("Confidence: " + confidence);

                        Debug.Log (" " + xLeftBottom
                        + " " + yLeftBottom
                        + " " + xRightTop
                        + " " + yRightTop);

                        Imgproc.rectangle (img, new Point (xLeftBottom, yLeftBottom), new Point (xRightTop, yRightTop),
                            new Scalar (0, 255, 0), 2);
                        string label = classNames [class_id] + ": " + confidence;
                        int[] baseLine = new int[1];
                        Size labelSize = Imgproc.getTextSize (label, Core.FONT_HERSHEY_SIMPLEX, 0.5, 1, baseLine);

                        Imgproc.rectangle (img, new Point (xLeftBottom, yLeftBottom),
                            new Point (xLeftBottom + labelSize.width, yLeftBottom + labelSize.height + baseLine [0]),
                            new Scalar (255, 255, 255), Core.FILLED);
                        Imgproc.putText (img, label, new Point (xLeftBottom, yLeftBottom + labelSize.height),
                            Core.FONT_HERSHEY_SIMPLEX, 0.5, new Scalar (0, 0, 0));
                    }
                }

                prob.Dispose ();

            }
            
            Imgproc.cvtColor (img, img, Imgproc.COLOR_BGR2RGB);

            Texture2D texture = new Texture2D (img.cols (), img.rows (), TextureFormat.RGBA32, false);

            Utils.matToTexture2D (img, texture);

            gameObject.GetComponent<Renderer> ().material.mainTexture = texture;


            Utils.setDebugMode (false);
        }
    
        // Update is called once per frame
        void Update ()
        {

        }

        /// <summary>
        /// Raises the back button click event.
        /// </summary>
        public void OnBackButtonClick ()
        {
            #if UNITY_5_3 || UNITY_5_3_OR_NEWER
            SceneManager.LoadScene ("OpenCVForUnityExample");
            #else
            Application.LoadLevel ("OpenCVForUnityExample");
            #endif
        }
    }
}
#endif