using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/**
 * Provides convenience functions for working with paths
 */
namespace CBSK
{
    public class PathManager : Manager<PathManager>
    {

        /**
         * Prefab to use when creating paths.
         */
        public GameObject pathPrefab;


        /**
         * How often to save data.
         */
        public SaveMode saveMode;

        /**
         * What separates sprite name from the direction flags
         */
        protected const string SUFFIX = "-";

        /**
         * Reference to grid
         */
        protected BuildingModeGrid grid;

        void Start()
        {
            grid = BuildingModeGrid.GetInstance();

        }

        /**
         *	Place path on the grid. 
         */
        virtual public void SwitchPath(string buildingTypeId, Vector3 worldPosition)
        {

            Building building = null;
            BuildingTypeData data = BuildingManager.GetInstance().GetBuildingTypeData(buildingTypeId);
            GridPosition pos = grid.WorldPositionToGridPosition(worldPosition);
            // If path exists remove and give resources
            IGridObject gridObject = grid.GetObjectAtPosition(pos);
            if (gridObject is Building && ((Building)gridObject).Type.isPath)
            {
                ResourceManager.Instance.AddResources(((Building)gridObject).Type.cost);
                if (((Building)gridObject).Type.additionalCosts != null)
                {
                    foreach (CustomResource cost in ((Building)gridObject).Type.additionalCosts)
                    {
                        ResourceManager.Instance.AddCustomResource(cost.id, cost.amount);
                    }
                }
                grid.RemoveObject(gridObject);
                BuildingManager.GetInstance().RemoveBuilding((Building)gridObject);
                Destroy(((Building)gridObject).gameObject);
            }

            // Add new path unless we just deleted a path of same type
            if ((gridObject == null || !(gridObject is Building) || ((Building)gridObject).Type.id != buildingTypeId) &&
                    data.isPath && BuildingManager.GetInstance().CanBuildBuilding(buildingTypeId) &&
                grid.CanObjectBePlacedAtPosition(data.shape.ToArray(), pos) &&
                    ResourceManager.Instance.CanBuild(data))
            {
                GameObject go;
                go = (GameObject)Instantiate(pathPrefab);
                go.transform.parent = BuildingManager.GetInstance().gameView.transform;
                building = go.GetComponent<Building>();
                building.Init(data, pos);
                building.Acknowledge();
                ResourceManager.Instance.RemoveResources(data.cost);
                if (data.additionalCosts != null)
                {
                    foreach (CustomResource cost in data.additionalCosts)
                    {
                        ResourceManager.Instance.RemoveCustomResource(cost.id, cost.amount);
                    }
                }
                grid.AddObjectAtPosition(building, pos);
                BuildingManager.GetInstance().AddBuilding(building);
                if ((int)saveMode == (int)SaveMode.SAVE_ALWAYS) PersistenceManager.GetInstance().Save();
            }
            else
            {
                if (BuildingManager.GetInstance().CanBuildBuilding(buildingTypeId))
                {
                    // TODO Show info message if not enough resource
                }
                else
                {
                    Debug.LogError("Tried to build unbuildable path");
                }
            }

            UpdatePosition(pos, building);
        }

        /**
         * Started path bulding
         */
        virtual public void EnterPathMode()
        {

        }

        /**
         * Finished path bulding, make sure to save.
         */
        virtual public void ExitPathMode()
        {
            if ((int)saveMode < (int)SaveMode.SAVE_NEVER) PersistenceManager.GetInstance().Save();
        }

        /**
         * Create a path during loading process
         */
        virtual public void CreateAndLoadPath(BuildingData data)
        {
            GameObject go;
            go = (GameObject)Instantiate(pathPrefab);
            go.transform.parent = BuildingManager.GetInstance().gameView.transform;
            Building building = go.GetComponent<Building>();
            building.Init(BuildingManager.GetInstance().GetBuildingTypeData(data.buildingTypeString), data);
            grid.AddObjectAtPosition(building, data.position);
            BuildingManager.GetInstance().AddBuilding(building);
            building.Acknowledge();
            UpdatePosition(data.position, building);
        }

        /**
         * Switch a grid position to be path (or not path).
         */
        virtual public void UpdatePosition(GridPosition pos, Building building)
        {

            List<GridPosition> positions = new List<GridPosition>();
            positions.Add(pos);
            positions.Add(pos + new GridPosition(1, 0));
            positions.Add(pos + new GridPosition(-1, 0));
            positions.Add(pos + new GridPosition(0, 1));
            positions.Add(pos + new GridPosition(0, -1));


            if (building != null && pos != building.Position)
            {
                grid.RemoveObjectAtPosition(building.Position);
                positions.Add(building.Position);
                positions.Add(building.Position + new GridPosition(1, 0));
                positions.Add(building.Position + new GridPosition(-1, 0));
                positions.Add(building.Position + new GridPosition(0, 1));
                positions.Add(building.Position + new GridPosition(0, -1));
                building.Position = pos;
            }

            UpdatePositions(positions.Distinct().ToList());

        }

        /**
         * Update all the views for the given UI position. 
         */
        virtual public void UpdatePositions(List<GridPosition> positions)
        {
            // Not super efficient but it does the job
            foreach (GridPosition pos in positions)
            {
                IGridObject gridObject = grid.GetObjectAtPosition(pos);
                if (gridObject is Building && ((Building)gridObject).Type.isPath)
                {
                    ((Building)gridObject).gameObject.SendMessage("UI_UpdateState", SendMessageOptions.DontRequireReceiver);
                }
            }
        }

        /// <summary>
        /// Gets the sprite suffix to use for a path at the given grid position.
        /// </summary>
        /// <returns>The sprite suffix.</returns>
        /// <param name="position">Position.</param>
        virtual public string GetSpriteSuffix(Building building)
        {
            string suffix = SUFFIX;
            IGridObject gridObject = grid.GetObjectAtPosition(building.Position + new GridPosition(1, 0));
            if (gridObject != building && gridObject != null && gridObject is Building && ((Building)gridObject).Type == building.Type) suffix += "N";

            gridObject = grid.GetObjectAtPosition(building.Position + new GridPosition(0, -1));
            if (gridObject != building && gridObject != null && gridObject is Building && ((Building)gridObject).Type == building.Type) suffix += "E";

            gridObject = grid.GetObjectAtPosition(building.Position + new GridPosition(-1, 0));
            if (gridObject != building && gridObject != null && gridObject is Building && ((Building)gridObject).Type == building.Type) suffix += "S";

            gridObject = grid.GetObjectAtPosition(building.Position + new GridPosition(0, 1));
            if (gridObject != building && gridObject != null && gridObject is Building && ((Building)gridObject).Type == building.Type) suffix += "W";

            // Default to the open path
            if (suffix == SUFFIX) suffix = "-NESW";

            return suffix;
        }

    }
}