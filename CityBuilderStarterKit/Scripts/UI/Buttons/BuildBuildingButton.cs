using UnityEngine;
using System.Collections;

namespace CBSK
{
    /**
     * Button on the select building panel which starts the
     * PLACING process.
     */
    public class BuildBuildingButton : UIButton
    {

        private string buildingTypeId;

        public void Init(string buildingTypeId)
        {
            this.buildingTypeId = buildingTypeId;
        }

        public override void Click()
        {
            if (ResourceManager.Instance.CanBuild(BuildingManager.GetInstance().GetBuildingTypeData(buildingTypeId)))
            {
                BuildingManager.GetInstance().CreateBuilding(buildingTypeId);
                UIGamePanel.ShowPanel(PanelType.PLACE_BUILDING);
            }
            else
            {
                Debug.LogWarning("This is where you bring up your in app purchase screen");
            }
        }
    }
}
