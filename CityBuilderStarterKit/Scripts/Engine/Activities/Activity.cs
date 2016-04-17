using UnityEngine;
using System.Collections.Generic;

/**
 * Generic class for an activity which is something
 * done over time that can be sped up with gold.
 */
namespace CBSK
{
    [System.Serializable]
    public class Activity
    {
        /**
         * Type of activity.
         */
        public string Type
        {
            get; set;
        }

        /**
         * Time activity commenced.
         */
        public System.DateTime StartTime
        {
            get; set;
        }

        /**
         * Time activity will finish.
         */
        public System.DateTime EndTime
        {
            get; set;
        }

        /**
         * Duration of activity in seconds.
         */
        public int DurationInSeconds
        {
            get; set;
        }

        /**
         * Supporting id, the first supporting id or null if none.
         */
        [System.Xml.Serialization.XmlIgnore]
        public string SupportingId
        {
            get
            {
                if (SupportingIds != null && SupportingIds.Count > 0) return SupportingIds[0];
                return null;
            }
        }

        /**
         * Supporting ids
         */
        public List<string> SupportingIds
        {
            get; set;
        }

        /**
         * Implementation that checks time based on activity type.
         */
        [System.Xml.Serialization.XmlIgnore]
        public float PercentageComplete
        {
            get
            {
                float elapsedSeconds = (int)(System.DateTime.Now - StartTime).TotalSeconds;
                float percentageComplete = elapsedSeconds / (float)DurationInSeconds;
                if (percentageComplete > 1.0f) percentageComplete = 1.0f;
                return percentageComplete;
            }
        }

        /**
         * Time left before this activity completes. 
         */
        [System.Xml.Serialization.XmlIgnore]
        public System.TimeSpan RemainingTime
        {
            get
            {
                System.TimeSpan span = EndTime - System.DateTime.Now;
                if (span.TotalSeconds < 0) return new System.TimeSpan(0);
                return span;
            }
        }

        /**
         * Is this an auto activity?
         */
        public bool IsAutoActivity
        {
            get
            {
                return (ActivityManager.GetInstance().GetActivityData(Type).automatic);
            }
        }

        public Activity()
        {

        }

        /**
         * Create a new activity and populate with the supplied values.
         */
        public Activity(string type, int durationInSeconds, System.DateTime startTime, string supportingId)
        {
            Type = type;
            DurationInSeconds = durationInSeconds;
            StartTime = startTime;
            EndTime = startTime + new System.TimeSpan(0, 0, durationInSeconds);
            SupportingIds = new List<string>();
            SupportingIds.Add(supportingId);
        }

        /**
         * Create a new activity and populate with the supplied values.
         */
        public Activity(string type, int durationInSeconds, System.DateTime startTime, List<string> supportingIds)
        {
            Type = type;
            DurationInSeconds = durationInSeconds;
            StartTime = startTime;
            EndTime = startTime + new System.TimeSpan(0, 0, durationInSeconds);
            SupportingIds = new List<string>();
            SupportingIds.AddRange(supportingIds);
        }
    }


    public class ActivityType
    {
        public const string NONE = "NONE";
        public const string BUILD = "BUILD";
        public const string GATHER = "GATHER";
        public const string AUTOMATIC = "AUTOMATIC";
        public const string CLEAR = "CLEAR";
        public const string RECRUIT = "RECRUIT";
        public const string ATTACK = "ATTACK";
    }
}