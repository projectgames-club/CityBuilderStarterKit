using System.Collections.Generic;
using UnityEngine;

/**
 * Extends building data by including a position at which units should be drawn
 */
namespace CBSK
{
    [System.Serializable]
    public class BuildingDataWithUnitAnimations : BuildingTypeData
    {

        /**
         * The relative positions to place the animations. Ordered so first position is for first unit, second for second, and so on.
         */
        virtual public List<Vector2> animationPositions { get; set; }


        /**
         * The relative positions to place the static sprites. Ordered so first position is for first unit, second for second, and so on.
         */
        virtual public List<Vector2> staticPositions { get; set; }

    }
}