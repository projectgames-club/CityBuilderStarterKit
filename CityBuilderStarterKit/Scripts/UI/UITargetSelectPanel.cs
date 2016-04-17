using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CBSK
{
    /**
     * Panel for showing target select
     */
    public class UITargetSelectPanel : UIGamePanel
    {

        List<AttackButton> attackButtons;

        /*TODO Make the attack buttons generated at runtime via data (to allow multiplayer)
        *That will remove the need for the refresh method (and this class really), as the UpdateStatus method can be called in OnEnable on the buttons themselves
        */
        protected void Refresh()
        {
            attackButtons = FindObjectsOfType<AttackButton>().ToList();
            foreach (AttackButton ab in attackButtons)
            {
                ab.UpdateStatus();
            }
        }

        protected override IEnumerator DoShow()
        {
            Refresh();
            return base.DoShow();
        }

        protected override IEnumerator DoReShow()
        {
            Refresh();
            return base.DoReShow();
        }
    }
}