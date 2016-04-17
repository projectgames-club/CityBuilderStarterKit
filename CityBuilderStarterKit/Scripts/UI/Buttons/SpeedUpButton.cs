using UnityEngine;
using System.Collections;

namespace CBSK
{
    /**
     * Button which speeds up a building or activity.
     */
    public class SpeedUpButton : UIButton
    {

        public override void Click()
        {
            BuildingManager.GetInstance().SpeedUp();
            UIGamePanel.ShowPanel(PanelType.DEFAULT);
        }
    }
}