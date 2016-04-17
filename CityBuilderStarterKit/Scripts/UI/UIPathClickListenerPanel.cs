using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;

namespace CBSK
{
    /**
     * Listens to a user clicking in path mode and sends to the path manager
     A fullscreen graphic is required to make use the graphics raycaster
     */
     [RequireComponent(typeof(Image))]
    public class UIPathClickListenerPanel : UIGamePanel, IPointerClickHandler, IDragHandler
    {
        //Removed in initial re-release 
        //public Transform buildingOffset;

        protected InputManager inputManager;
        private Image clickMask;

        /**
         * Internal initialisation.
         */
        public override void Awake()
        {
            base.Awake();
            inputManager = GameObject.FindObjectOfType(typeof(InputManager)) as InputManager;
            clickMask = GetComponent<Image>();
        }

        public override void SetPanelActive(bool isActive)
        {
            base.SetPanelActive(isActive);
            clickMask.enabled = isActive;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (isVisible)
            {
                // TODO change that reference to be a look up of scale
                PathManager.GetInstance().SwitchPath("PATH", (BuildingManager.GetInstance().gameCamera.ScreenToWorldPoint(Input.mousePosition)) +
                                                    BuildingManager.GetInstance().gameView.transform.localPosition * -1.0f);
            }
        }

        /**
         * When object is dragged, move object then snap it to the grid.
         */
        public void OnDrag(PointerEventData eventData)
        {
            if (isVisible)
            {
                //TODO Add the ability to draw paths by dragging.
                inputManager.OnDrag(eventData);
            }
        }
    }
}