using UnityEngine;
using System.Collections;


namespace CBSK
{
    /**
     * Button to show the recruit panel
     */
    public class RecruitButton : UIButton
    {
        public override void Click()
        {
            UIGamePanel.ShowPanel(PanelType.RECRUIT_OCCUPANTS);
        }
    }
}
