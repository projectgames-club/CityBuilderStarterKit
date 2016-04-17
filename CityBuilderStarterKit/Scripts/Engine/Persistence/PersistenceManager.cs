using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/**
 * Class for saving and loading data.
 */
namespace CBSK
{
    public class PersistenceManager : Manager<PersistenceManager>
    {

        protected Type[] types;

        /**
         * Used to determine if there is a game that should be loaded.
         */
        virtual public bool SaveGameExists()
        {
            return false;
        }

        /**
         * Override this with a method to persist the game state.
         */
        virtual public void Save()
        {
            Debug.LogWarning("You should extend this class with your own implementation or use one of the provided implementations.");
        }

        /**
         * Override this with a method to load the game state
         */
        virtual public SaveGameData Load()
        {
            Debug.LogWarning("You should extend this class with your own implementation or use one of the provided implementations.");
            return null;
        }

        /**
         * Gets the saved game data. ovveride this if you want to use a custom type
         * to extend SaveGameData (for example if you add a new resource type).
         */
        virtual protected SaveGameData GetSaveGameData()
        {
            SaveGameData dataToSave = new SaveGameData();
            dataToSave.buildings = BuildingManager.GetInstance().GetSaveData();
            dataToSave.resources = ResourceManager.Instance.Resources;
            dataToSave.gold = ResourceManager.Instance.Gold;
            dataToSave.xp = ResourceManager.Instance.Xp;

            dataToSave.activities = ActivityManager.GetInstance().GetSaveData();
            dataToSave.otherResources = ResourceManager.Instance.OtherResources;
            return dataToSave;
        }

        virtual protected void InitTypes()
        {
            if (types == null)
            {
                Type[] buildingTypes = typeof(BuildingData).Assembly.GetTypes().Where(t => t != typeof(BuildingData) && typeof(BuildingData).IsAssignableFrom(t)).ToArray();
                Type[] activityTypes = typeof(BuildingData).Assembly.GetTypes().Where(t => t != typeof(BuildingData) && typeof(BuildingData).IsAssignableFrom(t)).ToArray();
                List<Type> allTypes = new List<Type>();
                allTypes.AddRange(buildingTypes);
                allTypes.AddRange(activityTypes);
                types = allTypes.ToArray();
            }
        }
    }
}