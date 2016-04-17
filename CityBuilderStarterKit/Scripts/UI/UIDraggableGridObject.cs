using UnityEngine;
using System.Collections;

namespace CBSK
{
    public class UIDraggableGridObject : MonoBehaviour
    {

        /**
         * The grid to snap to.
         */
        protected AbstractGrid grid;

        /**
         * Cached transform
         */
        protected Transform target;

        /**
         * World position of the grid object.
         */
        protected Vector3 myPosition;


        /**
         * Internal initialisation.
         */
        void Awake()
        {
            target = transform;
            myPosition = target.position;
            grid = GameObject.FindObjectOfType(typeof(AbstractGrid)) as AbstractGrid;
        }

        /**
         * Update objects position
         */
        virtual public void SetPosition(GridPosition pos)
        {
            Vector3 position = grid.GridPositionToWorldPosition(pos);
            target.localPosition = position;
            myPosition = target.localPosition;
        }

        /**
        * Call to drag
        */
        virtual public void OnDrag(Vector2 delta)
        {

        }

        /**
         * Called after each drag.
         */
        virtual protected void PostDrag(GridPosition pos)
        {

        }

        /**
         * Snap object to grid. 
         */
        virtual protected GridPosition SnapToGrid()
        {
            GridPosition pos = grid.WorldPositionToGridPosition(myPosition);
            Vector3 position = grid.GridPositionToWorldPosition(pos);
            position.z = target.localPosition.z;
            target.localPosition = position;
            return pos;
        }

        /**
         * Can we drag this object.
         */
        virtual public bool CanDrag
        {
            get; set;
        }
    }
}