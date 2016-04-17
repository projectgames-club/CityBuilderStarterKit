using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace CBSK
{
    /**
     * User interface for an individual buildings panel. Shown when
     * selecting which building type to build.
     */
    public class UIBuildingSelectView : MonoBehaviour
    {
        public Text titleLabel;
        public Text descriptionLabel;
        public Text allowsLabel;
        public Text costLabel;
        public Text levelLabel;
        public Image sprite;
        public Image backgroundSprite;
        public BuildBuildingButton buildButton;
        public Image extraCostSprite;
        public Text extraCostLabel;

        public Sprite availableBackground;
        public Sprite unavailableBackground;

        private BuildingTypeData type;

        /**
         * Set up the building with the given type data.
         */
        virtual public void InitialiseWithBuildingType(BuildingTypeData type)
        {
            this.type = type;
            titleLabel.text = type.name;
            descriptionLabel.text = type.description;
            costLabel.text = type.cost.ToString();
            sprite.sprite = SpriteManager.GetBuildingSprite(type.spriteName);
            if (type.additionalCosts != null && type.additionalCosts.Count > 0)
            {
                extraCostLabel.gameObject.SetActive(true);
                extraCostSprite.gameObject.SetActive(true);
                extraCostLabel.text = "" + type.additionalCosts[0].amount;
                extraCostSprite.sprite = SpriteManager.GetSprite(ResourceManager.Instance.GetCustomResourceType(type.additionalCosts[0].id).spriteName);
            }
            else
            {
                extraCostLabel.gameObject.SetActive(false);
                extraCostSprite.gameObject.SetActive(false);
            }
            UpdateBuildingStatus();
        }

        /**
         * Updates the UI (text, buttons, etc), based on if the building type
         * requirements are met or not.
         */
        virtual public void UpdateBuildingStatus()
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

            // We don't check for the amount of resources here, becaue we want to prompt users
            // to buy resources. However you could add a check for resources if you preferred.
            // To do this check that ResourceManager.Instance.Resource >= type.cost.
            if (BuildingManager.GetInstance().CanBuildBuilding(type.id))
            {
                allowsLabel.text = string.Format("<color=#000000>Allows: {0}</color>", FormatIds(type.allowIds, false));
                backgroundSprite.sprite = availableBackground;
                buildButton.gameObject.SetActive(true);
                buildButton.Init(type.id);
            }
            else
            {
                allowsLabel.text = string.Format("<color=#ff0000>Requires: {0}</color>", FormatIds(type.requireIds, true));
                backgroundSprite.sprite = unavailableBackground;
                buildButton.gameObject.SetActive(false);
            }
        }

        /**
         * Formats the allows/required identifiers to be nice strings, coloured correctly.
         * Returns the identifiers.
         * "HTML like" tags are used to format Rich Text in the Unity UI system.
         */
        virtual protected string FormatIds(List<string> allowIds, bool redIfNotPresent)
        {
            BuildingManager manager = BuildingManager.GetInstance();
            string result = "";
            foreach (string id in allowIds)
            {
                if (redIfNotPresent && !manager.PlayerHasBuilding(id) && !OccupantManager.GetInstance().PlayerHasOccupant(id))
                {
                    result += "<color=#ff0000>";
                }
                else
                {
                    result += "<color=#000000>";
                }
                BuildingTypeData type = manager.GetBuildingTypeData(id);
                OccupantTypeData otype = OccupantManager.GetInstance().GetOccupantTypeData(id);
                if (type != null)
                {
                    result += type.name + "</color>, ";
                }
                else if (otype != null)
                {
                    result += otype.name + "</color>, ";
                }
                else
                {
                    Debug.LogWarning("No building or occupant type data found for id:" + id);
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
    }

}