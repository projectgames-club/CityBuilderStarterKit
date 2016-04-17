using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace CBSK
{
    /**
     * A button which performs the attack activity.
     */
    public class AttackButton : UIButton
    {

        public string activity;
        public Image icon;
        public Image ring;
        public Image background;
        public ChooseTargetButton chooseTargetButton;

        void OnEnable()
        {
            UpdateStatus();
        }

        public void UpdateStatus()
        {
            if (OccupantManager.GetInstance().GetAllOccupants().Count < 1)
            {
                enabled = false;
                icon.color = UIColor.DESATURATE;
                ring.gameObject.SetActive(false);
                background.color = UIColor.DESATURATE;
            }
            else
            {
                enabled = true;
                icon.color = Color.white;
                ring.gameObject.SetActive(true);
                background.color = Color.white;
            }
        }

        public override void Click()
        {
            ActivityManager.GetInstance().StartActivity(activity, System.DateTime.Now, OccupantManager.GetInstance().GetAllOccupants().Select(o => o.uid).ToList());
            UIGamePanel.ShowPanel(PanelType.DEFAULT);
        }

    }
}
