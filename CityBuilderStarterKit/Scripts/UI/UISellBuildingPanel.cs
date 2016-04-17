using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace CBSK
{
    /**
     * Panel for selling a building.
     */
    public class UISellBuildingPanel : UIGamePanel
    {

        public Text goldLabel;
        public Text resourceLabel;
        public Image buildingSprite;
        public Text messageLabel;
        public GameObject sellForGoldButton;

        /**
         * Set up the building with the given building.
         */
        override public void InitialiseWithBuilding(Building building)
        {
            resourceLabel.text = ((int)building.Type.cost * BuildingManager.RECLAIM_PERCENTAGE).ToString();
            goldLabel.text = ((int)Mathf.Max(1.0f, (int)(building.Type.cost * BuildingManager.GOLD_SELL_PERCENTAGE))).ToString();
            buildingSprite.sprite = SpriteManager.GetBuildingSprite(building.Type.spriteName);
            messageLabel.text = string.Format("         Are you sure you want to sell your {0} for {1} resources?", building.Type.name, (BuildingManager.GOLD_SELL_PERCENTAGE <= 0 ? "" : "gold or "));
            if (BuildingManager.GOLD_SELL_PERCENTAGE <= 0) sellForGoldButton.SetActive(false);
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
            StartCoroutine(DoHide());
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