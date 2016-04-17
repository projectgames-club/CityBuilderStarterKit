using UnityEngine;
using System.Collections;

namespace CBSK
{
    /**
     * Cancel placing the building on the map and give the
     * resources back.
     */
    public class CancelBuildingButton : UIButton
    {

        public override void Click()
        {
            if (BuildingManager.ActiveBuilding != null)
            {
                // Moving
                if (BuildingManager.ActiveBuilding.State == BuildingState.MOVING)
                {
                    BuildingManager.ActiveBuilding.ResetPosition();
                    BuildingManager.ActiveBuilding.FinishMoving();
                    UIGamePanel.ShowPanel(PanelType.DEFAULT);
                }
                else
                {
                    // Placing
                    if (BuildingManager.GetInstance().SellBuilding(BuildingManager.ActiveBuilding, true))
                    {
                        UIGamePanel.ShowPanel(PanelType.DEFAULT);
                    }
                }
            }
        }
    }
}