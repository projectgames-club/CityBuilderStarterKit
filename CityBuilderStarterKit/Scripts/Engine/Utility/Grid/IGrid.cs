using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CBSK
{
    /**
     * Interface implemented by anything maintaining a grid.
     */
    public interface IGrid
    {

        /**
         * Returns the object at the given position.
         */
        IGridObject GetObjectAtPosition(GridPosition position);

        /**
         * Adds the given object at the given position.
         */
        void AddObjectAtPosition(IGridObject gridObject, GridPosition position);

        /**
         * Removes the given object.
         */
        void RemoveObject(IGridObject gridObject);

        /**
         * Removes whatever object is at the given position.
         */
        void RemoveObjectAtPosition(GridPosition position);

        /**
         * Returns true if the given object can be placed at the given grid position. Returns false
         * if another grid object is in the way. The definition of "in the way" may be different for different
         * grids.
         */
        bool CanObjectBePlacedAtPosition(IGridObject gridObject, GridPosition position);

        /**
         * Returns the grid position in world co-ordiantes. World coordinates may vary between implementations.
         */
        Vector3 GridPositionToWorldPosition(GridPosition position);

        Vector3 GridPositionToWorldPosition(GridPosition position, List<GridPosition> shape);

        /**
         * Returns grid position nearest to the world coordiantes
         */
        GridPosition WorldPositionToGridPosition(Vector3 position);

        /**
         * Returns the world position snapped to the grid
         */
        Vector3 SnapWorldPositionToGrid(Vector3 position);

    }

}