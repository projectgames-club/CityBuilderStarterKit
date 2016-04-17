using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CBSK
{
    /// <summary>
    /// This class is mainly for doing gernal UI things like loading button icons. 
    /// Functionality of the individual UI components is handled by their own classes
    /// </summary>
    public class SpriteManager : MonoBehaviour
    {
        public static SpriteManager instance;

        public List<Sprite> buttonSprites = new List<Sprite>();
        public List<Sprite> buildingSprites = new List<Sprite>();
        public List<Sprite> unitSprites = new List<Sprite>();

        void Start()
        {
            instance = this;
        }

        /// <summary>
        /// Return a sprite by the given name. This is the general method, for speed use one of the specific methods
        /// </summary>
        /// <param name="spriteName"></param>
        /// <returns></returns>
        public static Sprite GetSprite(string spriteName)
        {
            if (SpriteManager.instance == null)
            {
                Debug.LogError("UIManager not found");
                return null;
            }

            //This method will have to search up to 3 lists to find the sprite in question.
            Sprite newSprite = GetButtonSprite(spriteName);
            if (newSprite == null)
            {
                newSprite = GetBuildingSprite(spriteName);
                if (newSprite == null)
                {
                    newSprite = GetUnitSprite(spriteName);
                    if (newSprite == null)
                    {
                        Debug.LogError(string.Format("Sprite {0} not found in {1}", spriteName, SpriteManager.instance.gameObject.name));
                    }
                }
            }
            return newSprite;
        }


        /// <summary>
        /// Returns a button sprite by the given name
        /// </summary>
        /// <param name="spriteName"></param>
        /// <returns></returns>
        public static Sprite GetButtonSprite(string spriteName)
        {
            if (SpriteManager.instance == null)
            {
                Debug.LogError("UIManager not found");
                return null;
            }
            Sprite newSprite = SpriteManager.instance.buttonSprites.FirstOrDefault(x => x.name.Equals(spriteName));
            if (newSprite == null)
            {
                Debug.LogError(string.Format("Sprite {0} not found in {1}", spriteName, SpriteManager.instance.gameObject.name));
            }
            return newSprite;
        }


        /// <summary>
        /// Returns a building sprite by the given name
        /// </summary>
        /// <param name="spriteName"></param>
        /// <returns></returns>
        public static Sprite GetBuildingSprite(string spriteName)
        {
            if (SpriteManager.instance == null)
            {
                Debug.LogError("UIManager not found");
                return null;
            }
            Sprite newSprite = SpriteManager.instance.buildingSprites.FirstOrDefault(x => x.name.Equals(spriteName));
            if (newSprite == null)
            {
                Debug.LogError(string.Format("Sprite {0} not found in {1}", spriteName, SpriteManager.instance.gameObject.name));
            }
            return newSprite;
        }

        /// <summary>
        /// Returns a unit sprite by the given name
        /// </summary>
        /// <param name="spriteName"></param>
        /// <returns></returns>
        public static Sprite GetUnitSprite(string spriteName)
        {
            if (SpriteManager.instance == null)
            {
                Debug.LogError("UIManager not found");
                return null;
            }
            Sprite newSprite = SpriteManager.instance.unitSprites.FirstOrDefault(x => x.name.Equals(spriteName));
            if (newSprite == null)
            {
                Debug.LogError(string.Format("Sprite {0} not found in {1}", spriteName, SpriteManager.instance.gameObject.name));
            }
            return newSprite;
        }
    }
}