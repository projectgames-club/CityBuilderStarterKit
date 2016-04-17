using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace CBSK
{
    /**
     * UI for showing result of battle.
     */
    public class UIBattleResultsPanel : UIGamePanel
    {

        /**
         * Label to show overall result.
         */
        public Text resultLabel;

        /**
         * Label to show troop losses.
         */
        public Text lossesLabel;

        /**
         * Label to show looted gold..
         */
        public Text goldLabel;

        /**
         * Label to show looted resources.
         */
        public Text resourceLabel;

        /**
         * GameObject showing the reward (loot).
         */
        public GameObject rewardsGo;

        /**
         * Set up screen with correct values.
         */
        public void InitialiseWithResults(bool victory, string lossString, int lootedGold, int lootedResources)
        {
            if (victory)
            {
                resultLabel.text = "Victory";
                if (lossString == null || lossString.Length < 1) lossString = "None.";
                lossesLabel.text = lossString;
                rewardsGo.SetActive(true);
                resourceLabel.text = lootedResources.ToString();
                goldLabel.text = lootedGold.ToString();
            }
            else
            {
                resultLabel.text = "Defeat";
                if (lossString == null || lossString.Length < 1) lossString = "None.";
                lossesLabel.text = lossString;
                rewardsGo.SetActive(false);
            }
        }
    }
}