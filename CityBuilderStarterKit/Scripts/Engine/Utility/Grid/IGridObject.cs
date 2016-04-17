using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CBSK
{
    /**
     * Interface for any object that can be positioned on the grid
     */
    public interface IGridObject
    {
        List<GridPosition> Shape { get; }
        GridPosition Position { get; set; }
    }
}
