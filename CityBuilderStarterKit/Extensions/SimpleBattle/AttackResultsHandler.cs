using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/**
 * A simple class for working out the results of a battle activity. It handles the custom reward activity.
 */
namespace CBSK
{
    public class AttackResultsHandler : MonoBehaviour
    {
        /**
         * GameObject to send results to.
         */
        public UIBattleResultsPanel resultsPanel;

        /**
         * Handle the battle results calculations
         */
        public void CustomReward(Activity activity)
        {
            List<string> losses = new List<string>();
            int goldRewarded = 0;
            int resourcesRewarded = 0;
            ActivityData type = ActivityManager.GetInstance().GetActivityData(activity.Type);
            if (type is AttackActivityData)
            {
                int troopStrength = 0;
                foreach (string s in activity.SupportingIds)
                {
                    OccupantData o = OccupantManager.GetInstance().GetOccupant(s);
                    if (o != null && o.Type is AttackerOccupantTypeData)
                    {
                        troopStrength += ((AttackerOccupantTypeData)o.Type).attack;
                    }
                }
                // Calculate result
                bool winner = false;
                if (troopStrength >= ((AttackActivityData)type).strength * 2)
                {
                    winner = true;
                    // No losses
                }
                else if (troopStrength >= ((AttackActivityData)type).strength)
                {
                    if (Random.Range(0, 3) != 0) winner = true;
                    // 25% chance of losing each unit
                    losses.AddRange(activity.SupportingIds.Where(o => Random.Range(0, 4) == 0).ToList());
                    // Ensure at least one troop member survives
                    if (losses.Count == activity.SupportingIds.Count) losses.RemoveAt(0);
                }
                else if (troopStrength >= (int)(((AttackActivityData)type).strength * 0.5f))
                {
                    // Calculate losses
                    if (Random.Range(0, 3) == 0) winner = true;
                    // 50% chance of losing each unit
                    losses.AddRange(activity.SupportingIds.Where(o => Random.Range(0, 2) == 0).ToList());
                    // Ensure at least one troop member survives
                    if (losses.Count == activity.SupportingIds.Count) losses.RemoveAt(0);
                }
                else
                {
                    // Lose everyone
                    losses.AddRange(activity.SupportingIds);
                }

                // Calculate reward
                if (winner)
                {
                    goldRewarded = Random.Range(0, type.rewardAmount + 1);
                    resourcesRewarded = Random.Range(1, type.rewardAmount + 1) * 100;
                }

                // Remove occupants
                string lossesString = "";
                foreach (string o in losses)
                {
                    lossesString += OccupantManager.GetInstance().GetOccupant(o).Type.name + ", ";
                }
                foreach (string o in losses)
                {
                    OccupantManager.GetInstance().DismissOccupant(o);
                }
                if (lossesString.Length > 0) lossesString.Substring(0, lossesString.Length - 2);

                // Add rewards
                ResourceManager.Instance.AddResources(resourcesRewarded);
                ResourceManager.Instance.AddGold(goldRewarded);

                // Show panel - 
                UIBattleResultsPanel panel = (UIBattleResultsPanel)FindObjectOfType(typeof(UIBattleResultsPanel));
                if (panel != null)
                {
                    panel.InitialiseWithResults(winner, lossesString, goldRewarded, resourcesRewarded);
                    UIGamePanel.ShowPanel(PanelType.BATTLE_RESULTS);
                }
            }
        }
    }
}
