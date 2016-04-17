using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CBSK
{
    public class UIButtonSwitchButton : UIButton
    {

        public PanelType type;

        public override void Click()
        {
            UIGamePanel.ShowPanel(type);
        }
    }
}
