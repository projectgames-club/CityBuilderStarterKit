using System.Collections.Generic;

namespace CBSK
{
    [System.Serializable]

    /**
     * Data for the type of an occupant. Public variables are used for simplicity of serialization
     * but should not be modified directly.
     */
    public class OccupantTypeData
    {

        public virtual string id { get; set; }                      // Unique id of the occupant.
        public virtual string name { get; set; }                        // Human readable name of the occupant.
        public virtual string description { get; set; }             // A human readable description of the occupant.
        public virtual string spriteName { get; set; }              // The name of the sprite used to represent this occupant.

        public virtual int level { get; set; }                      // Level required to build.

        public virtual int cost { get; set; }                           // How many resources it costs to build this occupant.
        public virtual int buildTime { get; set; }                  // How long in seconds it takes to build this occupant.

        public virtual List<string> recruitFromIds { get; set; }        // Ids of the building where this occupant is recruited/trained/built.
        public virtual List<string> allowIds { get; set; }          // Ids of the objects that this occupant allows.
        public virtual List<string> requireIds { get; set; }            // Ids of the objects required before this occupant can be built.
        public virtual List<string> housingIds { get; set; }            // Ids of the building where this occupant can reside.

        public virtual int generationBoost { get; set; }                // Amount of additional reward to generate each time interval when this occupant is in a building.

        public virtual int occupantSize { get; set; }                   // How much occupant storage space does this occupant take.
    }
}