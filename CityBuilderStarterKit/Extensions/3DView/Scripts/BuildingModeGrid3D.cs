using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * The grid used in building mode for the 3D view.
 */
namespace CBSK
{
    public class BuildingModeGrid3D : BuildingModeGrid
    {
        /**
         * If there is already a grid destroy self, else initialise and assign to the static reference.
         */
        void Awake()
        {
            if (instance == null)
            {
                if (!initialised) Init();
                instance = (BuildingModeGrid)this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        /**
         * Initialise the grid.
         */
        override protected void Init()
        {
            initialised = true;
            grid = new IGridObject[gridSize, gridSize];
            FillUnusableGrid();
            gridObjects = new Dictionary<IGridObject, List<GridPosition>>();
        }


        override public Vector3 GridPositionToWorldPosition(GridPosition position)
        {
            return GridPositionToWorldPosition(position, new List<GridPosition>(GridPosition.DefaultShape));
        }

        override public Vector3 GridPositionToWorldPosition(GridPosition position, List<GridPosition> shape)
        {
            // TODO Add shape handler
            return new Vector3(position.x * gridWidth, 0, position.y * gridHeight);

        }

        override public GridPosition WorldPositionToGridPosition(Vector3 position)
        {
            return new GridPosition((int)((position.x) / gridWidth), (int)((position.z) / gridHeight));
        }


        /**
         * Fill up the grid.
         */
        protected void FillUnusableGrid()
        {
            // No need for unusable grid in 3D view as its already a rectangle.
            if (gridUnusuableHeight > 0 || gridUnusuableWidth > 0) Debug.LogWarning("Unusable grid is ignored in 3D mode");
        }

    }
}
