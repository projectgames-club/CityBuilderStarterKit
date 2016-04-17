using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CBSK
{
    /**
     * Panel for selecting the occupant to recruit.
     */
    public class UIOccupantSelectPanel : UIGamePanel
    {

        public GameObject occupantScrollPanel;
        public GameObject occupantPanelPrefab;
        public ScrollRect scrollRect;

        private bool initialised = false;
        private List<UIOccupantSelectView> occupantSelectPanels;

        private InputManager inputManager;

        void Start()
        {
            inputManager = FindObjectOfType<InputManager>();
        }

        override public void InitialiseWithBuilding(Building building)
        {
            if (!initialised)
            {
                List<OccupantTypeData> types = OccupantManager.GetInstance().GetAllOccupantTypes().Where(o => o.recruitFromIds.Contains(building.Type.id)).ToList();
                occupantSelectPanels = new List<UIOccupantSelectView>();
                foreach (OccupantTypeData type in types)
                {
                    AddOccupantPanel(type);
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
            if (occupantSelectPanels != null)
            {
                foreach (UIOccupantSelectView o in occupantSelectPanels)
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

        private void AddOccupantPanel(OccupantTypeData type)
        {
            GameObject panelGo = (GameObject)Instantiate(occupantPanelPrefab);
            UIOccupantSelectView panel = panelGo.GetComponent<UIOccupantSelectView>();
            panelGo.transform.SetParent(occupantScrollPanel.transform);
            panelGo.transform.localScale = Vector3.one;
            panel.InitialiseWithOccupantType(type);
            occupantSelectPanels.Add(panel);
        }
    }
}