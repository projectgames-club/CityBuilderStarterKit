/**
 * Extends building data by including a building level which can be upgraded.
 */
namespace CBSK
{
    [System.Serializable]
    public class UpgradableBuildingData : BuildingData
    {

        /**
         * The buildings current level.
         */
        virtual public int level { get; set; }
    }
}