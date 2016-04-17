using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CBSK
{
    /**
     * User interface shown when a building is selected.
     */
    public class UIBuildingInfoPanel : UIGamePanel
    {

        /**
         * Collect button (which is optional depending on buildings auto activity).
         */
        public CollectButton collectButton;

        /**
         * Button for recruiting occupants.
         */
        public RecruitButton recruitButton;

        /**
         * Button for viewing occupants.
         */
        public UIButtonSwitchButton occupantButton;

        /**
         * List of buttons which are set up with a function depending on the building type.
         */
        public ActivityButton[] buttonTemplates;

        private Building building;

        /**
         * Set up the building with the given building.
         */
        override public void InitialiseWithBuilding(Building building)
        {
            BuildingManager.ActiveBuilding = building;
            this.building = building;
        }

        /**
         * Show the panel.
         */
        override protected IEnumerator DoShow()
        {
            yield return new WaitForSeconds(UI_DELAY / 3.0f);

            //uiGroup.alpha = 1;
            //uiGroup.blocksRaycasts = true;

            SetPanelActive(true);

            if (building.Type.generationAmount > 0)
            {
                collectButton.gameObject.SetActive(true);
                collectButton.Init(building);
            }
            else
            {
                collectButton.gameObject.SetActive(false);
            }
            if (OccupantManager.GetInstance().CanBuildingRecruit(building.Type.id) && building.CurrentActivity == null && building.CompletedActivity == null)
            {
                recruitButton.gameObject.SetActive(true);
            }
            else
            {
                if (recruitButton != null) recruitButton.gameObject.SetActive(false);
            }
            if (OccupantManager.GetInstance().CanBuildingHoldOccupants(building.Type.id))
            {
                occupantButton.gameObject.SetActive(true);
            }
            else
            {
                if (occupantButton != null) occupantButton.gameObject.SetActive(false);
            }
            if (building.CurrentActivity != null || building.CompletedActivity != null)
            {
                for (int i = 0; i < buttonTemplates.Length; i++)
                {
                    buttonTemplates[i].gameObject.SetActive(false);
                }
            }
            else if (buttonTemplates.Length > 0)
            {
                int i = 0;
                foreach (string activityType in building.Type.activities)
                {
                    // Add special cases for special activities here
                    buttonTemplates[i].gameObject.SetActive(true);
                    buttonTemplates[i].InitWithActivityType(DoActivity, activityType, "");
                    i++;
                }
                for (int j = i; j < buttonTemplates.Length; j++)
                {
                    buttonTemplates[j].gameObject.SetActive(false);
                }
            }
            MoveTo(showPosition);
        }


        /**
         * Reshow the panel (i.e. same panel but for a different object/building).
         */
        override protected IEnumerator DoReShow()
        {
            MoveTo(hidePosition);
            yield return new WaitForSeconds(UI_DELAY / 3.0f);
            if (building.Type.generationAmount > 0)
            {
                collectButton.gameObject.SetActive(true);
                collectButton.Init(building);
            }
            else
            {
                collectButton.gameObject.SetActive(false);
            }
            if (OccupantManager.GetInstance().CanBuildingRecruit(building.Type.id) && building.CurrentActivity == null && building.CompletedActivity == null)
            {
                recruitButton.gameObject.SetActive(true);
            }
            else
            {
                if (recruitButton != null) recruitButton.gameObject.SetActive(false);
            }
            if (OccupantManager.GetInstance().CanBuildingHoldOccupants(building.Type.id))
            {
                occupantButton.gameObject.SetActive(true);
            }
            else
            {
                if (occupantButton != null) occupantButton.gameObject.SetActive(false);
            }
            if (building.CurrentActivity != null || building.CompletedActivity != null)
            {
                for (int i = 0; i < buttonTemplates.Length; i++)
                {
                    buttonTemplates[i].gameObject.SetActive(false);
                }
            }
            else if (buttonTemplates.Length > 0)
            {
                int i = 0;
                foreach (string activityType in building.Type.activities)
                {
                    // Add special cases for special activities here
                    buttonTemplates[i].gameObject.SetActive(true);
                    buttonTemplates[i].InitWithActivityType(DoActivity, activityType, "");
                    i++;
                }
                for (int j = i; j < buttonTemplates.Length; j++)
                {
                    buttonTemplates[j].gameObject.SetActive(false);
                }

            }
            MoveTo(showPosition);
        }

        /**
         * Start the generic activty function.
         */
        public void DoActivity(string type, string supportingId)
        {
            building.StartActivity(type, System.DateTime.Now, supportingId);
            UIGamePanel.ShowPanel(PanelType.DEFAULT);
        }

        public UIBuildingInfoPanel Instance
        {
            get; private set;
        }
    }

    /**
     * Delegate type used by the buttons.
     */
    public delegate void ActivityDelegate(string type, string supportingId);
}