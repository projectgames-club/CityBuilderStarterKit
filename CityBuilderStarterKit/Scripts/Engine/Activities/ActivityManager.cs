using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/**
 * Manages activity data (activity types) as well as activities that are not tied to a building.
 */
namespace CBSK
{
    public class ActivityManager : Manager<ActivityManager>
    {

        /**
         * A gameobject which will recieve messages about the view.
         */
        public GameObject view;

        /**
         * A list of activity files (resources) to load.
         */
        public List<string> activityDataFiles;

        /**
         * Activity types mapped to ids.
         */
        private Dictionary<string, ActivityData> types;

        /**
         * Loader for loading the data.
         */
        private Loader<ActivityData> loader;

        /**
         * Activities currently in progress;
         */
        virtual protected List<Activity> currentActivities { get; set; }

        /**
         * The completed activities awaiting acknowledgement;
         */
        virtual protected List<Activity> completedActivities { get; set; }

        /**
         * Initialise the instance.
         */
        override protected void Init()
        {
            types = new Dictionary<string, ActivityData>();

            if (activityDataFiles != null)
            {
                foreach (string dataFile in activityDataFiles)
                {
                    LoadActivityDataFromResource(dataFile, false);
                }
            }
            currentActivities = new List<Activity>();
            completedActivities = new List<Activity>();
            initialised = true;
        }

        /**
         * Get a list of each building type.
         */
        virtual public List<ActivityData> GetAllBuildingTypes()
        {
            return types.Values.ToList();
        }

        /**
         * Load the activity type data from the given resource.
         * 
         * @param dataFile	Name of the resource to load data from.
         * @param skipDuplicates	If false throw an exception if a duplicate is found.
         */
        virtual public void LoadActivityDataFromResource(string dataFile, bool skipDuplicates)
        {
            if (loader == null) loader = new Loader<ActivityData>();
            List<ActivityData> data = loader.Load(dataFile);
            foreach (ActivityData type in data)
            {
                try
                {
                    types.Add(type.type, type);
                }
                catch (System.Exception ex)
                {
                    if (!skipDuplicates) throw ex;
                }
            }
        }

        /**
         * Return the  data for the given activity type. Returns null if the activity type is not found.
         */
        virtual public ActivityData GetActivityData(string type)
        {
            if (types.ContainsKey(type))
            {
                return types[type];
            }
            return null;
        }

        /**
         * Return the data for a save game (i.e. all current and completed activity data).
         */
        virtual public List<Activity> GetSaveData()
        {
            List<Activity> result = new List<Activity>();
            result.AddRange(completedActivities);
            result.AddRange(currentActivities);
            return result;
        }

        /**
         * Loads the saved game data.
         */
        virtual public void Load(SaveGameData data)
        {
            StartCoroutine(DoLoad(data));
        }

        /**
         * Does the loading of a saved game
         */
        virtual protected IEnumerator DoLoad(SaveGameData data)
        {
            // Wait one frame to ensure everything is initialised
            yield return true;
            // Activities
            if (data.activities != null)
            {
                foreach (Activity a in data.activities)
                {
                    if (a.RemainingTime.TotalSeconds <= 0)
                    {
                        completedActivities.Add(a);
                        view.SendMessage("UI_CompleteActivity", a.Type);
                    }
                    else
                    {
                        StartCoroutine(GenericActivity(a));
                    }
                }
            }
        }
        /**
         * Start generic activity and subtract any cost.
         * Returns the activity if cost can be paid, otherwise returns null and doesn't start the activity.
         */
        virtual public Activity StartActivity(string type, System.DateTime startTime, List<string> supportingIds)
        {
            ActivityData data = GetActivityData(type);
            if (data != null && ResourceManager.Instance.Resources < data.activityCost) return null;
            Activity activity = new Activity(type, data.durationInSeconds, startTime, supportingIds);
            ResourceManager.Instance.RemoveResources(data.activityCost);
            StartCoroutine(GenericActivity(activity));
            return activity;
        }

        /**
         *  Acknowledge generic activity.
         */
        virtual public void AcknowledgeActivity(Activity activity)
        {
            if (completedActivities.Contains(activity))
            {
                ActivityData data = GetActivityData(activity.Type);
                if (data != null)
                {
                    switch (data.reward)
                    {
                        case RewardType.RESOURCE:
                            ResourceManager.Instance.AddResources(data.rewardAmount);
                            break;
                        case RewardType.GOLD:
                            ResourceManager.Instance.AddGold(data.rewardAmount);
                            break;
                        case RewardType.CUSTOM_RESOURCE:
                            ResourceManager.Instance.AddCustomResource(data.rewardId, data.rewardAmount);
                            break;
                        case RewardType.CUSTOM:
                            // You need to include a custom reward handler if you use the CUSTOM RewardType
                            SendMessage("CustomReward", activity, SendMessageOptions.RequireReceiver);
                            break;
                    }
                    completedActivities.Remove(activity);
                    view.SendMessage("UI_AcknowledgeActivity");
                    ResourceManager.Instance.AddXp(GetXpForCompletingActivity(data));
                    if ((int)BuildingManager.GetInstance().saveMode < (int)SaveMode.SAVE_NEVER) PersistenceManager.GetInstance().Save();
                }
                else
                {
                    Debug.LogError("Couldn't find data for activity: " + activity.Type);
                }
            }
        }

        /**
         * Check for an activity by one of the supporting Id's and return the activity if found
         * or null if not found. Note this is not a copy of the activity. Checks both current
         * and completed activites.
         */
        virtual public Activity CheckForActivityById(string id)
        {
            foreach (Activity activity in currentActivities)
            {
                if (activity.SupportingIds.Contains(id)) return activity;
            }
            foreach (Activity activity in completedActivities)
            {
                if (activity.SupportingIds.Contains(id)) return activity;
            }
            return null;
        }

        /**
         * Find all activities which have data of a given class. Useful for finding custom activity types.
         */
        virtual public List<Activity> GetActivitiesOfDataClassType(System.Type type)
        {
            List<Activity> result = new List<Activity>();
            foreach (Activity activity in currentActivities)
            {
                ActivityData data = GetActivityData(activity.Type);
                if (type.IsAssignableFrom(data.GetType())) result.Add(activity);
            }
            foreach (Activity activity in completedActivities)
            {
                ActivityData data = GetActivityData(activity.Type);
                if (type.IsAssignableFrom(data.GetType())) result.Add(activity);
            }
            return result;
        }

        /**
         * Start a generic activity.
         */
        virtual protected IEnumerator GenericActivity(Activity activity)
        {
            currentActivities.Add(activity);
            if ((int)BuildingManager.GetInstance().saveMode < (int)SaveMode.SAVE_NEVER) PersistenceManager.GetInstance().Save();
			view.SendMessage("UI_StartActivity", activity, SendMessageOptions.DontRequireReceiver);
            while (activity != null && activity.PercentageComplete < 1.0f)
            {
                view.SendMessage("UI_UpdateProgress", activity, SendMessageOptions.DontRequireReceiver);
                // Check more frequently as time gets closer, but never more frequently than once per second
                yield return new WaitForSeconds(Mathf.Max(1.0f, (float)activity.RemainingTime.TotalSeconds / 15.0f));
            }
            // If this wasn't triggered by a speed-up
            if (currentActivities.Contains(activity))
            {
                completedActivities.Add(activity);
                currentActivities.Remove(activity);
                if ((int)BuildingManager.GetInstance().saveMode < (int)SaveMode.SAVE_MOSTLY) PersistenceManager.GetInstance().Save();
				view.SendMessage("UI_CompleteActivity", activity.Type, SendMessageOptions.DontRequireReceiver);
            }
        }

        /// <summary>
        /// Gets the xp for completing an activity with the provided activity data.
        /// </summary>
        /// <returns>The xp for completing the activity.</returns>
        /// <param name="activity">Activity.</param>
        virtual public int GetXpForCompletingActivity(ActivityData data)
        {
            // Obivously pretty simple you might want to use data files or a lookup table
            if (data != null)
            {
                return data.rewardAmount + data.durationInSeconds;
            }
            return 0;
        }

        /// <summary>
        /// Gets the xp for completing the provided activity.
        /// </summary>
        /// <returns>The xp for completing the activity.</returns>
        /// <param name="activity">Activity.</param>
        virtual public int GetXpForCompletingActivity(Activity activity)
        {
            return GetXpForCompletingActivity(GetActivityData(activity.Type));
        }
    }
}