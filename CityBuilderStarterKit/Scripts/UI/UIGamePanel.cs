using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


namespace CBSK
{
    public class UIGamePanel : MonoBehaviour
    {

        public const float UI_DELAY = 0.75f;
        public const float UI_TRAVEL_DIST = 0.6f;

        public Vector2 NEW_UI_TRAVEL_DIST;
        public float NEW_UI_DELAY = 10f;

        public GameObject animatePanel;
        public PanelType panelType;
        [BitMask(typeof(PanelType))]
        public PanelType openFromFlags;

        //Used to hide and show the panel
        internal bool isVisible;
        private GameObject contentPanel;
        private RectTransform uiTransform;
        /**
         * Position of the buttons when visible.
         */
        protected Vector3 showPosition;

        /**
         * Position of the buttons when hidden.
         */
        protected Vector3 hidePosition;

        public static Dictionary<PanelType, UIGamePanel> panels;

        public virtual void Awake()
        {
            if (panels == null) panels = new Dictionary<PanelType, UIGamePanel>();
            panels.Add(panelType, this);

            if (animatePanel != null)
            {
                //A subpanel has been defined, perform the transform movement on it instead
                uiTransform = animatePanel.GetComponent<RectTransform>();
            }
            else
            {
                uiTransform = GetComponent<RectTransform>();
            }
            contentPanel = transform.GetChild(0).gameObject;
            isVisible = contentPanel.activeInHierarchy;

            showPosition = uiTransform.anchoredPosition;
            hidePosition = (Vector2)showPosition + NEW_UI_TRAVEL_DIST;


            if (panelType == PanelType.DEFAULT)
            {
                activePanel = this;
            }
            else
            {
                uiTransform.anchoredPosition = hidePosition;
            }
            Init();
        }

        virtual protected void Init()
        {
        }

        virtual public void InitialiseWithBuilding(Building building)
        {
        }

        virtual public void Show()
        {
            if (activePanel == this)
            {
                StartCoroutine(DoReShow());
            }
            else if (activePanel == null || HasOpenPanelFlag(activePanel.panelType))
            {

                if (activePanel != null)
                {
                    activePanel.Hide();
                }
                StartCoroutine(DoShow());
                activePanel = this;
            }
        }

        virtual public void Hide()
        {
            StartCoroutine(DoHide());
        }

        public static void ShowPanel(PanelType panelType)
        {
            if (panelType == PanelType.DEFAULT) BuildingManager.ActiveBuilding = null;
            if (panels.ContainsKey(panelType))
            {
                panels[panelType].Show();
            }
        }

        public static UIGamePanel activePanel;

        /**
         * Reshow the panel (i.e. same panel but for a different object/building).
         */
        virtual protected IEnumerator DoReShow()
        {
            MoveTo(hidePosition);
            yield return new WaitForSeconds(UI_DELAY / 3.0f);
            MoveTo(showPosition);
        }


        /**
         * Show the panel.
         */
        virtual protected IEnumerator DoShow()
        {
            yield return new WaitForSeconds(UI_DELAY / 3.0f);
            //uiGroup.alpha = 1;
            //uiGroup.blocksRaycasts = true;
            SetPanelActive(true);
            MoveTo(showPosition);

        }

        /**
         * Hide the panel. 
         */
        virtual protected IEnumerator DoHide()
        {

            MoveTo(hidePosition);

            yield return new WaitForSeconds(UI_DELAY / 3.0f);

            //uiGroup.alpha = 0;
            //uiGroup.blocksRaycasts = false;

            SetPanelActive(false);
        }

        public virtual void SetPanelActive(bool isActive)
        {
            contentPanel.SetActive(isActive);
            isVisible = isActive;
        }

        internal void MoveTo(Vector2 targetPosition)
        {
            iTween.ValueTo(gameObject, iTween.Hash(
            "from", uiTransform.anchoredPosition,
            "to", targetPosition,
            "time", UI_DELAY,
            "easetype", iTween.Defaults.easeType,
            "onupdatetarget", this.gameObject,
            "onupdate", "UpdateRectTransform")
            );
        }

        private void UpdateRectTransform(Vector2 position)
        {
            uiTransform.anchoredPosition = position;
        }

        internal bool HasOpenPanelFlag(PanelType typeToCheck)
        {
            int val = (int)typeToCheck;
            if (openFromFlags == 0) return true;
            return (val & (int)openFromFlags) == val;
        }
    }

    [System.Flags]
    public enum PanelType
    {
        PLACE_PATH = (1 << 0),
        DEFAULT = (1 << 1),
        BUY_BUILDING = (1 << 2),
        PLACE_BUILDING = (1 << 3),
        RESOURCE = (1 << 4),
        BUY_RESOURCES = (1 << 5),
        BUILDING_INFO = (1 << 6),
        SELL_BUILDING = (1 << 7),
        OBSTACLE_INFO = (1 << 8),
        SPEED_UP = (1 << 9),
        RECRUIT_OCCUPANTS = (1 << 10),
        VIEW_OCCUPANTS = (1 << 11),
        TARGET_SELECT = (1 << 12),
        BATTLE_RESULTS = (1 << 13)
    }
}