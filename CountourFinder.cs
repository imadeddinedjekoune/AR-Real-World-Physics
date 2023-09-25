#if !(UNITY_LUMIN && !UNITY_EDITOR)

using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ObjdetectModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.UnityUtils.Helper;
using OpenCVRect = OpenCVForUnity.CoreModule.Rect;


namespace OpenCVForUnityExample
{
    [RequireComponent (typeof(WebCamTextureToMatHelper))]
    public class CountourFinder : MonoBehaviour
    {
        Mat grayMat;
        Texture2D texture;
        Mat points;
        OpenCVRect imageSizeRect;
        WebCamTextureToMatHelper webCamTextureToMatHelper;
        FpsMonitor fpsMonitor;

        // Use this for initialization
        void Start ()
        {
            fpsMonitor = GetComponent<FpsMonitor> ();

            webCamTextureToMatHelper = gameObject.GetComponent<WebCamTextureToMatHelper> ();

            Debug.Log(WebCamTexture.devices[1].name);

            webCamTextureToMatHelper.Initialize (WebCamTexture.devices[1].name, 640, 480, false, 30f,false);

            #if UNITY_ANDROID && !UNITY_EDITOR
            // Avoids the front camera low light issue that occurs in only some Android devices (e.g. Google Pixel, Pixel2).
            webCamTextureToMatHelper.avoidAndroidFrontCameraLowLightIssue = true;
            #endif
        }


        /// <summary>
        /// Raises the web cam texture to mat helper initialized event.
        /// </summary>
        public void OnWebCamTextureToMatHelperInitialized ()
        {
            Debug.Log ("OnWebCamTextureToMatHelperInitialized");
            
            Mat webCamTextureMat = webCamTextureToMatHelper.GetMat ();

            texture = new Texture2D (webCamTextureMat.cols (), webCamTextureMat.rows (), TextureFormat.RGBA32, false);

            gameObject.GetComponent<Renderer> ().material.mainTexture = texture;

            gameObject.transform.localScale = new Vector3 (webCamTextureMat.cols (), webCamTextureMat.rows (), 1);
            Debug.Log ("Screen.width " + Screen.width + " Screen.height " + Screen.height + " Screen.orientation " + Screen.orientation);

            if (fpsMonitor != null) {
                fpsMonitor.Add ("width", webCamTextureMat.width ().ToString ());
                fpsMonitor.Add ("height", webCamTextureMat.height ().ToString ());
                fpsMonitor.Add ("orientation", Screen.orientation.ToString ());
            }


            float width = webCamTextureMat.width ();
            float height = webCamTextureMat.height ();
            
            float widthScale = (float)Screen.width / width;
            float heightScale = (float)Screen.height / height;
            if (widthScale < heightScale) {
                Camera.main.orthographicSize = (width * (float)Screen.height / (float)Screen.width) / 2;
            } else {
                Camera.main.orthographicSize = height / 2;
            }

            grayMat = new Mat (webCamTextureMat.rows (), webCamTextureMat.cols (), CvType.CV_8UC1);
            imageSizeRect = new OpenCVRect (0, 0, grayMat.width (), grayMat.height ());

            points = new Mat ();

            // if WebCamera is frontFaceing, flip Mat.
            if (webCamTextureToMatHelper.GetWebCamDevice ().isFrontFacing) {
                webCamTextureToMatHelper.flipHorizontal = true;
            }
        }

        /// <summary>
        /// Raises the web cam texture to mat helper disposed event.
        /// </summary>
        public void OnWebCamTextureToMatHelperDisposed ()
        {
            Debug.Log ("OnWebCamTextureToMatHelperDisposed");

            if (grayMat != null)
                grayMat.Dispose ();

            if (texture != null) {
                Texture2D.Destroy (texture);
                texture = null;
            }

            if (points != null)
                points.Dispose ();
        }

        /// <summary>
        /// Raises the web cam texture to mat helper error occurred event.
        /// </summary>
        /// <param name="errorCode">Error code.</param>
        public void OnWebCamTextureToMatHelperErrorOccurred (WebCamTextureToMatHelper.ErrorCode errorCode)
        {
            Debug.Log ("OnWebCamTextureToMatHelperErrorOccurred " + errorCode);
        }

        // Update is called once per frame

        public float min_THRESH = 0.0f ;
        public int polygones = 10 ;
        public float minArea = 2500;
        public PolygonCollider2D PolygonCollider;


        void Update ()
        {
            if (webCamTextureToMatHelper.IsPlaying () && webCamTextureToMatHelper.DidUpdateThisFrame ()) {
               
                Mat rgbaMat = webCamTextureToMatHelper.GetMat ();
                List<MatOfPoint> srcContours = new List<MatOfPoint>();
                Mat srcHierarchy = new Mat();

                Imgproc.cvtColor (rgbaMat, grayMat, Imgproc.COLOR_RGBA2GRAY);
               
                Imgproc.threshold(grayMat, grayMat, min_THRESH, 255, Imgproc.THRESH_BINARY_INV );
                
                Imgproc.findContours(grayMat, srcContours, srcHierarchy, Imgproc.RETR_TREE, Imgproc.CHAIN_APPROX_SIMPLE);

                PolygonCollider.pathCount = 0;
                foreach ( MatOfPoint contour in srcContours)
                {
                    MatOfPoint2f c2f = new MatOfPoint2f(contour.toArray());
                    MatOfPoint2f approx = new MatOfPoint2f();
                    Imgproc.approxPolyDP(c2f, approx,polygones, true);
                    var area = Imgproc.contourArea(contour);

                    if (area > minArea)
                    {
                        drawContours(grayMat,new Scalar (127,0,0) , 2 , approx.toArray());
                        PolygonCollider.pathCount++;
                        PolygonCollider.SetPath(PolygonCollider.pathCount - 1, toVector2(approx.toArray()));
                    }
                }

                Texture2D texture = new Texture2D (rgbaMat.cols (), rgbaMat.rows (), TextureFormat.RGBA32, false);
                Utils.matToTexture2D (rgbaMat, texture);
                gameObject.GetComponent<Renderer> ().material.mainTexture = texture;
            }
        }

        Vector2[] vectorList;
        private Vector2[] toVector2(Point[] points){
            vectorList = new Vector2[points.Length];
            for(int i=0;i<points.Length; i++)
            {
                vectorList[i] = new Vector2((float)points[i].x, (float)points[i].y);
            }
            return vectorList;
        }
        

        void drawContours(Mat image , Scalar  color, int thikness , Point [] Points)
        {
            for (int i = 1 ; i < Points.Length ; i++)
            {
                Imgproc.line(image,Points[i-1],Points[i],color,thikness);
            }
            Imgproc.line(image,Points[Points.Length-1],Points[0],color,thikness);
        }

        /// <summary>
        /// Raises the destroy event.
        /// </summary>
        void OnDestroy ()
        {
            webCamTextureToMatHelper.Dispose ();
        }

        /// <summary>
        /// Raises the back button click event.
        /// </summary>
        public void OnBackButtonClick ()
        {
            SceneManager.LoadScene ("OpenCVForUnityExample");
        }

        /// <summary>
        /// Raises the play button click event.
        /// </summary>
        public void OnPlayButtonClick ()
        {
            webCamTextureToMatHelper.Play ();
        }

        /// <summary>
        /// Raises the pause button click event.
        /// </summary>
        public void OnPauseButtonClick ()
        {
            webCamTextureToMatHelper.Pause ();
        }

        /// <summary>
        /// Raises the stop button click event.
        /// </summary>
        public void OnStopButtonClick ()
        {
            webCamTextureToMatHelper.Stop ();
        }

        /// <summary>
        /// Raises the change camera button click event.
        /// </summary>
        public void OnChangeCameraButtonClick ()
        {
            webCamTextureToMatHelper.requestedIsFrontFacing = !webCamTextureToMatHelper.IsFrontFacing ();
        }
    }
}

#endif