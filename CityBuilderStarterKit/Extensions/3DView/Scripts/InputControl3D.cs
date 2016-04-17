using UnityEngine;
using System.Collections;

/**
 * Detects clicks and responds in 3D space.
 */
namespace CBSK
{
    public class InputControl3D : MonoBehaviour
    {

        public Camera gameCamera;
        public Camera uiCamera;
        public GameObject gameView;

        float lastActionTimer;
        bool mousePressed;
        bool dragStarted;
        Vector3 lastMousePosition;
        GameObject dragTarget;

        const float MAX_CLICK_TIME = 1.0f;
        const float MAX_CLICK_DELTA = 10;
        const float MIN_DRAG_DELTA = 1.0f;

        void Update()
        {

            lastActionTimer += Time.deltaTime;

            // Mouse button down
            if (Input.GetMouseButtonDown(0) && !mousePressed)
            {
                mousePressed = true;
                lastActionTimer = 0;
                lastMousePosition = Input.mousePosition;
            }

            RaycastHit hit;
            Ray ray = gameCamera.ScreenPointToRay(Input.mousePosition);


            // Mouse button up
            if (Input.GetMouseButtonUp(0))
            {
                if (lastActionTimer < MAX_CLICK_TIME && Vector2.Distance(lastMousePosition, Input.mousePosition) < MAX_CLICK_DELTA)
                {
                    // Check for building
                    if (Physics.Raycast(ray, out hit, 10000, 1 << BuildingManager3D.BUILDING_LAYER))
                    {
                        hit.collider.gameObject.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
                    }
                }
                mousePressed = false;
                dragStarted = false;
                lastActionTimer = 0;
            }


            // Drag
            if (mousePressed && !dragStarted)
            {
                if (Vector2.Distance(lastMousePosition, Input.mousePosition) >= MAX_CLICK_DELTA)
                {
                    if (Physics.Raycast(ray, out hit, 10000, 1 << BuildingManager3D.BUILDING_LAYER))
                    {
                        dragStarted = true;
                        dragTarget = hit.collider.gameObject;
                    }
                }
            }

            if (dragStarted && Vector2.Distance(lastMousePosition, Input.mousePosition) >= MIN_DRAG_DELTA)
            {
                if (Physics.Raycast(ray, out hit, 10000, 1 << BuildingManager3D.TERRAIN_LAYER))
                {
                    dragTarget.SendMessage("OnDrag", hit.point - gameView.transform.position, SendMessageOptions.DontRequireReceiver);
                    lastMousePosition = Input.mousePosition;
                    lastActionTimer = 0;
                }
            }
        }
    }
}