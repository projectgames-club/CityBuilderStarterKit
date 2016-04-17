using UnityEngine;
using UnityEngine.UI;
using System.Collections;
namespace CBSK
{
    /**
     * Button for collecting stored resources.
     */
    public class CollectButton : UIButton
    {

        public Text resourceLabel;
        public Image icon;
        public Image ring;

        protected Building myBuilding;

        virtual public void Init(Building building)
        {
            myBuilding = building;
            icon.sprite = SpriteManager.GetSprite(building.Type.generationType.ToString().ToLower() + "_icon");
            ring.color = UIColor.GetColourForRewardType(building.Type.generationType);
            StartCoroutine(DoUpdateResourceLabel());
        }

        public override void Click()
        {
            myBuilding.Collect();
            resourceLabel.text = "" + myBuilding.StoredResources;
        }

        /**
         * Coroutine to ensure the displayed resource is up to date.
         */
        private IEnumerator DoUpdateResourceLabel()
        {
            resourceLabel.text = "" + myBuilding.StoredResources;
            while (gameObject.activeInHierarchy || myBuilding == null)
            {
                resourceLabel.text = "" + myBuilding.StoredResources;
                // Update frequently
                yield return new WaitForSeconds(0.25f);
            }
        }
    }
}