using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace CBSK
{
    /**
     * Controls a battle button
     */
	public class UIBattleControlButton : UIButton
    {

        public Image icon;
        public Image backgroundRing;
        public Image ring;
        public Text label;

        public ChooseTargetButton actualButton;

        /**
         * Activity completed.
         */
        public void UI_StartActivity(Activity activity)
        {
            StopCoroutine("DoBobble");
            icon.color = UIColor.DESATURATE;
            backgroundRing.fillAmount = activity.PercentageComplete;
            ring.fillAmount = activity.PercentageComplete;
        }


        /**
         * Indicate progress on the progress ring.
         */
        public void UI_UpdateProgress(Activity activity)
        {
            ring.fillAmount = activity.PercentageComplete;
            backgroundRing.fillAmount = activity.PercentageComplete;
        }

        /**
         * Activity completed.
         */
        public void UI_CompleteActivity(string type)
        {
            icon.color = Color.white;
            ring.fillAmount = 1.0f;
            backgroundRing.fillAmount = 1.0f;
            StartCoroutine("DoBobble");
        }

        /**
         * Activity acknowledged.
         */
        public void UI_AcknowledgeActivity()
        {
            StopCoroutine("DoBobble");
            icon.color = Color.white;
            ring.fillAmount = 1.0f;
            backgroundRing.fillAmount = 1.0f;
        }

        private IEnumerator DoBobble()
        {
            while (actualButton.gameObject.activeInHierarchy)
            {
                iTween.PunchPosition(actualButton.gameObject, new Vector3(0, 0.035f, 0), 1.5f);
                yield return new WaitForSeconds(Random.Range(1.0f, 1.5f));
            }
        }

    }
}