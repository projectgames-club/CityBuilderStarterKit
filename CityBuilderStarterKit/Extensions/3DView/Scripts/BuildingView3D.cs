using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace CBSK
{
    public class BuildingView3D : UIDraggableGridObject
    {

        protected GameObject[] components;

        protected ParticleSystem particles;

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
        }

        /**
         * Initialise the building view.
         */
        public void UI_Init(Building building)
        {
            this.building = building;
            myPosition = transform.localPosition;
            GameObject buildingViewPrefab = (GameObject)Resources.Load("3D-" + building.Type.spriteName, typeof(GameObject));
            if (buildingViewPrefab != null)
            {
                GameObject buildingView = (GameObject)GameObject.Instantiate(buildingViewPrefab);
                buildingView.transform.parent = transform;
                buildingView.transform.localPosition = Vector3.zero;
                components = buildingView.GetComponentsInChildren<MeshRenderer>().Select(o => o.gameObject).OrderBy(g => g.name).ToArray();
                if (components.Length < 1) Debug.LogWarning("Expected building to have at least two parts.");
                particles = (ParticleSystem)buildingView.GetComponentInChildren<ParticleSystem>();
            }
            else
            {
                Debug.LogWarning("Can't find prefab for building");
            }
            // Use post drag to set colour
            PostDrag(building.Position);
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
        public void UI_UpdateState()
        {
            switch (building.State)
            {
                case BuildingState.IN_PROGRESS:
                    if (particles != null) particles.Stop();
                    break;
                case BuildingState.READY:
                    foreach (GameObject go in components)
                    {
                        go.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1);
                        go.SetActive(true);
                    }
                    if (particles != null) particles.Stop();
                    break;
                case BuildingState.BUILT:
                    foreach (GameObject go in components)
                    {
                        go.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1);
                        go.SetActive(true);
                    }
                    if (particles != null) particles.Play();
                    break;
                case BuildingState.MOVING:
                    foreach (GameObject go in components)
                    {
                        go.GetComponent<Renderer>().material.color = new Color(0.25f, 1, 0.25f, 1);
                        go.SetActive(true);
                    }
                    if (particles != null) particles.Stop();
                    break;
            }
        }

        /**
         * Activity completed.
         */
        public void UI_StartActivity(Activity activity)
        {
            if (activity.Type == ActivityType.BUILD)
            {
                foreach (GameObject go in components)
                {
                    go.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1);
                    go.SetActive(false);
                }
                components[0].SetActive(true);
            }
        }


        /**
         * Indicate progress on the progress ring.
         */
        public void UI_UpdateProgress(Activity activity)
        {
            if (activity.Type == ActivityType.BUILD)
            {
                int max = (int)((float)components.Length * activity.PercentageComplete);
                for (int i = 1; i < max && i < components.Length; i++)
                {
                    components[i].GetComponent<Renderer>().material.color = new Color(1, 1, 1, 0.75f);
                    components[i].SetActive(true);
                }
            }
        }

        /**
         * Activity completed.
         */
        public void UI_CompleteActivity(string type)
        {
            if (type == ActivityType.BUILD)
            {
                foreach (GameObject go in components)
                {
                    go.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1);
                    go.SetActive(true);
                }
                // Automatically Acknowledge
                building.AcknowledgeActivity();
            }
        }

        /**
         * Store of auto generated rewards is full.
         */
        public void UI_StoreFull()
        {

        }

        /**
         * Activity acknowledged.
         */
        public void UI_AcknowledgeActivity()
        {

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
        }
        /**
         * Set color after drag.
         */

        protected void SetColor(GridPosition pos)
        {
            if (BuildingModeGrid.GetInstance().CanObjectBePlacedAtPosition(building, pos))
            {
                foreach (GameObject go in components)
                {
                    go.GetComponent<Renderer>().material.color = new Color(0.25f, 1, 0.25f, 1);
                }
            }
            else
            {
                foreach (GameObject go in components)
                {
                    go.GetComponent<Renderer>().material.color = new Color(1, 0.25f, 0.25f, 1);
                }
            }
        }

        public void OnClick()
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
                if (UIGamePanel.activePanel is UIBuildingInfoPanel) ((UIBuildingInfoPanel)UIGamePanel.activePanel).InitialiseWithBuilding(building);
            }
        }

        public void OnDrag(Vector3 newPosition)
        {
            if (CanDrag && enabled)
            {
                myPosition = newPosition;
                GridPosition pos = SnapToGrid();
                PostDrag(pos);
            }
        }

        /**
         * Snap object to grid. 
         */
        override protected GridPosition SnapToGrid()
        {
            myPosition.y = target.localPosition.y;
            GridPosition pos = grid.WorldPositionToGridPosition(myPosition);
            Vector3 position = grid.GridPositionToWorldPosition(pos);
            position.y = target.localPosition.y;
            myPosition = position;
            target.localPosition = position;
            return pos;
        }


        /**
         * Update objects position
         */
        override public void SetPosition(GridPosition pos)
        {
            Vector3 position = grid.GridPositionToWorldPosition(pos);
            position.y = target.localPosition.y;
            target.localPosition = position;
            myPosition = target.localPosition;
        }
    }
}