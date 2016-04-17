using UnityEngine;
using System.Collections;

namespace CBSK
{
    public class DoSellBuildingButton : UIButton
    {

        public bool isGold = false;

        public override void Click()
        {
            if (isGold)
            {
                BuildingManager.GetInstance().SellBuildingForGold(BuildingManager.ActiveBuilding);
            }
            else
            {
                BuildingManager.GetInstance().SellBuilding(BuildingManager.ActiveBuilding, false);
            }
            UIGamePanel.ShowPanel(PanelType.DEFAULT);
            BuildingManager.ActiveBuilding = null;
        }
    }
}
