using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CBSK
{
    public class UIBackground : MonoBehaviour, IPointerDownHandler, IDragHandler
    {

        InputManager inputManager;

        void Awake()
        {
            inputManager = GameObject.FindObjectOfType(typeof(InputManager)) as InputManager;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            //inputManager.StartDrag();
        }

        public void OnDrag(PointerEventData eventData)
        {
            inputManager.OnDrag(eventData);
        }

    }
}
