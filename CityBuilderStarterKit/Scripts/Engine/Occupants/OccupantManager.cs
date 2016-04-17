using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/**
 * Manages things inside a building. Occupants are stored as part of the building but the type data is managed from here.
 * This class also tracks references to the individual occupants to allow for easy management/searching/etc.
 */
namespace CBSK
{
    public class OccupantManager : Manager<OccupantManager>
    {

        /**
         * A list of data files (resources) containing building data
         */
        public List<string> occupantDataFiles;

        /**
         * Occupant types mapped to ids.
         */
        private Dictionary<string, OccupantTypeData> types;

        /**
         * Loader for loading the data.
         */
        private Loader<OccupantTypeData> loader;

        /**
         * Individual Occupants mapped to ids.
         */
        private Dictionary<string, OccupantData> occupants;

        /**
         * Initialise the instance.
         */
        override protected void Init()
        {
            types = new Dictionary<string, OccupantTypeData>();
            occupants = new Dictionary<string, OccupantData>();
            if (occupantDataFiles != null)
            {
                foreach (string dataFile in occupantDataFiles)
                {
                    LoadOccupantDataFromResource(dataFile, false);
                }
            }
            initialised = true;
        }

        /**
         * Get a list of each occupant type.
         */
        public List<OccupantTypeData> GetAllOccupantTypes()
        {
            return types.Values.ToList();
        }

        /**
         * Get a list of all occupants.
         */
        public List<OccupantData> GetAllOccupants()
        {
            return occupants.Values.ToList();
        }


        /**
         * Get occupant with given id, or returns null if not found.
         */
        public OccupantData GetOccupant(string id)
        {
            if (occupants.ContainsKey(id)) return occupants[id];
            return null;
        }


        /**
         * Load the building type data from the given resource.
         * 
         * @param dataFile	Name of the resource to load data from.
         * @param skipDuplicates	If false throw an exception if a duplicate is found.
         */
        public void LoadOccupantDataFromResource(string dataFile, bool skipDuplicates)
        {
            if (loader == null) loader = new Loader<OccupantTypeData>();
            List<OccupantTypeData> data = loader.Load(dataFile);
            foreach (OccupantTypeData type in data)
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
         * Return the type data for the given building id. Returns null if the building type is not found.
         */
        public OccupantTypeData GetOccupantTypeData(string id)
        {
            if (types.ContainsKey(id))
            {
                return types[id];
            }
            return null;
        }

        /**
         * Return true if the player has at least one occupant of the given type.
         */
        public bool PlayerHasOccupant(string id)
        {
            if (occupants.Values.Where(b => b.Type.id == id).Count() > 0) return true;
            return false;
        }

        /**
         * Return true if player can build the given occupant. Excludes
         * resource costs as we can pop up an IAP purchase window here.
         */
        public bool CanRecruitOccupant(string occupantTypeId)
        {
            if (types[occupantTypeId].level > ResourceManager.Instance.Level) return false;
            foreach (string id in types[occupantTypeId].requireIds)
            {
                if (!PlayerHasOccupant(id) && !BuildingManager.GetInstance().PlayerHasBuilding(id)) return false;
            }
            return true;
        }

        /**
         * Returns true if the given building type (defined by id) can recruit any kind of occupant.
         */
        public bool CanBuildingRecruit(string buildingTypeId)
        {
            if (types.Values.Where(t => t.recruitFromIds.Contains(buildingTypeId)).Count() > 0) return true;
            return false;
        }

        /**
         * Returns true if the given building type (defined by id) can house any kind of occupant.
         */
        public bool CanBuildingHoldOccupants(string buildingTypeId)
        {
            if (types.Values.Where(t => t.housingIds.Contains(buildingTypeId)).Count() > 0) return true;
            return false;
        }

        /**
         * Returns true if the given building type (defined by id) can house the given occupant id.
         */
        public bool CanBuildingHoldOccupant(string buildingTypeId, string occupantTypeId)
        {
            if (types.ContainsKey(occupantTypeId) && types[occupantTypeId].housingIds.Contains(buildingTypeId)) return true;
            return false;
        }

        /**
         * Recruited an occupant.
         */
        public void RecruitedOccupant(OccupantData data)
        {
            occupants.Add(data.uid, data);
        }

        /**
         * Dismiss an occupant by ID, search all building to ensure they are removed. This should
         * only be used when the occupant cant be dismissed at the building level as it is more expensive.
         */
        public void DismissOccupant(string id)
        {
            if (occupants.ContainsKey(id))
            {
                OccupantData data = occupants[id];
                List<Building> buildings = BuildingManager.GetInstance().GetAllBuildings();
                foreach (Building b in buildings)
                {
                    if (b.Occupants != null && b.Occupants.Contains(data))
                    {
                        b.DismissOccupant(data);
                        break;
                    }
                }
            }
        }

        /**
         * Called when a bulding dismissed an occupant.
         */
        public void DismissedOccupant(OccupantData data)
        {
            if (data != null && occupants.ContainsKey(data.uid))
            {
                occupants.Remove(data.uid);
            }
            else
            {
                Debug.LogWarning("Dismissed an occupant that wasn't found");
            }
        }
    }
}