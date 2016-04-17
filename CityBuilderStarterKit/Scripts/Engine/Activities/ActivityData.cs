
/**
 * Data class for activity data.
 */
namespace CBSK
{
    [System.Serializable]
    public class ActivityData
    {

        /**
         * Type of activity. This is a string but certain values defined in Activity 
         */
        public virtual string type { get; set; }

        /**
         * Human readable description of the activity.
         */
        public virtual string description { get; set; }

        /**
         * How long it takes to complete the activity. -1 is used
         * for special activities like training troops which have
         * custom implementations.
         */
        public virtual int durationInSeconds { get; set; }

        /**
         * What you get for completing the activity.
         */
        public virtual RewardType reward { get; set; }

        /**
         * How much you get for completing the activity.
         */
        public virtual int rewardAmount { get; set; }

        /**
         * Id of the item rewarded for rewards like mobiles or research.
         */
        public virtual string rewardId { get; set; }

        /**
         * If this is true the action doesn't have a menu item. Instead it automatically triggers itself. 
         */
        public virtual bool automatic { get; set; }

        /**
         * Cost of this activity
         */
        public virtual int activityCost { get; set; }

    }
}