using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace CBSK
{
    /**
     * Panel for speeding up an activity by paying gold.
     */
    public class UISpeedUpPanel : UIGamePanel
    {

        public Text goldLabel;
        public Image buildingSprite;
        public Text timeLabel;
        public Image headerSprite;
        public Image headerRing;

        //private Building building;

        /**
         * Set up the building with the given building.
         */
        override public void InitialiseWithBuilding(Building building)
        {
            //	this.building = building;
            if (building.CurrentActivity != null)
            {
                BuildingManager.ActiveBuilding = building;
                timeLabel.text = string.Format("Time Remaining: {0} minutes {1} second{2}", (int)building.CurrentActivity.RemainingTime.TotalMinutes, building.CurrentActivity.RemainingTime.Seconds, (building.CurrentActivity.RemainingTime.Seconds != 1 ? "s" : ""));
                goldLabel.text = ((int)Mathf.Max(1, (float)(building.CurrentActivity.RemainingTime.TotalSeconds + 1) / (float)BuildingManager.GOLD_TO_SECONDS_RATIO)).ToString();
                buildingSprite.sprite = SpriteManager.GetBuildingSprite(building.Type.spriteName);
                headerSprite.sprite = SpriteManager.GetSprite(building.CurrentActivity.Type.ToString().ToLower() + "_icon");
                headerRing.color = UIColor.GetColourForActivityType(building.CurrentActivity.Type);
                StartCoroutine(UpdateLabels());
            }
            else
            {
                Debug.LogError("Can't speed up a building with no activity");
            }
            // Make sure we close if its zero
            if (building.CurrentActivity.RemainingTime.Seconds < 1)
            {
                UIGamePanel.ShowPanel(PanelType.DEFAULT);
            }
        }

        override public void Show()
        {
            if (activePanel == null || HasOpenPanelFlag(activePanel.panelType))
            {
                if (activePanel != null) activePanel.Hide();
                StartCoroutine(DoShow());
                activePanel = this;
            }
        }

        override public void Hide()
        {
            StopAllCoroutines();
            StartCoroutine(DoHide());
        }

        /**
         * Update the labels as time passes.
         */
        protected IEnumerator UpdateLabels()
        {
            while (BuildingManager.ActiveBuilding != null && BuildingManager.ActiveBuilding.CurrentActivity != null && BuildingManager.ActiveBuilding.CurrentActivity.RemainingTime.TotalSeconds > 1)
            {
                timeLabel.text = string.Format("Time Remaining: {0} minutes {1} second{2}", (int)BuildingManager.ActiveBuilding.CurrentActivity.RemainingTime.TotalMinutes, BuildingManager.ActiveBuilding.CurrentActivity.RemainingTime.Seconds, (BuildingManager.ActiveBuilding.CurrentActivity.RemainingTime.Seconds != 1 ? "s" : ""));
                goldLabel.text = ((int)Mathf.Max(1, (float)(BuildingManager.ActiveBuilding.CurrentActivity.RemainingTime.TotalSeconds + 1) / (float)BuildingManager.GOLD_TO_SECONDS_RATIO)).ToString();
                yield return true;
            }
            // Finished
            UIGamePanel.ShowPanel(PanelType.DEFAULT);
        }

        new protected IEnumerator DoShow()
        {
            yield return new WaitForSeconds(UI_DELAY / 3.0f);

            //uiGroup.alpha = 1;
            //uiGroup.blocksRaycasts = true;
            SetPanelActive(true);
        }

        new protected IEnumerator DoHide()
        {

            //uiGroup.alpha = 0;
            //uiGroup.blocksRaycasts = false;
            SetPanelActive(false);

            yield return true;
        }

    }
}