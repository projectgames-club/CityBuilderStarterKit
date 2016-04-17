using UnityEngine;
using System.Collections.Generic;
namespace CBSK
{
    /**
     * Button for choosing battle target or acknoledging battle complete.
     */
    public class ChooseTargetButton : UIButton
    {
        public override void Click()
        {
            List<Activity> activities = ActivityManager.GetInstance().GetActivitiesOfDataClassType(typeof(AttackActivityData));

            if (activities == null || activities.Count == 0)
            {
                UIGamePanel.ShowPanel(PanelType.TARGET_SELECT);
            }
            else
            {
                // This button only handles one activity
                if (activities[0].PercentageComplete >= 1)
                {
                    ActivityManager.GetInstance().AcknowledgeActivity(activities[0]);
                }
            }
        }
    }
}