using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace CBSK
{
    public class InputManager : MonoBehaviour
    {

        public static InputManager instance;

        public float maxZoom = 1;
        public float minZoom = .5f;
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        public float zoomSpeed = 1.0f;
#else
		public float zoomSpeed = 4;
#endif
        public float dragSpeed = 2f;
        public float acceleration = 4;
        public Camera inputCamera;


        private float scrollTarget;
        private float scrollDelta;
        private Vector3 zoomCenter;

        private Vector2 startDragPosition;
        private Vector3 dragDiff;
        private Vector2 targetCameraPosition;
        private float[] relativeBounds;
        private bool hasBounds;
		private Vector2[] bounds;

        //Zoom to cursor variables
        private float previousZoom;
        private Vector2 prevCursorPos;
        private Vector2 cursorDelta;

        //Drag unit variables
        private Vector2 p1;
        private Vector2 p2;
        float worldUnit;

        private bool isDragging = false;

        //This can be switched off by certain full screen panels. Expecially useful for ones that use their own dragging or scrolling to display content
        internal bool acceptInput = true;

        // Use this for initialization
        void Start()
        {
            instance = this;
            if (inputCamera == null)
            {
                inputCamera = Camera.main;
            }

            scrollTarget = inputCamera.orthographicSize;
            targetCameraPosition = inputCamera.transform.position;

            //Check to make sure the camera has the required bounds
            if (inputCamera.GetComponent<BoxCollider2D>() == null)
            {
                Debug.LogWarning(string.Format("A BoxCollider2D is required on {0} to control the camera bounds", inputCamera.name));
                return;
            }

			BoxCollider2D boundingBox = inputCamera.GetComponent<BoxCollider2D> ();
			if (boundingBox != null) {
				hasBounds = true; 
				bounds = boundingBox.GetRelativeBounds();
				SetRelativeBounds ();
			}
        }

        // Update is called once per frame
        void Update()
        {
            UpdateZoom();
            UpdateDrag();
        }


        void UpdateZoom()
        {
            //TODO add perspective camera
#if UNITY_STANDALONE || UNITY_EDITOR
            if (acceptInput)
            {
                scrollDelta = Input.GetAxis("Mouse ScrollWheel");
            }
            else
            {
                scrollDelta = 0;
            }
            scrollTarget -= scrollDelta * zoomSpeed;
			scrollTarget = Mathf.Clamp(scrollTarget, minZoom, maxZoom);
			Vector3 zoomCenter = Input.mousePosition;

#endif
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        // If there are two touches on the device...
        if (acceptInput && Input.touchCount == 2)
        {
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            scrollDelta = prevTouchDeltaMag - touchDeltaMag;
			scrollTarget += scrollDelta * zoomSpeed * Time.deltaTime;
			scrollTarget = Mathf.Clamp(scrollTarget, minZoom, maxZoom);

            //Find focal point of the zoom
            zoomCenter = (touchZero.position + touchOne.position)/2;
        }
        else
        {
            scrollDelta = 0;
        }
#endif

            prevCursorPos = inputCamera.ScreenToWorldPoint(zoomCenter);
            inputCamera.orthographicSize = Mathf.Lerp(inputCamera.orthographicSize, scrollTarget, acceleration * Time.deltaTime);
            
			// Only do this if we are scrolling
			if (Mathf.Abs(scrollTarget - inputCamera.orthographicSize) > 0.001f ) {

                // Recalculate the camera bounds, if there are any
                if (hasBounds)
                {
                    SetRelativeBounds();
                }

                //Make sure that if we start dragging before the zoom lerp has completed, that we don't fight with the drag position.
                if (!isDragging)
                {
                    // Update size and find the world delta
                    cursorDelta = prevCursorPos - (Vector2)inputCamera.ScreenToWorldPoint(zoomCenter);

                    // Update camera position to keep cursor/fingers centered, using the drag target. 
                    targetCameraPosition += cursorDelta;
                }

                //Clamp the drag target values before assigning them to the camera.
                ClampCameraPosition();

                if (!isDragging)
                {
                    inputCamera.transform.position = targetCameraPosition;
                }
			}
		}

        void UpdateDrag()
        {
			// Clamp first so that targetDragPosition guaranteed to be in bounds
			ClampCameraPosition();
            inputCamera.transform.position = Vector2.Lerp(inputCamera.transform.position, targetCameraPosition, acceleration * Time.deltaTime);

            if((Vector2)inputCamera.transform.position == targetCameraPosition)
            {
                isDragging = false;
            }
		}
		
        //Makes sure the camera can't be moved outside of the bounds.
        void ClampCameraPosition()
        {
            if (hasBounds)
            {
                //Make sure our drag position is inside the current bounds.
                targetCameraPosition.x = Mathf.Clamp(targetCameraPosition.x, relativeBounds[0], relativeBounds[1]);
                targetCameraPosition.y = Mathf.Clamp(targetCameraPosition.y, relativeBounds[2], relativeBounds[3]);
			}
		}
		
		//This method processes drag via "click and drag" and touch.
        public void OnDrag(PointerEventData data)
        {
#if UNITY_ANDROID || UNITY_IOS
        if (!acceptInput || Input.touchCount > 1)
        {
            return;
        }
#endif

            //Find the current screen to world unit ratio
            p1 = inputCamera.ScreenToWorldPoint(Vector2.zero);
            p2 = inputCamera.ScreenToWorldPoint(Vector2.right);
            worldUnit = Vector2.Distance(p1, p2);

            //Do the conversion
            data.delta *= worldUnit;

            isDragging = true;

            //Move the camera
            targetCameraPosition = targetCameraPosition - data.delta;
        }


        /// <summary>
        /// Camera bounds depends on the size or field of view. Since players have the ability to zoom, the camera's bounds change.
        /// This method finds the bounds relative to the current size of the camera
        /// </summary>
        void SetRelativeBounds()
        {
			if (hasBounds) {
				relativeBounds = new float[4];
				relativeBounds [0] = bounds[0].x + (inputCamera.orthographicSize * inputCamera.aspect);
				relativeBounds [1] = bounds[1].x - (inputCamera.orthographicSize * inputCamera.aspect);
				relativeBounds [2] = bounds[0].y + inputCamera.orthographicSize;
				relativeBounds [3] = bounds[1].y - inputCamera.orthographicSize;
			}
        }
    }

}