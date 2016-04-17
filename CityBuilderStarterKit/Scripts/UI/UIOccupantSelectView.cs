using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace CBSK
{
    /**
     * User interface for an individual occupant panel. Shown when
     * selecting which occupant type to recruit.
     */
    public class UIOccupantSelectView : MonoBehaviour
    {
        public Text titleLabel;
        public Text descriptionLabel;
        public Text allowsLabel;
        public Text costLabel;
        public Text levelLabel;
        public Image sprite;
        public Image backgroundSprite;
        public ActivityButton recruitButton;

        public Sprite availableBackground;
        public Sprite unavailableBackground;

        private OccupantTypeData type;

        /**
         * Set up the occupant with the given type data.
         */
        public void InitialiseWithOccupantType(OccupantTypeData type)
        {
            this.type = type;
            titleLabel.text = type.name;
            descriptionLabel.text = type.description;
            costLabel.text = type.cost.ToString();
            sprite.sprite = SpriteManager.GetUnitSprite(type.spriteName);
            UpdateOccupantStatus();
        }

        /**
         * Updates the UI (text, buttons, etc), based on if the occupant type
         * requirements are met or not.
         */
        public void UpdateOccupantStatus()
        {
            // Level indicator
            if (type.level == 0)
            {
                levelLabel.text = "";
            }
            else if (type.level <= ResourceManager.Instance.Level)
            {
                levelLabel.text = string.Format("<color=#000000>Level {0}</color>", type.level);
            }
            else
            {
                levelLabel.text = string.Format("<color=#ff0000>Requires Level {0}</color>", type.level);
            }
            if (OccupantManager.GetInstance().CanRecruitOccupant(type.id) && BuildingManager.ActiveBuilding.CanFitOccupant(type.occupantSize))
            {
                allowsLabel.text = string.Format("<color=#000000>Allows: {0}</color>", FormatIds(type.allowIds, false));
                backgroundSprite.sprite = availableBackground;
                recruitButton.gameObject.SetActive(true);
                recruitButton.InitWithActivityType(DoActivity, ActivityType.RECRUIT, type.id);
            }
            else
            {
                if (OccupantManager.GetInstance().CanRecruitOccupant(type.id))
                {
                    allowsLabel.text = "<color=#ff0000>Not Enough Room</color>";
                    backgroundSprite.sprite = unavailableBackground;
                    recruitButton.gameObject.SetActive(false);
                }
                else
                {
                    allowsLabel.text = string.Format("<color=#000000>Requires: {0}</color>", FormatIds(type.requireIds, true));
                    backgroundSprite.sprite = unavailableBackground;
                    recruitButton.gameObject.SetActive(false);
                }
            }
        }

        /**
         * Formats the allows/required identifiers to be nice strings, coloured correctly.
         * Returns the identifiers.
         */
        private string FormatIds(List<string> allowIds, bool redIfNotPresent)
        {
            BuildingManager manager = BuildingManager.GetInstance();
            string result = "";
            foreach (string id in allowIds)
            {
                if (redIfNotPresent && !manager.PlayerHasBuilding(id))
                {
                    result += "<color=#ff0000>";
                }
                else
                {
                    result += "<color=#000000>";
                }
                BuildingTypeData type = manager.GetBuildingTypeData(id);
                if (type != null)
                {
                    result += manager.GetBuildingTypeData(id).name + "</color>, ";
                }
                else
                {
                    Debug.LogWarning("No building type data found for id:" + id);
                    result += id + "</color>, ";
                }
            }
            if (result.Length > 2)
            {
                result = result.Substring(0, result.Length - 2);
            }
            else
            {
                return "Nothing";
            }
            return result;
        }

        /**
         * Start the generic activity function.
         */
        public void DoActivity(string type, string supportingId)
        {
            if (BuildingManager.ActiveBuilding.StartActivity(type, System.DateTime.Now, supportingId))
            {
                UIGamePanel.ShowPanel(PanelType.DEFAULT);
            }
            else
            {
                Debug.LogWarning("This is where you pop up your IAP screen");
            }
        }

    }
}