using UnityEngine;
using System.Collections;

/**
 * A building manager that creates 3D buildings
 */
namespace CBSK
{
    public class BuildingManager3D : BuildingManager
    {
        /**
         * Layer used for terrain collider.
         */
        public const int TERRAIN_LAYER = 10;

        /**
         * Layer used for building colliders.
         */
        public const int BUILDING_LAYER = 11;

        void Start()
        {
            if (PersistenceManager.GetInstance() != null)
            {
                if (PersistenceManager.GetInstance().SaveGameExists())
                {
                    SaveGameData savedGame = PersistenceManager.GetInstance().Load();
                    foreach (BuildingData building in savedGame.buildings)
                    {
                        CreateAndLoadBuilding(building);
                    }
                    ResourceManager.Instance.Load(savedGame);
                    ActivityManager.GetInstance().Load(savedGame);
                    return;
                }
            }
            CreateNewScene();
        }

        /**
         * Build a building. override this as we want to determine building position differently.
         */
        override public void CreateBuilding(string buildingTypeId)
        {
            if (CanBuildBuilding(buildingTypeId) && ResourceManager.Instance.CanBuild(GetBuildingTypeData(buildingTypeId)))
            {
                GameObject go = (GameObject)Instantiate(buildingPrefab);
                go.transform.parent = gameView.transform;
                Building building = go.GetComponent<Building>();
                Ray ray = gameCamera.ScreenPointToRay(new Vector2(Screen.width / 2.0f, Screen.height / 2.0f));
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1 << TERRAIN_LAYER))
                {
                    building.Init(types[buildingTypeId], BuildingModeGrid.GetInstance().WorldPositionToGridPosition(hit.point));
                    ActiveBuilding = building;
                    ResourceManager.Instance.RemoveResources(ActiveBuilding.Type.cost);
                    if ((int)saveMode < (int)SaveMode.SAVE_MOSTLY) PersistenceManager.GetInstance().Save();
                }
                else
                {
                    Debug.LogWarning("Couldn't find terrain, not able to place building.");
                }

            }
            else
            {
                if (CanBuildBuilding(buildingTypeId))
                {
                    Debug.LogWarning("This is where you bring up your in app purchase screen");
                }
                else
                {
                    Debug.LogError("Tried to build unbuildable building");
                }
            }
        }

        /**
         * Override as no obstacles in the 3D view.
         */
        override protected void CreateNewScene()
        {

        }
    }
}

