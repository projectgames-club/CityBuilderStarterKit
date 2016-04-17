using UnityEngine;
using System.Collections;

namespace CBSK
{
    public class MoveBuildingButton : UIButton
    {
        public override void Click()
        {
            BuildingManager.ActiveBuilding.StartMoving();
            UIGamePanel.ShowPanel(PanelType.PLACE_BUILDING);
        }
    }
}