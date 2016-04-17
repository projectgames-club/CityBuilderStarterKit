using UnityEngine;

/**
 * A building which can be upgraded.
 */
namespace CBSK
{
    public class UpgradableBuilding : Building
    {


        /**
         * Initialise the building with the given type and position.
         */
        override public void Init(BuildingTypeData type, GridPosition pos)
        {
            data = new UpgradableBuildingData();
            ((UpgradableBuildingData)data).level = 1;
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
         *  Acknowledge generic activity.
         */
        override public void AcknowledgeActivity()
        {
            if (!((this.data) is UpgradableBuildingData))
            {
                Debug.LogWarning("Upgradable buildings should use upgradable building data");
                return;
            }
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
                            ResourceManager.Instance.AddResources(data.rewardAmount * ((UpgradableBuildingData)this.data).level);
                            break;
                        case RewardType.GOLD:
                            ResourceManager.Instance.AddGold(data.rewardAmount * ((UpgradableBuildingData)this.data).level);
                            break;
                        case RewardType.CUSTOM_RESOURCE:
                            ResourceManager.Instance.AddCustomResource(data.rewardId, data.rewardAmount);
                            break;
                        case RewardType.CUSTOM:
                            // You need to include a custom reward handler if you use the CUSTOM RewardType
                            if (data.type == "UPGRADE")
                            {
                                ((UpgradableBuildingData)this.data).level += 1;
                            }
                            else
                            {
                                SendMessage("CustomReward", CompletedActivity, SendMessageOptions.RequireReceiver);
                            }
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
    }
}