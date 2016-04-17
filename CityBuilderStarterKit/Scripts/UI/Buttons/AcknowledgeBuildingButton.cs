using UnityEngine;
using System.Collections;

namespace CBSK
{
    /**
     * Acknowledge building turning it from ready to built.
     */
    public class AcknowledgeBuildingButton : UIButton
    {

        public Building building;

        public override void Click()
        {
            if (building.State == BuildingState.READY)
            {
                BuildingManager.GetInstance().AcknowledgeBuilding(building);
            }
            else if (building.State == BuildingState.IN_PROGRESS || building.CurrentActivity != null)
            {
                UIGamePanel.ShowPanel(PanelType.SPEED_UP);
                if (UIGamePanel.activePanel is UISpeedUpPanel) ((UISpeedUpPanel)UIGamePanel.activePanel).InitialiseWithBuilding(building);
            }
            else
            {
                building.AcknowledgeActivity();
            }
        }
    }
}