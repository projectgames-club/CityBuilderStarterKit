using System.Collections.Generic;
namespace CBSK
{
    [System.Serializable]
    public class BuildingData
    {

        /**
         * Unique identifier for the buidling.
         */
        public virtual string uid { get; set; }

        /**
         * The string defining the type of this building.
         */
        public virtual string buildingTypeString { get; set; }

        /**
         * State of the building.
         */
        public virtual BuildingState state { get; set; }

        /**
         * Current position of the building
         */
        public virtual GridPosition position { get; set; }

        /**
         * Time the building started building
         */
        public virtual System.DateTime startTime { get; set; }

        /**
         * Activities currently in progress or null if nothing in progress.
         */
        public virtual Activity currentActivity { get; set; }

        /**
         * Reward activity currently in progress or null if theres no automatic reward activity {get; set;}
         */
        public virtual Activity autoActivity { get; set; }

        /**
         * The completed activity awaiting acknowledgement or NONE if no activity completed.
         */
        public virtual Activity completedActivity { get; set; }

        /**
         * The number of auto generated resources stored in this building, ready to be collected.
         */
        public virtual int storedResources { get; set; }

        /**
         * List of all occupants in this building.
         */
        public virtual List<OccupantData> occupants { get; set; }

        override public string ToString()
        {
            return "Building(" + uid + "): " + state + " " + startTime.ToString() + " " + currentActivity;
        }
    }
}