using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CBSK
{
    public class UIBuildingSelectPanel : UIGamePanel
    {

        public GameObject buildingScrollPanel;
        public GameObject buildingPanelPrefab;
        public ScrollRect scrollRect;

        private bool initialised = false;
        private List<UIBuildingSelectView> buildingSelectPanels;

        private InputManager inputManager;

        void Start()
        {
            if (!initialised)
            {
                List<BuildingTypeData> types = BuildingManager.GetInstance().GetAllBuildingTypes().Where(b => !b.isObstacle && !b.isPath).ToList();
                buildingSelectPanels = new List<UIBuildingSelectView>();
                foreach (BuildingTypeData type in types)
                {
                    AddBuildingPanel(type);
                }
                inputManager = FindObjectOfType<InputManager>();
                initialised = true;
            }

             
        }

        override public void Show()
        {
            if (activePanel != null) activePanel.Hide();
            foreach (UIBuildingSelectView p in buildingSelectPanels)
            {
                p.UpdateBuildingStatus();
            }
            StartCoroutine(DoShow());
            activePanel = this;

            //Disable zoom and drag
            if (Camera.main.orthographic)
            {
                inputManager.acceptInput = false;
            }
        }

        public override void Hide()
        {
            base.Hide();
            if (Camera.main.orthographic)
            {
                inputManager.acceptInput = true;
            }
        }

        protected override IEnumerator DoShow()
        {
            yield return StartCoroutine(base.DoShow());
            //Make sure the content is scrolled to the top
            yield return new WaitForEndOfFrame();
            scrollRect.verticalNormalizedPosition = 1;
        }

        private void AddBuildingPanel(BuildingTypeData type)
        {
            GameObject panelGo = (GameObject)Instantiate(buildingPanelPrefab);
            UIBuildingSelectView panel = panelGo.GetComponent<UIBuildingSelectView>();
            panelGo.transform.SetParent(buildingScrollPanel.transform);
            panelGo.transform.localScale = Vector3.one;
            panel.InitialiseWithBuildingType(type);
            buildingSelectPanels.Add(panel);
        }

    }
}