using UnityEngine;
using System.Collections;

/**
 * An occupant that has a attack and defensive value.
 */
namespace CBSK
{
    [System.Serializable]
    public class AttackerOccupantTypeData : OccupantTypeData
    {
        public int attack;
        public int defense;
    }
}

