using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CBSK
{
    /**
     * Handles resources, gold, etc.
     */
    public class ResourceManager : MonoBehaviour
    {

        /**
         * View of resources.
         */
        public GameObject view;

        /**
         * Default for resources when new game is started.
         */
        public int defaultResources = 400;

        /**
         * Default for gold when new game is started.
         */
        public int defaultGold = 20;

        /**
         * A list of data files (resources) containing custom resource data
         */
        public List<string> resourceDataFiles;

        /**
         * The resource used for actually building buildings. In some games this is called coins or gp.
         */
        virtual public int Resources
        {
            get; protected set;
        }

        /**
         * The resource used to speed things up. Some games might call this gems or cash.
         */
        virtual public int Gold
        {
            get; protected set;
        }

        /**
         * The experiecne of the current player.
         */
        virtual public int Xp
        {
            get; protected set;
        }

        /**
         * Level of the player calculated from experience.
         */
        virtual public int Level
        {
            get
            {
                return GetLevelForExperience(Xp);
            }
        }

        /**
         * Get amount of a custom resource. Returns -1 if type not found.
         * 
         */
        virtual public int GetCustomResource(string type)
        {
			if (type == null) return -1;
            if (customResourceData.ContainsKey(type)) return customResourceData[type];
            return -1;
        }

        /**
         * Add an amount of a custom resource. Returns resulting amount or -1 if type does
         * not exist.
         */
        virtual public int AddCustomResource(string type, int amount)
        {
            if (customResourceData.ContainsKey(type))
            {
                int original = customResourceData[type];
                customResourceData.Remove(type);
                customResourceData.Add(type, original + amount);
                // We could make this cleaner by only sending the update we need, but this will do for illustration purposes
                if (customResourceTypes.Count > 0) view.SendMessage("UpdateCustomResource1", false, SendMessageOptions.DontRequireReceiver);
                if (customResourceTypes.Count > 1) view.SendMessage("UpdateCustomResource2", false, SendMessageOptions.DontRequireReceiver);
                return customResourceData[type];
            }
            else
            {
                if (customResourceTypes.Where(t => t.id == type).Count() > 0)
                {
                    customResourceData.Add(type, amount);
                    // We could make this cleaner by only sending the update we need, but this will do for illustration purposes
                    if (customResourceTypes.Count > 0) view.SendMessage("UpdateCustomResource1", false, SendMessageOptions.DontRequireReceiver);
                    if (customResourceTypes.Count > 1) view.SendMessage("UpdateCustomResource2", false, SendMessageOptions.DontRequireReceiver);
                    return customResourceData[type];
                }
            }
            Debug.LogWarning("The resource type " + type + " is not defined in the resource configuration.");
            return -1;
        }

        /**
         * Remove an amount of a custom resource, returns resulting amount or -1 if resource type not 
         * found (or not enough resource to remvoe amount).
         */
        virtual public int RemoveCustomResource(string type, int amount)
        {
            if (customResourceData.ContainsKey(type))
            {
                int original = customResourceData[type];
                if (original - amount < 0)
                {
                    Debug.LogWarning("Tried to buy something without enough " + type);
                    return -1;
                }
                customResourceData.Remove(type);
                customResourceData.Add(type, original - amount);
                // We could make this cleaner by only sending the update we need, but this will do for illustration purposes
                if (customResourceTypes.Count > 0) view.SendMessage("UpdateCustomResource1", false, SendMessageOptions.DontRequireReceiver);
                if (customResourceTypes.Count > 1) view.SendMessage("UpdateCustomResource2", false, SendMessageOptions.DontRequireReceiver);
                return customResourceData[type];
            }
            Debug.LogWarning("The resource type " + type + " is not defined in the resource configuration.");
            return -1;
        }

        /**
         * Get a list of current custom resource data
         */
        virtual public List<CustomResource> OtherResources
        {
            get
            {
                List<CustomResource> result = new List<CustomResource>();
                foreach (string key in customResourceData.Keys)
                {
                    result.Add(new CustomResource(key, customResourceData[key]));
                }
                return result;
            }
        }

        /**
         * Get all resource types.
         */
        public List<CustomResourceType> GetCustomResourceTypes()
        {
            return customResourceTypes.ToList();
        }

        /**
         * Get resource data for given type or null if not found.
         */
        public CustomResourceType GetCustomResourceType(string type)
        {
            return customResourceTypes.Where(t => t.id == type).FirstOrDefault();
        }

        /**
         * List of resource type data.
         */
        protected List<CustomResourceType> customResourceTypes;

        /**
         * Dictionary of resource id to current amount.
         */
        protected Dictionary<string, int> customResourceData;

        /**
         * Loader for loading the data.
         */
        protected Loader<CustomResourceType> loader;

        void Awake()
        {
            Instance = this;
            Resources = defaultResources;
            Gold = defaultGold;
            Xp = 0;
            customResourceTypes = new List<CustomResourceType>();
            customResourceData = new Dictionary<string, int>();

            if (resourceDataFiles != null)
            {
                foreach (string dataFile in resourceDataFiles)
                {
                    LoadResourceDataFromResource(dataFile, false);
                }
            }
        }

        /**
         * Load resources from save game data.
         */
        virtual public void Load(SaveGameData data)
        {
            Resources = data.resources;
            Gold = data.gold;
            Xp = data.xp;

            foreach (CustomResource c in data.otherResources)
            {
                // Only add data for resources that we have a type for
                if (customResourceData.ContainsKey(c.id))
                {
                    customResourceData.Remove(c.id);
                    customResourceData.Add(c.id, c.amount);
                }
            }
            view.SendMessage("UpdateResource", true, SendMessageOptions.DontRequireReceiver);
            view.SendMessage("UpdateGold", true, SendMessageOptions.DontRequireReceiver);
            if (customResourceTypes.Count > 0) view.SendMessage("UpdateCustomResource1", true, SendMessageOptions.DontRequireReceiver);
            if (customResourceTypes.Count > 1) view.SendMessage("UpdateCustomResource2", true, SendMessageOptions.DontRequireReceiver);
            view.SendMessage("UpdateLevel", false, SendMessageOptions.DontRequireReceiver);
        }

        /**
         * Return true if there are enough resources to build the given building.
         */
        public bool CanBuild(BuildingTypeData building)
        {
            if (Resources >= building.cost)
            {
                if (building.additionalCosts != null)
                {
                    foreach (CustomResource c in building.additionalCosts)
                    {
                        if (GetCustomResource(c.id) < c.amount)
                        {
                            Debug.Log("Not enough " + c.id);
                            return false;
                        }
                    }
                }
                return true;
            }
            return false;
        }

        /**
         * Subtract the given number of resources.
         */
        public void RemoveResources(int resourceCost)
        {
            if (Resources >= resourceCost)
            {
                Resources -= resourceCost;
                view.SendMessage("UpdateResource", false, SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                Debug.LogWarning("Tried to buy something without enough resource");
            }
        }

        /**
         * Adds the given number of resources.
         */
        public void AddResources(int resources)
        {
            Resources += resources;
            view.SendMessage("UpdateResource", false, SendMessageOptions.DontRequireReceiver);
        }

        /**
         * Subtract the given number of gold.
         */
        public void RemoveGold(int goldCost)
        {
            if (Gold >= goldCost)
            {
                Gold -= goldCost;
                view.SendMessage("UpdateGold", false, SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                Debug.LogWarning("Tried to buy something without enough gold");
            }
        }

        /**
         * Adds the given number of gold.
         */
        public void AddGold(int gold)
        {
            Gold += gold;
            view.SendMessage("UpdateGold", false, SendMessageOptions.DontRequireReceiver);
        }

        /**
         * Adds the given number of xp.
         */
        public void AddXp(int xp)
        {
            int oldLevel = Level;
            Xp += xp;
            if (oldLevel < Level)
            {
                view.SendMessage("UpdateLevel", true, SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                view.SendMessage("UpdateLevel", false, SendMessageOptions.DontRequireReceiver);
            }
        }

        /// <summary>
        /// Gets the players level based on their experience.
        /// </summary>
        /// <returns>The level from experience.</returns>
        /// <param name="xp">Experience.</param>
        virtual public int GetLevelForExperience(int xp)
        {
            // You can override with a different equation... this is like the D&D one.
            return Mathf.FloorToInt((1.0f + Mathf.Sqrt(xp / 125.0f + 1.0f)) / 2.0f);
        }

        /// <summary>
        /// Xp required for next level.
        /// </summary>
        /// <returns>The required for next level.</returns>
        virtual public int XpRequiredForNextLevel()
        {
            // Must match GetLevelForExperience()
            return 1000 * ((Level) + Combinations((Level), 2));
        }

        /// <summary>
        /// Xp required for current level.
        /// </summary>
        /// <returns>The xp required for current level.</returns>
        virtual public int XpRequiredForCurrentLevel()
        {
            // Must match GetLevelForExperience()
            if (Level <= 1) return 0;
            return 1000 * ((Level - 1) + Combinations((Level - 1), 2));
        }


        virtual protected int Combinations(int n, int m)
        {
            float result = 1.0f;
            for (int i = 0; i < m; i++) result *= (float)(n - i) / (i + 1.0f);
            return (int)result;
        }

        public static ResourceManager Instance
        {
            get; private set;
        }


        /**
         * Load the custom resource type data from the given (file) resource.
         * 
         * @param dataFile	Name of the resource to load data from.
         * @param skipDuplicates	If false throw an exception if a duplicate is found.
         */
        virtual public void LoadResourceDataFromResource(string dataFile, bool skipDuplicates)
        {
            if (loader == null) loader = new Loader<CustomResourceType>();
            List<CustomResourceType> data = loader.Load(dataFile);
            foreach (CustomResourceType type in data)
            {
                try
                {
                    customResourceTypes.Add(type);
                    customResourceData.Add(type.id, type.defaultAmount);
                }
                catch (System.Exception ex)
                {
                    if (!skipDuplicates) throw ex;
                }
            }
        }


    }
}
