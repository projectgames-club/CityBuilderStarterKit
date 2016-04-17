using UnityEngine;
using System.Collections;

namespace CBSK
{
    public class SellBuildingButton : UIButton
    {

        public override void Click()
        {
            UIGamePanel.ShowPanel(PanelType.SELL_BUILDING);
            ((UISellBuildingPanel)UIGamePanel.activePanel).InitialiseWithBuilding(BuildingManager.ActiveBuilding);
        }
    }
}