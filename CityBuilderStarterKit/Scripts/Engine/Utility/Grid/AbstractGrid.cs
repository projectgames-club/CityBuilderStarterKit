using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CBSK
{
    public abstract class AbstractGrid : MonoBehaviour, IGrid
    {
        /**
         * Dimensions of the grid.
         */
        public int gridSize;

        /**
         * Mapping between IGridObjects and position(s) on the grid.
         */
        protected Dictionary<IGridObject, List<GridPosition>> gridObjects;

        /**
         * The actual grid.
         */
        protected IGridObject[,] grid;

        /**
         * Return the object at the given position or null if no object
         * at the given position.
         */
        virtual public IGridObject GetObjectAtPosition(GridPosition position)
        {
            if (position.x < gridSize && position.y < gridSize && position.x >= 0 && position.y >= 0)
            {
                return grid[position.x, position.y];
            }
            return null;
        }

        virtual public void AddObjectAtPosition(IGridObject gridObject, GridPosition position)
        {

            List<GridPosition> gridPosisitons = new List<GridPosition>();
            GridPosition newPosition;
            foreach (GridPosition g in gridObject.Shape)
            {
                newPosition = position + g;
                gridPosisitons.Add(newPosition);
                grid[newPosition.x, newPosition.y] = gridObject;
            }
            gridObjects.Add(gridObject, gridPosisitons);
        }

        virtual public void RemoveObject(IGridObject gridObject)
        {
            foreach (GridPosition g in gridObjects[gridObject])
            {
                grid[g.x, g.y] = null;
            }
            gridObjects.Remove(gridObject);
        }

        virtual public void RemoveObjectAtPosition(GridPosition position)
        {
            IGridObject g = GetObjectAtPosition(position);
            if (g != null) RemoveObject(g);
        }

        virtual public bool CanObjectBePlacedAtPosition(IGridObject gridObject, GridPosition position)
        {
            GridPosition newPosition;
            foreach (GridPosition g in gridObject.Shape)
            {
                newPosition = position + g;
                if (newPosition.x < 0 || newPosition.x >= grid.GetLength(0)) return false;
                if (newPosition.y < 0 || newPosition.y >= grid.GetLength(1)) return false;
                if (grid[newPosition.x, newPosition.y] != null && grid[newPosition.x, newPosition.y] != gridObject) return false;
            }
            return true;
        }

        virtual public bool CanObjectBePlacedAtPosition(GridPosition[] shape, GridPosition position)
        {
            GridPosition newPosition;
            foreach (GridPosition g in shape)
            {
                newPosition = position + g;
                if (newPosition.x < 0 || newPosition.x >= grid.GetLength(0)) return false;
                if (newPosition.y < 0 || newPosition.y >= grid.GetLength(1)) return false;
                if (grid[newPosition.x, newPosition.y] != null) return false;
            }
            return true;
        }

        abstract public Vector3 GridPositionToWorldPosition(GridPosition position);

        abstract public Vector3 GridPositionToWorldPosition(GridPosition position, List<GridPosition> shape);

        abstract public GridPosition WorldPositionToGridPosition(Vector3 position);

        virtual public Vector3 SnapWorldPositionToGrid(Vector3 position)
        {
            return GridPositionToWorldPosition(WorldPositionToGridPosition(position));
        }
    }

}