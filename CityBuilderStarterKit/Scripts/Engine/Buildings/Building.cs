using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Represents a building in the game. 
 */
namespace CBSK
{
    public class Building : MonoBehaviour, IGridObject
    {
        /**
         * Wrapper holding the data for this building that is persisted.
         */
        protected BuildingData data;

        virtual public BuildingData BuildingData
        {
            get { return data; }
        }

        /**
         * View for this building.
         */
        protected GameObject view;

        /**
         * Unique identifier for the buidling.
         */
        virtual public string uid
        {
            get { return data.uid; }
            set { data.uid = value; }
        }

        protected BuildingTypeData type;

        /**
         * The data defining the type of this building.
         */
        virtual public BuildingTypeData Type
        {
            get { return type; }
            protected set
            {
                type = value;
                data.buildingTypeString = value.id;
            }
        }

        /**
         * State of the building.
         */
        virtual public BuildingState State
        {
            get { return data.state; }
            protected set { data.state = value; }
        }


        /**
         * Shape of the building in terms of the grid positions it fills. 
         */
        virtual public List<GridPosition> Shape
        {
            get
            {
                return Type.shape;
            }
        }


        /**
         * Current position of the building
         */
        virtual public GridPosition Position
        {
            get
            {
                return data.position;
            }
            set
            {
                data.position = value;
                MovePosition = value;
            }
        }

        /**
         * Position the building may be moved to.
         */
        virtual public GridPosition MovePosition
        {
            get; set;
        }

        /**
         * Time the building started building
         */
        virtual public System.DateTime StartTime
        {
            get { return data.startTime; }
            protected set { data.startTime = value; }
        }

        /**
         * Activities currently in progress or null if nothing in progress.
         */
        virtual public Activity CurrentActivity
        {
            get { return data.currentActivity; }
            protected set { data.currentActivity = value; }
        }

        /**
         * Reward activity currently in progress or null if theres no automatic reward activity;
         */
        virtual public Activity AutoActivity
        {
            get { return data.autoActivity; }
            protected set { data.autoActivity = value; }
        }

        /**
         * The completed activity awaiting acknowledgement or NONE if no activity completed.
         */
        virtual public Activity CompletedActivity
        {
            get { return data.completedActivity; }
            protected set { data.completedActivity = value; }
        }

        /**
         * The number of auto generated resources stored in this building, ready to be collected.
         */
        virtual public int StoredResources
        {
            get { return data.storedResources; }
            protected set { data.storedResources = value; }
        }

        /**
         * Returns true if something other than an automatic activity is in progress but not complete.
         */
        virtual public bool ActivityInProgress
        {
            get
            {
                if (CurrentActivity != null || CompletedActivity != null) return true;
                return false;
            }
        }

        /**
         * Returns true if the store is full.
         */
        virtual public bool StoreFull
        {
            get
            {
                if (type.generationAmount > 0 && StoredResources >= type.generationStorage) return true;
                return false;
            }
        }

        /**
         * Gets a list of the buildings occupants. WARNING: This is not a copy.
         */
        virtual public List<OccupantData> Occupants
        {
            get { return data.occupants; }
        }

        /**
         * Initialise the building with the given type and position.
         */
        virtual public void Init(BuildingTypeData type, GridPosition pos)
        {
            data = new BuildingData();
            uid = System.Guid.NewGuid().ToString();
            Position = pos;
            this.Type = type;
            State = BuildingState.PLACING;

            CurrentActivity = null;
            CompletedActivity = null;
            view = gameObject;
            view.SendMessage("UI_Init", this);
            view.SendMessage("SetPosition", data.position);
        }

        /**
         * Initialise the building with the given data
         */
        virtual public void Init(BuildingTypeData type, BuildingData data)
        {
            StartCoroutine(DoInit(type, data));
        }

        /**
         * Used on obstacles to start the clearing activity.
         */
        virtual public void StartClear()
        {
            StartCoroutine(GenericActivity(ActivityType.CLEAR, System.DateTime.Now, ""));
        }

        /**
         * Create a building from data. Uses a coroutine to ensure view can be synced with data.
         */
        protected virtual IEnumerator DoInit(BuildingTypeData type, BuildingData data)
        {
            this.data = data;
            this.type = type;
            this.Position = data.position;
            // Ensure occupant type references are loaded
            if (data.occupants != null)
            {
                foreach (OccupantData o in data.occupants)
                {
                    o.Type = OccupantManager.GetInstance().GetOccupantTypeData(o.occupantTypeString);
                    OccupantManager.GetInstance().RecruitedOccupant(o);
                }
            }
            // Update view
            view = gameObject;
            view.SendMessage("UI_Init", this);
            view.SendMessage("SetPosition", data.position);
            view.SendMessage("UI_UpdateState");

            // Wait one frame to ensure everything is initialised
            yield return true;
            if (data.state == BuildingState.IN_PROGRESS || data.state == BuildingState.READY)
            {
                StartCoroutine(BuildActivity(data.startTime));
            }
            else
            {
                // Activities
                if (data.completedActivity != null && data.completedActivity.Type != ActivityType.BUILD && data.completedActivity.Type != ActivityType.NONE)
                {
                    StartCoroutine(GenericActivity(data.completedActivity.Type, data.completedActivity.StartTime, data.completedActivity.SupportingId));
                }
                else if (data.currentActivity != null && data.currentActivity.Type != ActivityType.BUILD && data.currentActivity.Type != ActivityType.NONE)
                {
                    StartCoroutine(GenericActivity(data.currentActivity.Type, data.currentActivity.StartTime, data.currentActivity.SupportingId));
                }
                // Auto activities
                if (!Type.isObstacle)
                {
                    if (data.autoActivity != null)
                    {
                        System.TimeSpan span = System.DateTime.Now - data.autoActivity.StartTime;
                        int iterations = ((int)span.TotalSeconds) / type.generationTime;
                        int generated = iterations * type.generationAmount;
                        StoredResources = StoredResources + generated;
                        if (StoredResources > type.generationStorage) StoredResources = type.generationStorage;
                        // If storage not full resume activity
                        if (type.generationAmount > 0 && StoredResources < type.generationStorage)
                        {
                            System.DateTime newStartTime = data.autoActivity.StartTime + new System.TimeSpan(0, 0, iterations * type.generationTime);
                            StartAutomaticActivity(newStartTime);
                        }
                        else
                        {
                            AutoActivity = null;
                        }
                    }
                    else if (type.generationAmount > 0 && StoredResources < type.generationStorage)
                    {
                        // Start auto activty if one not saved
                        StartAutomaticActivity(System.DateTime.Now);
                    }
                    // If more than 50% full and no other activity to show then show a store collect indicator
                    if (type.generationAmount > 0 && CompletedActivity == null && CurrentActivity == null && StoredResources >= type.generationStorage * 0.5f)
                    {
                        view.SendMessage("UI_StoreFull");
                    }
                }
            }
        }

        /**
         * Place the building.
         */
        virtual public void Place()
        {
            State = BuildingState.IN_PROGRESS;
            StartTime = System.DateTime.Now;
            view.SendMessage("UI_UpdateState");
            StartCoroutine(BuildActivity(System.DateTime.Now));
        }

        /**
         * Acknowledge the building.
         */
        virtual public void Acknowledge()
        {
            State = BuildingState.BUILT;
            view.SendMessage("UI_UpdateState");
            if (!Type.isObstacle && Type.generationAmount > 0)
            {
                StartAutomaticActivity(System.DateTime.Now);
            }
            CurrentActivity = null;
            CompletedActivity = null;
        }

        /**
         * Speed up activity for a certian amount of gold.
         */
        virtual public void SpeedUp()
        {
            if (CurrentActivity == null)
            {
                Debug.LogError("Tried to speed up but no activity in progress");
                return;
            }
            if (CurrentActivity.Type == ActivityType.BUILD)
            {
                CompletedActivity = CurrentActivity;
                CurrentActivity = null;
                // Acknowledge();	
            }
            else
            {
                CompletedActivity = CurrentActivity;
                CurrentActivity = null;
                AcknowledgeActivity();
            }
        }

        /**
         * Start generic activity and subtract any cost.
         * Returns true if cost can be paid, otherwise returns false and doesn't start the activity.h
         */
        virtual public bool StartActivity(string type, System.DateTime startTime, string supportingId)
        {
            switch (type)
            {
                case ActivityType.CLEAR:
                    if (ResourceManager.Instance.Resources < this.type.cost) return false;
                    ResourceManager.Instance.RemoveResources(this.type.cost);
                    break;
                case ActivityType.RECRUIT:
                    OccupantTypeData odata = OccupantManager.GetInstance().GetOccupantTypeData(supportingId);
                    if (ResourceManager.Instance.Resources < odata.cost) return false;
                    ResourceManager.Instance.RemoveResources(odata.cost);
                    break;
                default:
                    ActivityData data = ActivityManager.GetInstance().GetActivityData(type);
                    if (data != null && ResourceManager.Instance.Resources < data.activityCost) return false;
                    ResourceManager.Instance.RemoveResources(data.activityCost);
                    break;
            }
            StartCoroutine(GenericActivity(type, startTime, supportingId));
            return true;
        }


        /**
         *  Start an automatic activity.
         */
        virtual public void StartAutomaticActivity(System.DateTime startTime)
        {
            StartCoroutine(AutomaticActivity(startTime));
        }


        /**
         *  Acknowledge generic activity.
         */
        virtual public void AcknowledgeActivity()
        {
            if (CompletedActivity != null && CompletedActivity.Type == ActivityType.CLEAR)
            {
                ResourceManager.Instance.AddXp(ActivityManager.GetInstance().GetXpForCompletingActivity(CompletedActivity));
                CurrentActivity = null;
                CompletedActivity = null;
                BuildingManager.GetInstance().ClearObstacle(this);
                view.SendMessage("UI_AcknowledgeActivity");
            }
            else if (CompletedActivity != null && CompletedActivity.Type == ActivityType.RECRUIT)
            {
                ResourceManager.Instance.AddXp(ActivityManager.GetInstance().GetXpForCompletingActivity(CompletedActivity));
                CreateOccupant(CompletedActivity.SupportingId);
                CurrentActivity = null;
                CompletedActivity = null;
                view.SendMessage("UI_AcknowledgeActivity");
                if ((int)BuildingManager.GetInstance().saveMode < (int)SaveMode.SAVE_NEVER) PersistenceManager.GetInstance().Save();
            }
            else if (CompletedActivity != null && CompletedActivity.Type != ActivityType.NONE && CompletedActivity.Type != ActivityType.AUTOMATIC)
            {
                ResourceManager.Instance.AddXp(ActivityManager.GetInstance().GetXpForCompletingActivity(CompletedActivity));
                ActivityData data = ActivityManager.GetInstance().GetActivityData(CompletedActivity.Type);
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
                            SendMessage("CustomReward", CompletedActivity, SendMessageOptions.RequireReceiver);
                            break;
                    }
                    CurrentActivity = null;
                    CompletedActivity = null;
                    view.SendMessage("UI_AcknowledgeActivity");
                    if ((int)BuildingManager.GetInstance().saveMode < (int)SaveMode.SAVE_NEVER) PersistenceManager.GetInstance().Save();
                }
                else
                {
                    Debug.LogError("Couldn't find data for activity: " + CompletedActivity.Type);
                }
            }
            else if (StoredResources > 0)
            {
                Collect();
            }
        }

        /**
         * Create a new occupant and add it to this building
         */
        protected void CreateOccupant(string supportingId)
        {
            OccupantData occupant = new OccupantData();
            occupant.uid = System.Guid.NewGuid().ToString();
            occupant.Type = OccupantManager.GetInstance().GetOccupantTypeData(supportingId);
            occupant.occupantTypeString = occupant.Type.id;
            if (data.occupants == null) data.occupants = new List<OccupantData>();
            data.occupants.Add(occupant);
            OccupantManager.GetInstance().RecruitedOccupant(occupant);
        }

        /**
         * Sets the view position back to the buildings position
         */
        virtual public void ResetPosition()
        {
            view.SendMessage("SetPosition", Position);
        }

        /**
         * Start moving the building.
         */
        virtual public void StartMoving()
        {
            State = BuildingState.MOVING;
            view.SendMessage("UI_UpdateState");
        }

        /**
         * Start moving the building.
         */
        virtual public void FinishMoving()
        {
            State = BuildingState.BUILT;
            view.SendMessage("UI_UpdateState");
        }

        /**
         * Collect stored resources.
         */
        virtual public void Collect()
        {
            switch (Type.generationType)
            {
                case RewardType.GOLD:
                    ResourceManager.Instance.AddGold(StoredResources);
                    break;
                case RewardType.RESOURCE:
                    ResourceManager.Instance.AddResources(StoredResources);
                    break;
            }
            StoredResources = 0;
            if (!(ActivityInProgress)) view.SendMessage("UI_AcknowledgeActivity");
            // If the store was full restart the auto activity
            if (AutoActivity == null) StartAutomaticActivity(System.DateTime.Now);
            if ((int)BuildingManager.GetInstance().saveMode < (int)SaveMode.SAVE_NEVER) PersistenceManager.GetInstance().Save();
        }


        /**
         * Start building the building.
         */
        protected IEnumerator BuildActivity(System.DateTime startTime)
        {
            Activity activity;
            activity = new Activity(ActivityType.BUILD, Type.buildTime, startTime, "");
            CurrentActivity = activity;
            view.SendMessage("UI_StartActivity", activity);
            while (CurrentActivity != null && activity.PercentageComplete < 1.0f)
            {
                // Check more frequently as time gets closer, but never more frequently than once per second
                yield return new WaitForSeconds(Mathf.Max(1.0f, (float)activity.RemainingTime.TotalSeconds / 15.0f));
                view.SendMessage("UI_UpdateProgress", activity);
            }
            // If this wasn't triggered by a speed up
            if (CurrentActivity != null)
            {
                CurrentActivity = null;
                CompleteBuild();
            }
        }

        /**
         * Start a generic activity.
         */
        protected IEnumerator GenericActivity(string type, System.DateTime startTime, string supportingId)
        {
            if (type == ActivityType.AUTOMATIC || type == ActivityType.NONE)
            {
                Debug.LogError("Unexpected activity type: " + type);
            }
            else if (type == ActivityType.CLEAR)
            {
                Activity activity = new Activity(type, this.type.buildTime, startTime, supportingId);
                CurrentActivity = activity;
                if ((int)BuildingManager.GetInstance().saveMode < (int)SaveMode.SAVE_NEVER) PersistenceManager.GetInstance().Save();
                view.SendMessage("UI_StartActivity", activity);
                while (CurrentActivity != null && CurrentActivity.PercentageComplete < 1.0f)
                {
                    view.SendMessage("UI_UpdateProgress", activity);
                    // Check more frequently as time gets closer, but never more frequently than once per second
                    yield return new WaitForSeconds(Mathf.Max(1.0f, (float)CurrentActivity.RemainingTime.TotalSeconds / 15.0f));
                }
                // If this wasn't triggered by a speed-up
                if (CurrentActivity != null)
                {
                    CompletedActivity = CurrentActivity;
                    CurrentActivity = null;
                    if ((int)BuildingManager.GetInstance().saveMode < (int)SaveMode.SAVE_MOSTLY) PersistenceManager.GetInstance().Save();
                    view.SendMessage("UI_CompleteActivity", type);
                }
            }
            else if (type == ActivityType.RECRUIT)
            {
                Activity activity = new Activity(type, OccupantManager.GetInstance().GetOccupantTypeData(supportingId).buildTime, startTime, supportingId);
                CurrentActivity = activity;
                if ((int)BuildingManager.GetInstance().saveMode < (int)SaveMode.SAVE_NEVER) PersistenceManager.GetInstance().Save();
                view.SendMessage("UI_StartActivity", activity);
                while (CurrentActivity != null && CurrentActivity.PercentageComplete < 1.0f)
                {
                    view.SendMessage("UI_UpdateProgress", activity);
                    // Check more frequently as time gets closer, but never more frequently than once per second
                    yield return new WaitForSeconds(Mathf.Max(1.0f, (float)CurrentActivity.RemainingTime.TotalSeconds / 15.0f));
                }
                // If this wasn't triggered by a speed-up
                if (CurrentActivity != null)
                {
                    CompletedActivity = CurrentActivity;
                    CurrentActivity = null;
                    if ((int)BuildingManager.GetInstance().saveMode < (int)SaveMode.SAVE_MOSTLY) PersistenceManager.GetInstance().Save();
                    view.SendMessage("UI_CompleteActivity", type);
                }
            }
            else
            {
                ActivityData data = ActivityManager.GetInstance().GetActivityData(type);
                Activity activity = new Activity(type, data.durationInSeconds, startTime, "");
                CurrentActivity = activity;
                if ((int)BuildingManager.GetInstance().saveMode < (int)SaveMode.SAVE_NEVER) PersistenceManager.GetInstance().Save();
                view.SendMessage("UI_StartActivity", activity);
                while (CurrentActivity != null && CurrentActivity.PercentageComplete < 1.0f)
                {
                    view.SendMessage("UI_UpdateProgress", activity);
                    // Check more frequently as time gets closer, but never more frequently than once per second
                    yield return new WaitForSeconds(Mathf.Max(1.0f, (float)CurrentActivity.RemainingTime.TotalSeconds / 15.0f));
                }
                // If this wasn't triggered by a speed-up
                if (CurrentActivity != null)
                {
                    CompletedActivity = CurrentActivity;
                    CurrentActivity = null;
                    if ((int)BuildingManager.GetInstance().saveMode < (int)SaveMode.SAVE_MOSTLY) PersistenceManager.GetInstance().Save();
                    view.SendMessage("UI_CompleteActivity", type);
                }
            }
        }

        /**
         * Do automatic activity.
         */
        protected IEnumerator AutomaticActivity(System.DateTime startTime)
        {
            AutoActivity = new Activity(ActivityType.AUTOMATIC, Type.generationTime, startTime, "");
            while (StoredResources < Type.generationStorage)
            {
                yield return new WaitForSeconds((float)AutoActivity.RemainingTime.TotalSeconds);
                StoredResources += Type.generationAmount;
                if (StoredResources > Type.generationStorage) StoredResources = Type.generationStorage;
                AutoActivity = new Activity(ActivityType.AUTOMATIC, Type.generationTime, System.DateTime.Now, "");
            }
            if (StoredResources > Type.generationStorage) StoredResources = Type.generationStorage;
            AutoActivity = null;
            view.SendMessage("UI_StoreFull");
        }

        /**
         * Returns true if there is enough room in this building to fit the given occupant.
         */
        virtual public bool CanFitOccupant(int occupantSize)
        {
            int currentSize = 0;
            if (data.occupants != null)
            {
                foreach (OccupantData o in data.occupants)
                {
                    currentSize += OccupantManager.GetInstance().GetOccupantTypeData(o.occupantTypeString).occupantSize;
                }
            }
            if ((currentSize + occupantSize) <= type.occupantStorage) return true;
            return false;
        }

        /**
         * Dismiss (remove) the occupant. Returns true if occupant found and removed, otherwise false;
         */
        virtual public bool DismissOccupant(OccupantData occupant)
        {
            if (data.occupants != null)
            {
                if (data.occupants.Contains(occupant))
                {
                    data.occupants.Remove(occupant);
                    OccupantManager.GetInstance().DismissedOccupant(occupant);
                    view.SendMessage("UI_DismissOccupant", SendMessageOptions.DontRequireReceiver);
                    return true;
                }
            }
            return false;
        }

        /**
         * Finish building.
         */
        virtual public void CompleteBuild()
        {
            State = BuildingState.READY;
            view.SendMessage("UI_UpdateState");
        }

    }
}
