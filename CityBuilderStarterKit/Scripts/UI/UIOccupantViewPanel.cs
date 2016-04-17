using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CBSK
{
    /**
     * Panel for viewing the occupants of a building.
     */
    public class UIOccupantViewPanel : UIGamePanel
    {

        public GameObject occupantScrollPanel;
        public GameObject occupantPanelPrefab;
        public ScrollRect scrollRect;

        private bool initialised = false;
        private List<UIOccupantView> occupantViews;

        private InputManager inputManager;

        void Start()
        {
            inputManager = FindObjectOfType<InputManager>();
        }

        override public void InitialiseWithBuilding(Building building)
        {
            if (!initialised)
            {
                List<OccupantData> data = building.Occupants;
                occupantViews = new List<UIOccupantView>();
                if (data != null)
                {
                    foreach (OccupantData o in data)
                    {
                        AddOccupantPanel(o, false);
                    }
                }
                if ((building.CurrentActivity != null && building.CurrentActivity.Type == ActivityType.RECRUIT) || ((building.CompletedActivity != null && building.CompletedActivity.Type == ActivityType.RECRUIT)))
                {
                    OccupantData no = new OccupantData();
                    no.Type = OccupantManager.GetInstance().GetOccupantTypeData(building.CurrentActivity.SupportingId);
                    AddOccupantPanel(no, true);
                    // TODO Coroutine to allow constant update of this panel (or maybe it should be in the panel itself?)
                }
                initialised = true;
            }
        }

        override public void Show()
        {
            if (activePanel == this)
            {
                StartCoroutine(ClearThenReshow());
            }
            else
            {
                if (activePanel != null) activePanel.Hide();
                StartCoroutine(ClearThenReshow());
                StartCoroutine(DoShow());
                activePanel = this;
            }

            //Disable zoom and drag
            if (Camera.main.orthographic)
            {
                inputManager.acceptInput = false;
            }
        }

        protected override IEnumerator DoShow()
        {
            yield return StartCoroutine(base.DoShow());
            //Make sure the content is scrolled to the top
            yield return new WaitForEndOfFrame();
            scrollRect.verticalNormalizedPosition = 1;
        }

        protected IEnumerator ClearThenReshow()
        {
            if (occupantViews != null)
            {
                foreach (UIOccupantView o in occupantViews)
                {
                    Destroy(o.gameObject);
                }
            }
            yield return true;
            initialised = false;
            InitialiseWithBuilding(BuildingManager.ActiveBuilding);
        }

        public override void Hide()
        {
            base.Hide();
            if (Camera.main.orthographic)
            {
                inputManager.acceptInput = true;
            }
        }

        private void AddOccupantPanel(OccupantData data, bool inProgress)
        {
            GameObject panelGo = (GameObject)Instantiate(occupantPanelPrefab);
            UIOccupantView panel = panelGo.GetComponent<UIOccupantView>();
            panelGo.transform.SetParent(occupantScrollPanel.transform);
            panelGo.transform.localScale = Vector3.one;
            panel.InitialiseWithOccupant(data, inProgress);
            occupantViews.Add(panel);
        }
    }
}