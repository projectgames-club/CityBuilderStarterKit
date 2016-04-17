using System.Collections;

namespace CBSK
{
    [System.Serializable]
    public class OccupantData
    {

        /**
         * Unique identifier for the occupant.
         */
        public virtual string uid { get; set; }

        /**
         * The string defining the type of this occupant
         */
        public virtual string occupantTypeString { get; set; }

        /**
         * Type reference.
         */
        [System.Xml.Serialization.XmlIgnore]
        public OccupantTypeData Type
        {
            get; set;
        }
    }
}
	
	