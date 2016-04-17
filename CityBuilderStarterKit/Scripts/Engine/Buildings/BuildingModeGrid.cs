using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * The grid used in building mode.
 */
namespace CBSK
{
    public class BuildingModeGrid : AbstractGrid
    {

        public float gridWidth;                     // Width of the grid in world units (not grid units)
        public float gridHeight;                    // Height of the grid in world units (not grid units)
        public float gridUnusuableHeight;           // Height (from bottom AND top) that cannot be used (used to make the grid a square instead of a diamond).
        public float gridUnusuableWidth;            // Width from (left AND right) that cannot be used (used to make the grid a square instead of a diamond).
        public float gridScale = 0.003125f; //This scale is used to make the public variable numbers easier to work with. For example "64x32" grid size is much more understandable than the actual world values of ".2fx.1f"

        //public Color usableGrid;
        //public Color unusableGrid;
        //public Sprite gridBackground;

        protected static BuildingModeGrid instance; // Static reference to this grid.

        protected bool initialised = false;         // Has this grid been initialised.

        /**
         * Get the instance of the grid class or create if one has not yet been created.
         * 
         * @returns An instance of the grid class.
         */
        public static BuildingModeGrid GetInstance()
        {
            return instance;
        }

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

            gridWidth *= gridScale;
            gridHeight *= gridScale;
            gridUnusuableHeight *= gridScale;
            gridUnusuableWidth *= gridScale;

            //ShowGrid();
        }

        /**
         * Initialise the grid.
         */
        virtual protected void Init()
        {
            initialised = true;
            grid = new IGridObject[gridSize, gridSize];
            FillUnusableGrid();
            gridObjects = new Dictionary<IGridObject, List<GridPosition>>();
        }

        ////Fix issue
        ////Testing grid visibly
        //void ShowGrid()
        //{
        //    for (int y = 0; y < gridSize; y++)
        //    {
        //        for (int x = 0; x < gridSize; x++)
        //        {
        //            GameObject go = new GameObject(string.Format(string.Format("{0},{1}", x, y)));
        //            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        //            sr.sprite = gridBackground;
        //            sr.color = usableGrid;
        //            go.transform.SetParent(gameObject.transform);
        //            go.transform.localScale = Vector3.one;
        //            go.transform.position = GridPositionToWorldPosition(new GridPosition(x, y));

        //        }
        //    }
        //}


        override public Vector3 GridPositionToWorldPosition(GridPosition position)
        {
            return GridPositionToWorldPosition(position, new List<GridPosition>(GridPosition.DefaultShape));
        }

        override public Vector3 GridPositionToWorldPosition(GridPosition position, List<GridPosition> shape)
        {
            float x = (gridWidth / 2) * (position.x - position.y);
            float y = (gridHeight / 2) * (position.x + position.y);
            float sz = 9999.0f;
            float lz = -9999.0f;
            float tsz;

            // TODO Clean up and fix to cater for even odder shapes
            foreach (GridPosition pos in shape)
            {
                tsz = ((position.y + pos.y) * (gridHeight / 2)) + ((position.x + pos.x) * (gridHeight / 2)) - 2;
                if (sz >= tsz) sz = tsz;
                if (lz <= tsz) lz = tsz;
            }

            return new Vector3(x, y, ((lz + sz) / 2.0f));

        }

        override public GridPosition WorldPositionToGridPosition(Vector3 position)
        {
            int tx = Mathf.RoundToInt(((position.x / (gridWidth / 2)) + (position.y / (gridHeight / 2))) / 2);
            int ty = Mathf.RoundToInt(((position.y / (gridHeight / 2)) - (position.x / (gridWidth / 2))) / 2);
            return new GridPosition(tx, ty);
        }

        /**
         * Get the building at the given position. If the space is empty or the object
         * at the position is not a building return null.
         */
        public Building GetBuildingAtPosition(GridPosition position)
        {
            IGridObject result;
            result = GetObjectAtPosition(position);
            if (result is Building) return (Building)result;
            return null;
        }

        /**
         * Fill up the grid so the diamon becomes a rectangle.
         */
        private void FillUnusableGrid()
        {

            for (int y = 0; y < gridSize; y++)
            {
                for (int x = 0; x < gridSize; x++)
                {
                    // Fill Bottom
                    if (x + y < gridUnusuableHeight)
                    {
                        grid[x, y] = new UnusableGrid();
                    }
                    // Fill Top
                    if (x + y > (gridSize * 2) - gridUnusuableHeight)
                    {
                        grid[x, y] = new UnusableGrid();
                    }
                    // Fill Left
                    if (x - y > gridSize - gridUnusuableWidth)
                    {
                        grid[x, y] = new UnusableGrid();
                    }
                    // Fill Right
                    if (y - x > gridSize - gridUnusuableWidth)
                    {
                        grid[x, y] = new UnusableGrid();
                    }
                }
            }

            for (int y = 0; y < gridSize; y++)
            {
                for (int x = 0; x < gridSize; x++)
                {

                }
            }
            // Fill Left
            for (int y = 0; y < gridSize; y++)
            {
                for (int x = 0; x < gridSize; x++)
                {
                    if (x + y > (gridSize * 2) - gridUnusuableHeight)
                    {
                        grid[x, y] = new UnusableGrid();
                    }
                }
            }
            // Fill Right
            for (int y = 0; y < gridSize; y++)
            {
                for (int x = 0; x < gridSize; x++)
                {
                    if (x + y > (gridSize * 2) - gridUnusuableHeight)
                    {
                        grid[x, y] = new UnusableGrid();
                    }
                }
            }
        }

    }
}
