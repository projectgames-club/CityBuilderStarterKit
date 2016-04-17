using UnityEngine;
using System.Collections;

namespace CBSK
{
    /**
     * Button which saves paths and exits path mode
     */
    public class SavePathsButton : UIButton
    {

        public override void Click()
        {
            PathManager.GetInstance().ExitPathMode();
            UIGamePanel.ShowPanel(PanelType.DEFAULT);
        }
    }
}
