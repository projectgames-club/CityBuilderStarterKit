using UnityEngine.UI;

namespace CBSK
{
    /**
     * A button which can perform any activity type.
     */
    public class ActivityButton : UIButton
    {

        public Image icon;
        public Image ring;
        public Text label;

        private ActivityDelegate activity;
        private string activityType;
        private string supportingId;

        /**
         * Set up the delegate and visuals.
         */
        public void InitWithActivityType(ActivityDelegate activity, string activityType, string supportingId)
        {
            this.activity = activity;
            this.activityType = activityType;
            this.supportingId = supportingId;
            if (activityType != ActivityType.RECRUIT)
            {
                icon.sprite = SpriteManager.GetSprite(activityType.ToString().ToLower() + "_icon");

                ring.color = UIColor.GetColourForActivityType(activityType);
            }
            string labelString = activityType.ToString();
            labelString.Replace("_", " ");
            labelString = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(labelString);
            if (label != null)
            {
                label.text = labelString;
            }
        }

        public override void Click()
        {
            activity(activityType, supportingId);
        }
    }
}
