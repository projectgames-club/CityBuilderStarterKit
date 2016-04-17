using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace CBSK
{
    public class UIResourceView : MonoBehaviour
    {

        public Text resourceLabel;

        public Text goldLabel;

        public Text levelLabel;

        public Image xpSprite;

        // We could use a collection here but this is adequate to illustrate how you can handle more types

        public Text customResource1Label;

        public Image customResource1Sprite;

        public Text customResource2Label;

        public Image customResource2Sprite;

        private int displayedResources;
        private int displayedGold;
        private int displayedCustomResources1;
        private int displayedCustomResources2;
        private string customResourceType1;
        private string customResourceType2;

        void Start()
        {
            displayedResources = ResourceManager.Instance.Resources;
            resourceLabel.text = displayedResources.ToString();
            displayedGold = ResourceManager.Instance.Gold;
            goldLabel.text = displayedGold.ToString();
            List<CustomResourceType> resources = ResourceManager.Instance.GetCustomResourceTypes();
            if (resources.Count == 0)
            {
                customResource1Sprite.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                customResource1Sprite.transform.parent.gameObject.SetActive(true);
                customResource1Sprite.sprite = SpriteManager.GetButtonSprite(resources[0].spriteName);
                displayedCustomResources1 = ResourceManager.Instance.GetCustomResource(resources[0].id);
                customResource1Label.text = displayedCustomResources1.ToString();
                customResourceType1 = resources[0].id;
            }
            if (resources.Count < 2)
            {
                customResource2Sprite.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                customResource2Sprite.transform.parent.gameObject.SetActive(true);
                customResource2Sprite.sprite = SpriteManager.GetButtonSprite(resources[1].spriteName);
                displayedCustomResources2 = ResourceManager.Instance.GetCustomResource(resources[1].id);
                customResource2Label.text = displayedCustomResources2.ToString();
                customResourceType2 = resources[1].id;
            }
        }

        public void UpdateResource(bool instant)
        {
            StopCoroutine("DisplayResource");
            if (instant)
            {
                resourceLabel.text = ResourceManager.Instance.Resources.ToString();
                displayedResources = ResourceManager.Instance.Resources;
            }
            else
            {
                StartCoroutine("DisplayResource");
            }
        }

        public void UpdateGold(bool instant)
        {
            StopCoroutine("DisplayGold");
            if (instant)
            {
                goldLabel.text = ResourceManager.Instance.Gold.ToString();
                displayedGold = ResourceManager.Instance.Gold;
            }
            else
            {
                StartCoroutine("DisplayGold");
            }
        }

        public void UpdateLevel(bool showLevelUp)
        {
            levelLabel.text = "Level " + ResourceManager.Instance.Level.ToString();
            xpSprite.fillAmount = (float)(ResourceManager.Instance.Xp - ResourceManager.Instance.XpRequiredForCurrentLevel()) / (float)ResourceManager.Instance.XpRequiredForNextLevel();
            if (showLevelUp)
            {
                // TODO Some kind of level up thingy
            }
        }

        public void UpdateCustomResource1(bool instant)
        {
            StopCoroutine("DisplayCustomResource1");
            if (instant)
            {
                customResource1Label.text = ResourceManager.Instance.GetCustomResource(customResourceType1).ToString();
                displayedCustomResources1 = ResourceManager.Instance.GetCustomResource(customResourceType1);
            }
            else
            {
                StartCoroutine("DisplayCustomResource1");
            }
        }

        public void UpdateCustomResource2(bool instant)
        {
            StopCoroutine("DisplayCustomerResource2");
            if (instant)
            {
                customResource2Label.text = ResourceManager.Instance.GetCustomResource(customResourceType2).ToString();
                displayedCustomResources2 = ResourceManager.Instance.GetCustomResource(customResourceType2);
            }
            else
            {
                StartCoroutine("DisplayCustomerResource2");
            }
        }

        private IEnumerator DisplayResource()
        {
            while (displayedResources != ResourceManager.Instance.Resources)
            {
                int difference = displayedResources - ResourceManager.Instance.Resources;
                if (difference > 2000) displayedResources -= 1000;
                else if (difference > 200) displayedResources -= 100;
                else if (difference > 20) displayedResources -= 10;
                else if (difference > 0) displayedResources -= 1;
                else if (difference < -2000) displayedResources += 1000;
                else if (difference < -200) displayedResources += 100;
                else if (difference < -20) displayedResources += 10;
                else if (difference < 0) displayedResources += 1;
                resourceLabel.text = displayedResources.ToString();
                yield return true;
            }
        }

        private IEnumerator DisplayGold()
        {
            while (displayedGold != ResourceManager.Instance.Gold)
            {
                int difference = displayedGold - ResourceManager.Instance.Gold;
                if (difference > 2000) displayedGold -= 1000;
                else if (difference > 200) displayedGold -= 100;
                else if (difference > 20) displayedGold -= 10;
                else if (difference > 0) displayedGold -= 1;
                else if (difference < -2000) displayedGold += 1000;
                else if (difference < -200) displayedGold += 100;
                else if (difference < -20) displayedGold += 10;
                else if (difference < 0) displayedGold += 1;
                goldLabel.text = displayedGold.ToString();
                yield return true;
            }
        }

        private IEnumerator DisplayCustomResource1()
        {
            while (displayedCustomResources1 != ResourceManager.Instance.GetCustomResource(customResourceType1))
            {
                int difference = displayedCustomResources1 - ResourceManager.Instance.GetCustomResource(customResourceType1);
                if (difference > 2000) displayedCustomResources1 -= 1000;
                else if (difference > 200) displayedCustomResources1 -= 100;
                else if (difference > 20) displayedCustomResources1 -= 10;
                else if (difference > 0) displayedCustomResources1 -= 1;
                else if (difference < -2000) displayedCustomResources1 += 1000;
                else if (difference < -200) displayedCustomResources1 += 100;
                else if (difference < -20) displayedCustomResources1 += 10;
                else if (difference < 0) displayedCustomResources1 += 1;
                customResource1Label.text = displayedCustomResources1.ToString();
                yield return true;
            }
        }

        private IEnumerator DisplayCustomResource2()
        {
            while (displayedCustomResources2 != ResourceManager.Instance.GetCustomResource(customResourceType2))
            {
                int difference = displayedCustomResources2 - ResourceManager.Instance.GetCustomResource(customResourceType2);
                if (difference > 2000) displayedCustomResources2 -= 1000;
                else if (difference > 200) displayedCustomResources2 -= 100;
                else if (difference > 20) displayedCustomResources2 -= 10;
                else if (difference > 0) displayedCustomResources2 -= 1;
                else if (difference < -2000) displayedCustomResources2 += 1000;
                else if (difference < -200) displayedCustomResources2 += 100;
                else if (difference < -20) displayedCustomResources2 += 10;
                else if (difference < 0) displayedCustomResources2 += 1;
                customResource2Label.text = displayedCustomResources2.ToString();
                yield return true;
            }
        }
    }
}