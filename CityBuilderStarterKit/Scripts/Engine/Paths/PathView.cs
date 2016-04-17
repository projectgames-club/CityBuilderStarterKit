using UnityEngine;
using System.Collections;

/**
 * Simplified building view used for paths.
 */
namespace CBSK
{
    public class PathView : BuildingView
    {

        /**
         * Initialise the path view.
         */
        override public void UI_Init(Building building)
        {
            this.building = building;
            //widget = buildingSprite;
            buildingSprite.color = Color.white;
            buildingSprite.sprite = SpriteManager.GetBuildingSprite(building.Type.spriteName + PathManager.GetInstance().GetSpriteSuffix(building));
            myPosition = transform.localPosition;
            SnapToGrid();
            //Vector3 position = grid.GridPositionToWorldPosition(building.Position);
            //widget.depth = 999 - (int)position.z;
        }

        /**
         * Update view
         */
        override public void UI_UpdateState()
        {
            // Make sure sprite matches
            buildingSprite.sprite = SpriteManager.GetBuildingSprite(building.Type.spriteName + PathManager.GetInstance().GetSpriteSuffix(building));
        }


        /**
         * Update objects position
         */
        override public void SetPosition(GridPosition pos)
        {
            Vector3 position = grid.GridPositionToWorldPosition(pos);
            //widget.depth = 999 - (int)position.z;
            position.z = target.localPosition.z;
            target.localPosition = position;
            myPosition = target.localPosition;
        }

        /**
         * Don't allow color change for paths.
         */
        override protected void SetColor(GridPosition pos)
        {

        }

        /**
         * Can we drag this object.
         */
        override public bool CanDrag
        {
            get
            {
                // Paths cant be dragged
                return false;
            }
            set
            {
                // Do nothing
            }
        }

        /**
         * After object dragged set color
         */
        override protected void PostDrag(GridPosition pos)
        {

        }
    }
}