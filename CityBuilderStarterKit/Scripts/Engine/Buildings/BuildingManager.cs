using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/**
 * Manages building types and building instances. This is also the entry point for loading save game data.
 */
namespace CBSK
{
    public class BuildingManager : Manager<BuildingManager>
    {

        /**
         * Percentage of resources reclaimed when selling a building.
         */
        public static float RECLAIM_PERCENTAGE = 0.75f;

        /**
         * Percentage of resources reclaimed as gold when selling a building. If set to 0
         * you cannot sell the building for gold.
         */
        public static float GOLD_SELL_PERCENTAGE = 0.01f;

        /**
         * The number of seconds of speed-up each gold coin buys.
         */
        public static float GOLD_TO_SECONDS_RATIO = 60;

        /**
         * Prefab to use when creating buildings.
         */
        public GameObject buildingPrefab;


        /**
         * The game object which buildings are parented to.
         */
        public GameObject gameView;

        /**
         * We use the game camera to place new buildings in the middle of the screen.
         */
        public Camera gameCamera;

        /**
         * A list of data files (resources) containing building data
         */
        public List<string> buildingDataFiles;

        /**
         * How often to save data.
         */
        public SaveMode saveMode;

        /**
         * Building types mapped to ids.
         */
        protected Dictionary<string, BuildingTypeData> types;

        /**
         * Loader for loading the data.
         */
        protected Loader<BuildingTypeData> loader;

        /**
         * Individual buildings mapped to guids.
         */
        protected Dictionary<string, Building> buildings;

        /**
         * List of buildings in progress
         */
        protected List<Building> buildingsInProgress;

        /**
         * Initialise the instance.
         */
        override protected void Init()
        {
            types = new Dictionary<string, BuildingTypeData>();
            buildings = new Dictionary<string, Building>();
            buildingsInProgress = new List<Building>();

            if (buildingDataFiles != null)
            {
                foreach (string dataFile in buildingDataFiles)
                {
                    LoadBuildingDataFromResource(dataFile, false);
                }
            }
            initialised = true;
        }

        void Start()
        {
            if (PersistenceManager.GetInstance() != null)
            {
                if (PersistenceManager.GetInstance().SaveGameExists())
                {
                    SaveGameData savedGame = PersistenceManager.GetInstance().Load();
                    foreach (BuildingData building in savedGame.buildings)
                    {
                        if (types[building.buildingTypeString].isPath)
                        {
                            PathManager.GetInstance().CreateAndLoadPath(building);
                        }
                        else
                        {
                            CreateAndLoadBuilding(building);
                        }
                    }
                    ResourceManager.Instance.Load(savedGame);
                    ActivityManager.GetInstance().Load(savedGame);
                    return;
                }
            }
            CreateNewScene();
        }

        /**
         * Create a few obstacles if the scene is empty.
         */
        virtual protected void CreateNewScene()
        {
            // Create a new scene
            CreateObstacle("ROCK", new GridPosition(25, 25));
            CreateObstacle("ROCK", new GridPosition(17, 28));
            CreateObstacle("TREE", new GridPosition(28, 19));
        }

        /**
         * Get a list of each building type.
         */
        virtual public List<BuildingTypeData> GetAllBuildingTypes()
        {
            return types.Values.ToList();
        }

        /**
         * Load the building type data from the given resource.
         * 
         * @param dataFile	Name of the resource to load data from.
         * @param skipDuplicates	If false throw an exception if a duplicate is found.
         */
        virtual public void LoadBuildingDataFromResource(string dataFile, bool skipDuplicates)
        {
            if (loader == null) loader = new Loader<BuildingTypeData>();
            List<BuildingTypeData> data = loader.Load(dataFile);
            foreach (BuildingTypeData type in data)
            {
                try
                {
                    types.Add(type.id, type);
                }
                catch (Exception ex)
                {
                    if (!skipDuplicates) throw ex;
                }
            }
        }

        /**
         * Return all completed buildings. Returns a copy of the list.
         */
        virtual public List<Building> GetAllBuildings()
        {
            List<Building> result = new List<Building>();
            result.AddRange(buildings.Values);
            return result;
        }

        /**
         * Return the type data for the given building id. Returns null if the building type is not found.
         */
        virtual public BuildingTypeData GetBuildingTypeData(string id)
        {
            if (types.ContainsKey(id))
            {
                return types[id];
            }
            return null;
        }

        /**
         * Return true if the player has at least one building of the given type.
         */
        virtual public bool PlayerHasBuilding(string id)
        {
            if (buildings.Values.Where(b => b.Type.id == id).Count() > 0) return true;
            return false;
        }

        /**
         * Return true if player can build the given building. Excludes
         * resource costs as we can pop up an IAP purchase window here.
         */
        virtual public bool CanBuildBuilding(string buildingTypeId)
        {
            if (types[buildingTypeId].level > ResourceManager.Instance.Level) return false;
            if (types.ContainsKey(buildingTypeId))
            {
                foreach (string id in types[buildingTypeId].requireIds)
                {
                    if (!PlayerHasBuilding(id)) return false;
                }
            }
            else
            {
                Debug.LogError("Unknown building id: " + buildingTypeId);
                return false;
            }
            return true;
        }

        /**
         * Return true if player can build the given building. Excludes
         * resource costs as we can pop up an IAP purchase window here.
         */
        virtual public bool CanBuildBuilding(Building building)
        {
            foreach (string id in building.Type.requireIds)
            {
                if (!PlayerHasBuilding(id)) return false;
            }
            return true;
        }

        /**
         * Build a building.
         */
        virtual public void CreateBuilding(string buildingTypeId)
        {
            if (CanBuildBuilding(buildingTypeId) && ResourceManager.Instance.CanBuild(GetBuildingTypeData(buildingTypeId)))
            {
                GameObject go;
                go = (GameObject)Instantiate(buildingPrefab);
                go.transform.parent = gameView.transform;
                Building building = go.GetComponent<Building>();
                Vector3 point = gameCamera.ScreenToWorldPoint(new Vector2(Screen.width / 2.0f, Screen.height / 2.0f)) + (go.transform.parent.localPosition * -1);
                building.Init(types[buildingTypeId], BuildingModeGrid.GetInstance().WorldPositionToGridPosition(point));
                ActiveBuilding = building;

                //If we are in 2D mode, setup 2D options
                if (gameCamera.orthographic)
                {
                    go.GetComponentInChildren<Canvas>().worldCamera = Camera.main;
                    go.AddComponent<BoxCollider2D>();
                }

                ResourceManager.Instance.RemoveResources(ActiveBuilding.Type.cost);
                if (ActiveBuilding.Type.additionalCosts != null)
                {
                    foreach (CustomResource cost in ActiveBuilding.Type.additionalCosts)
                    {
                        ResourceManager.Instance.RemoveCustomResource(cost.id, cost.amount);
                    }
                }
                if ((int)saveMode < (int)SaveMode.SAVE_MOSTLY) PersistenceManager.GetInstance().Save();
            }
            else
            {
                if (CanBuildBuilding(buildingTypeId))
                {
                    // TODO Show info message if not enough resource
                }
                else
                {
                    Debug.LogError("Tried to build unbuildable building");
                }
            }
        }

        /**
         * Create a building during loading process
         */
        virtual public void CreateAndLoadBuilding(BuildingData data)
        {
            GameObject go;

            go = (GameObject)Instantiate(buildingPrefab);

            go.transform.parent = gameView.transform;
            Building building = go.GetComponent<Building>();
            building.Init(GetBuildingTypeData(data.buildingTypeString), data);


            //If we are in 2D mode, setup 2D options
            if (gameCamera.orthographic)
            {
                go.GetComponentInChildren<Canvas>().worldCamera = Camera.main;
                go.AddComponent<BoxCollider2D>();
            }
            
            if (building.State != BuildingState.BUILT) buildingsInProgress.Add(building);
            else buildings.Add(building.uid, building);
            BuildingModeGrid.GetInstance().AddObjectAtPosition(building, data.position);
        }


        /**
         *	Place active building on the grid. 
         *
         * Returns true if successful.
         */
        virtual public bool PlaceBuilding()
        {
            if (CanBuildBuilding(ActiveBuilding))
            {
                ActiveBuilding.Place();
                buildingsInProgress.Add(ActiveBuilding);
                BuildingModeGrid.GetInstance().AddObjectAtPosition(ActiveBuilding, ActiveBuilding.Position);
                ActiveBuilding = null;
                if ((int)saveMode < (int)SaveMode.SAVE_NEVER) PersistenceManager.GetInstance().Save();
                return true;
            }
            return false;
        }

        /**
         *	Move active building on the grid. 
         *
         *  Returns true if successful.
         */
        virtual public bool MoveBuilding()
        {
            BuildingModeGrid.GetInstance().RemoveObject(ActiveBuilding);
            ActiveBuilding.Position = ActiveBuilding.MovePosition;
            BuildingModeGrid.GetInstance().AddObjectAtPosition(ActiveBuilding, ActiveBuilding.Position);
            ActiveBuilding.FinishMoving();
            if ((int)saveMode < (int)SaveMode.SAVE_NEVER) PersistenceManager.GetInstance().Save();
            return true;
        }

        /**
         *	Finish active building for gold, or speed up activity if its already built.
         *
         *  Returns true if successful.
         */
        virtual public bool SpeedUp()
        {
            if (ActiveBuilding == null)
            {
                Debug.LogError("Active Building was NULL");
                return false;
            }
            if (ActiveBuilding.CurrentActivity == null)
            {
                Debug.LogError("Current activity was NULL");
                return false;
            }
            int cost = ((int)Mathf.Max(1, (float)(ActiveBuilding.CurrentActivity.RemainingTime.TotalSeconds + 1) / (float)BuildingManager.GOLD_TO_SECONDS_RATIO));
            if (ResourceManager.Instance.Gold >= cost)
            {
                ResourceManager.Instance.RemoveGold(cost);
                if (ActiveBuilding.CurrentActivity.Type == ActivityType.BUILD)
                {
                    ActiveBuilding.CompleteBuild();
                    AcknowledgeBuilding(ActiveBuilding);
                }
                else
                {
                    ActiveBuilding.SpeedUp();
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        /**
         * Acknowledge building changing it from READY
         * to BUILT.
         */
        virtual public bool AcknowledgeBuilding(Building building)
        {
            if (building.State == BuildingState.READY)
            {
                building.Acknowledge();
                buildings.Add(building.uid, building);
                buildingsInProgress.Remove(building);
                ResourceManager.Instance.AddXp(GetXpForBuildingBuilding(building));
                if ((int)saveMode < (int)SaveMode.SAVE_NEVER) PersistenceManager.GetInstance().Save();
                return true;
            }
            return false;
        }

        /**
         * Sell building. 
         *
         * Returns true if successful.
         */
        virtual public bool SellBuilding(Building building, bool fullRefund = false)
        {
            if (buildings.ContainsKey(building.uid)) buildings.Remove(building.uid);
            ResourceManager.Instance.AddResources(fullRefund ? building.Type.cost : (int)(building.Type.cost * RECLAIM_PERCENTAGE));
            if (building.Type.additionalCosts != null)
            {
                foreach (CustomResource cost in building.Type.additionalCosts)
                {
                    ResourceManager.Instance.AddCustomResource(cost.id, fullRefund ? cost.amount : (int)(cost.amount * RECLAIM_PERCENTAGE));
                }
            }
            if (building.State != BuildingState.PLACING && building.State != BuildingState.PLACING_INVALID)
                BuildingModeGrid.GetInstance().RemoveObject(building);
            if (ActiveBuilding == building) ActiveBuilding = null;
            Destroy(building.gameObject);
            if ((int)saveMode < (int)SaveMode.SAVE_NEVER) PersistenceManager.GetInstance().Save();
            return true;
        }

        /**
         * Sells building for gold. 
         *
         * Returns true if successful.
         */
        virtual public bool SellBuildingForGold(Building building)
        {
            if (buildings.ContainsKey(building.uid)) buildings.Remove(building.uid);
            ResourceManager.Instance.AddGold((int)Mathf.Max(1.0f, (int)(building.Type.cost * GOLD_SELL_PERCENTAGE)));
            BuildingModeGrid.GetInstance().RemoveObject(building);
            if (ActiveBuilding == building) ActiveBuilding = null;
            Destroy(building.gameObject);
            if ((int)saveMode < (int)SaveMode.SAVE_NEVER) PersistenceManager.GetInstance().Save();
            return true;
        }

        /**
         * Clear an obstacle.
         *
         * Returns true if successful.
         */
        virtual public bool ClearObstacle(Building building)
        {
            if (buildings.ContainsKey(building.uid)) buildings.Remove(building.uid);
            switch (building.Type.generationType)
            {
                // TODO Special cases
                case RewardType.RESOURCE:
                    ResourceManager.Instance.AddResources(building.Type.generationAmount);
                    break;
                case RewardType.GOLD:
                    ResourceManager.Instance.AddGold(building.Type.generationAmount);
                    break;
            }
            BuildingModeGrid.GetInstance().RemoveObject(building);
            if (ActiveBuilding == building) ActiveBuilding = null;
            Destroy(building.gameObject);
            if ((int)saveMode < (int)SaveMode.SAVE_NEVER) PersistenceManager.GetInstance().Save();
            return true;
        }

        /**
         * Create an Obstacle in the scene (generally used on first load).
         */
        virtual protected void CreateObstacle(string buildingTypeId, GridPosition pos)
        {
            if (!types.ContainsKey(buildingTypeId))
            {
                Debug.LogWarning("Tried to create an obstacle with an invalid ID");
                return;
            }
            BuildingTypeData type = types[buildingTypeId];
            if (!type.isObstacle)
            {
                Debug.LogError("Tried to create an obstacle with non-obstacle building data");
                return;
            }
            GameObject go = (GameObject)Instantiate(buildingPrefab);
            go.transform.parent = gameView.transform;
            Building obstacle = go.GetComponent<Building>();
            obstacle.Init(type, pos);

            //If we are in 2D mode, setup 2D options
            if (gameCamera.orthographic)
            {
                go.GetComponentInChildren<Canvas>().worldCamera = Camera.main;
                go.AddComponent<BoxCollider2D>();
            }

            BuildingModeGrid.GetInstance().AddObjectAtPosition(obstacle, pos);
            buildings.Add(obstacle.uid, obstacle);
            obstacle.Acknowledge();
            if ((int)saveMode < (int)SaveMode.SAVE_MOSTLY) PersistenceManager.GetInstance().Save();
        }

        /**
         * Add a building to the list (for example for paths or if you have other custom builders).
         */
        virtual public void AddBuilding(Building building)
        {
            buildings.Add(building.uid, building);
        }

        /**
         * Remove a building from the list (for example for paths or if you have other custom builders).
         */
        virtual public void RemoveBuilding(Building building)
        {
            buildings.Remove(building.uid);
        }


        /**
         * Building currently being placed/moved/interacted with.
         */
        protected static Building _activeBuilding;
        public static Building ActiveBuilding
        {
            get { return _activeBuilding; }
            set
            {
                // Make sure we cancel any moving if we click another bulding
                if (_activeBuilding != null && _activeBuilding.State == BuildingState.MOVING)
                {
                    _activeBuilding.ResetPosition();
                    _activeBuilding.FinishMoving();
                }
                _activeBuilding = value;
            }
        }

        /**
         * Gets built and in progress building state for use by the save game system.
         */
        virtual public List<BuildingData> GetSaveData()
        {
            List<BuildingData> dataToSave = new List<BuildingData>();
            dataToSave.AddRange(buildingsInProgress.Select(b => b.BuildingData).ToList());
            dataToSave.AddRange(buildings.Values.Select(b => b.BuildingData).ToList());
            return dataToSave;
        }

        /// <summary>
        /// Gets the xp for building the provided building.
        /// </summary>
        /// <returns>The xp for building building.</returns>
        /// <param name="building">Building.</param>
        virtual public int GetXpForBuildingBuilding(Building building)
        {
            return (building.Type.level + 1) * building.Type.cost;
        }

    }
}