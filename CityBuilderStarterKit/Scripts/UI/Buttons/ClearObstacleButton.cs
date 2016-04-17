using UnityEngine;
using System.Collections;

namespace CBSK
{
    /**
     * A button which can perform the clear activity.
     */
    public class ClearObstacleButton : CollectButton
    {

        /**
         * Set up the visuals.
         */
        override public void Init(Building building)
        {
            myBuilding = building;
            resourceLabel.text = "" + building.Type.cost;
        }

        /**
         * Button clicked, start activity.
         */
        override public void Click()
        {
            if (ResourceManager.Instance.Resources > myBuilding.Type.cost)
            {
                myBuilding.StartActivity(ActivityType.CLEAR, System.DateTime.Now, "");
                UIGamePanel.ShowPanel(PanelType.DEFAULT);
            }
        }
    }
}
