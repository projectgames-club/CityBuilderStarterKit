using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;
using Random = UnityEngine.Random;

namespace CBSK
{
    public class BuildingView : UIDraggableGridObject, IPointerClickHandler, IDragHandler
    {

        /**
         * Building Sprite
         */
        public SpriteRenderer buildingSprite;

        /**
         * Activity icon.
         */
        public Image currentActivity;

        /**
         * Game object wrapping the various
         * widgets used to indicate progress. Also
         * acts as a button.
         */
        public AcknowledgeBuildingButton progressIndicator;

        /**
         * Sprites showing progress of current activity.
         */
        public Image[] progressRings;

        /* The UIWidget that is being dragged */
        //protected UIWidget widget;

        /**
         * Reference to the camera showing this object.
         */
        protected InputManager inputManager;

        /**
         * Building this view references.
         */
        protected Building building;


        /**
         * Internal initialisation.
         */
        void Awake()
        {
            target = transform;
            myPosition = target.position;
            grid = GameObject.FindObjectOfType(typeof(AbstractGrid)) as AbstractGrid;
            inputManager = GameObject.FindObjectOfType(typeof(InputManager)) as InputManager;
        }

        /**
         * Initialise the building view.
         */
        virtual public void UI_Init(Building building)
        {
            this.building = building;
            //widget = buildingSprite;
            buildingSprite.color = Color.white;
            if (building.State == BuildingState.IN_PROGRESS || building.State == BuildingState.READY)
            {
                buildingSprite.sprite = SpriteManager.GetBuildingSprite("InProgress2x2");
            }
            else
            {
                buildingSprite.sprite = SpriteManager.GetBuildingSprite(building.Type.spriteName);
            }
            //buildingSprite.MakePixelPerfect();
            progressIndicator.building = building;
            SetColor(building.Position);
            myPosition = transform.localPosition;
            SnapToGrid();
            buildingSprite.sortingOrder = -(int)(transform.position.y * 100);
        }

        /**
         * Update objects position
         */
        override public void SetPosition(GridPosition pos)
        {
            Vector3 position = grid.GridPositionToWorldPosition(pos);
            //widget.depth = 1000 - (int)position.z;
            position.z = target.localPosition.z;
            target.localPosition = position;
            myPosition = target.localPosition;
            buildingSprite.sortingOrder = -(int)(transform.position.y * 100);
        }

        /**
         * When object is dragged, move object then snap it to the grid.
         */
        public void OnDrag(PointerEventData eventData)
        {
#if UNITY_ANDROID || UNITY_IOS
            if (Input.touchCount > 1)
            {
                return;
            }
#endif
            if (CanDrag && enabled && target != null)
            {

                transform.position = BuildingManager.GetInstance().gameCamera.ScreenToWorldPoint(eventData.position);
                myPosition = transform.localPosition;
                GridPosition pos = SnapToGrid();
                PostDrag(pos);
            }
            else
            {
                inputManager.OnDrag(eventData);
            }
        }

        /**
         * Click event... building clicked.
         */
        public void OnPointerClick(PointerEventData eventData)
        {
            if (building.Type.isObstacle)
            {
                if (building.CurrentActivity == null && building.CompletedActivity == null)
                {
                    UIGamePanel.ShowPanel(PanelType.OBSTACLE_INFO);
                    if (UIGamePanel.activePanel is UIBuildingInfoPanel) ((UIBuildingInfoPanel)UIGamePanel.activePanel).InitialiseWithBuilding(building);
                }
            }
            else if (building.State == BuildingState.BUILT)
            {
                UIGamePanel.ShowPanel(PanelType.BUILDING_INFO);
                if (UIGamePanel.activePanel is UIBuildingInfoPanel)
                {
                    ((UIBuildingInfoPanel)UIGamePanel.activePanel).InitialiseWithBuilding(building);
                }
            }
        }

        /**
         * Snap object to grid. 
         */
        override protected GridPosition SnapToGrid()
        {
            GridPosition pos = grid.WorldPositionToGridPosition(myPosition);

            Vector3 position = grid.GridPositionToWorldPosition(pos);
            position.z = target.localPosition.z;
            target.localPosition = position;
            return pos;
        }

        /**
         * Can we drag this object.
         */
        override public bool CanDrag
        {
            get
            {
                if (building.State == BuildingState.PLACING) return true;
                if (building.State == BuildingState.MOVING) return true;
                return false;
            }
            set
            {
                // Do nothing
            }
        }

        /**
         * Building state changed, update view.
         */
        virtual public void UI_UpdateState()
        {
            switch (building.State)
            {
                case BuildingState.IN_PROGRESS:
                    // Snap to grid will ensure correct depth after dragging.
                    SnapToGrid();
                    progressIndicator.gameObject.SetActive(true);
                    buildingSprite.color = Color.white;
                    buildingSprite.sprite = SpriteManager.GetBuildingSprite("InProgress2x2");
                    progressIndicator.gameObject.SetActive(true);
                    currentActivity.color = UIColor.DESATURATE;
                    progressRings[0].color = UIColor.BUILD;
                    UpdateProgressRings(0f);
                    currentActivity.sprite = SpriteManager.GetButtonSprite("build_icon");
                    break;
                case BuildingState.READY:
                    // Snap to grid will ensure correct depth after dragging.
                    SnapToGrid();
                    progressIndicator.gameObject.SetActive(true);
                    currentActivity.color = Color.white;
                    buildingSprite.color = Color.white;
                    progressRings[0].color = UIColor.BUILD;
                    UpdateProgressRings(1f);
                    StartCoroutine("DoBobble");
                    break;
                case BuildingState.BUILT:
                    // Snap to grid will ensure correct depth after dragging.
                    SnapToGrid();
                    if ((building.CurrentActivity == null || building.CurrentActivity.Type == "BUILD") &&
                        (building.CompletedActivity == null || building.CompletedActivity.Type == "BUILD"))
                    {
                        progressIndicator.gameObject.SetActive(false);
                    }
                    buildingSprite.color = Color.white;
                    buildingSprite.sprite = SpriteManager.GetBuildingSprite(building.Type.spriteName);
                    buildingSprite.color = Color.white;
                    buildingSprite.sortingLayerName = "BuildingLayer";
                    break;
                case BuildingState.MOVING:
                    SetColor(building.Position);
                    buildingSprite.sortingLayerName = "DraggingLayer";
                    break;
            }
            // StartCoroutine(WaitThenRefreshPanel());
        }

        /**
         * Activity completed.
         */
        public void UI_StartActivity(Activity activity)
        {
            if (activity.Type != ActivityType.BUILD)
            {
                progressIndicator.gameObject.SetActive(true);
                StopCoroutine("DoBobble");
                currentActivity.color = UIColor.DESATURATE;
                progressRings[0].color = UIColor.GetColourForActivityType(activity.Type);
                UpdateProgressRings(activity.PercentageComplete);
                currentActivity.sprite = SpriteManager.GetButtonSprite(activity.Type.ToString().ToLower() + "_icon");
            }
        }


        /**
         * Indicate progress on the progress ring.
         */
        public void UI_UpdateProgress(Activity activity)
        {
            UpdateProgressRings(activity.PercentageComplete);
        }

        /**
         * Activity completed.
         */
        public void UI_CompleteActivity(string type)
        {
            progressIndicator.gameObject.SetActive(true);
            currentActivity.color = Color.white;
            progressRings[0].color = UIColor.GetColourForActivityType(type);
            UpdateProgressRings(1f);
            StartCoroutine("DoBobble");
        }

        /**
         * Store of auto generated rewards is full.
         */
        public void UI_StoreFull()
        {
            if (!building.ActivityInProgress)
            {
                progressIndicator.gameObject.SetActive(true);
                currentActivity.sprite = SpriteManager.GetButtonSprite(building.Type.generationType.ToString().ToLower() + "_icon");
                currentActivity.color = Color.white;
                progressRings[0].color = UIColor.GetColourForRewardType(building.Type.generationType);
                UpdateProgressRings(1f);
                StartCoroutine("DoBobble");
            }
        }

        /**
         * Activity acknowledged.
         */
        virtual public void UI_AcknowledgeActivity()
        {
            if (!building.ActivityInProgress)
            {
                if (building.StoreFull) UI_StoreFull();
                else progressIndicator.gameObject.SetActive(false);
            }
        }

        /**
         * After object dragged set color
         */
        override protected void PostDrag(GridPosition pos)
        {
            if (building.State != BuildingState.MOVING)
            {
                building.Position = pos;
            }
            else
            {
                building.MovePosition = pos;
            }
            SetColor(pos);
            // Move forward so always clickable
            buildingSprite.sortingOrder = -(int)(transform.position.y * 100);
        }

        virtual protected void SetColor(GridPosition pos)
        {
            if (BuildingModeGrid.GetInstance().CanObjectBePlacedAtPosition(building, pos))
            {
                buildingSprite.color = UIColor.PLACING_COLOR;
            }
            else
            {
                buildingSprite.color = UIColor.PLACING_COLOR_BLOCKED;
            }
        }

        protected IEnumerator DoBobble()
        {
            while (progressIndicator.gameObject.activeInHierarchy)
            {
                iTween.PunchPosition(progressIndicator.gameObject, new Vector3(0, 0.035f, 0), 1.5f);
                yield return new WaitForSeconds(Random.Range(1.0f, 1.5f));
            }
        }

        void UpdateProgressRings(float fillAmount)
        {
            foreach (Image progressRing in progressRings)
            {
                progressRing.fillAmount = fillAmount;
            }
        }
    }
}