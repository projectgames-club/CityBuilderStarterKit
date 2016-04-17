using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CBSK
{
    public class UnusableGrid : IGridObject
    {

        /*
         * Position of the object on the grid measured from the bottom left corner.
         */
        public GridPosition Position
        {
            get { return new GridPosition(0, 0); }
            set { }
        }

        /*
         * New position of the object on the grid measured from the bottom left corner.
         * Used when the obejct is being placed
         */
        public GridPosition NewPosition
        {
            get { return new GridPosition(0, 0); }
            set { }
        }

        /*
         * Shape of the grid object defined by offsets from the base position of 0,0.
         */
        public List<GridPosition> Shape
        {
            get { return new List<GridPosition> { new GridPosition(0, 0) }; }
        }

        /*
         * Get rid of this object and any attached game objects, etc.
         */
        public void Dispose()
        {

        }

    }
}
