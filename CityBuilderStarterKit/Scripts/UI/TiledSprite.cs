using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CBSK
{

    [ExecuteInEditMode]
    [RequireComponent(typeof(SpriteRenderer))]
    public class TiledSprite : MonoBehaviour
    {
        public Vector2 gridSize;

        internal SpriteRenderer spriteRenderer;
        private Vector2 spriteSize;

        //Used for instantiation
        internal GameObject newObject;
        internal SpriteRenderer newSprite;

#if UNITY_EDITOR
        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.enabled = false;
        }

        void Clear()
        {
            var tempList = transform.Cast<Transform>().ToList();
            foreach (var child in tempList)
            {
                DestroyImmediate(child.gameObject);
            }
        }

        public virtual void Instantiate()
        {
            if (GetSpriteAlignment(spriteRenderer.sprite) != SpriteAlignment.TopLeft)
            {
                Debug.LogWarning(string.Format("Make sure sprite {0} has its pivot set to the top left.", spriteRenderer.sprite.name));
            }
            Vector2 startingPosition, worldSize;
            worldSize.x = spriteRenderer.bounds.size.x;
            worldSize.y = spriteRenderer.bounds.size.y;
            startingPosition.x = -((worldSize.x * gridSize.x) / 2);
            startingPosition.y = (worldSize.y * gridSize.y) / 2;

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    newObject = new GameObject(string.Format("{0},{1}", x, y));
                    newObject.hideFlags = HideFlags.HideInHierarchy;
                    newObject.transform.SetParent(gameObject.transform);
                    newObject.transform.position = new Vector3(gameObject.transform.position.x + startingPosition.x + worldSize.x * x, gameObject.transform.position.y + startingPosition.y - worldSize.y * y, gameObject.transform.position.z);
                    newObject.transform.localScale = Vector3.one;
                    newSprite = newObject.AddComponent<SpriteRenderer>();
                    newSprite.sprite = spriteRenderer.sprite;
                    newSprite.color = spriteRenderer.color;
                    newSprite.sortingLayerID = spriteRenderer.sortingLayerID;
                    newSprite.sortingOrder = spriteRenderer.sortingOrder;
                }
            }

        }

        void Update()
        {
            if (gameObject.transform.childCount < 1 && spriteRenderer.sprite != null && gridSize.x > 0 && gridSize.y > 0)
            {
                Instantiate();
            }
            else if (gameObject.transform.childCount > 0 && gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite != spriteRenderer.sprite)
            {
                Clear();
                Instantiate();
            }
            else if (gameObject.transform.childCount > 0 && gameObject.transform.childCount != gridSize.x * gridSize.y)
            {
                Clear();
                Instantiate();
            }
        }

        internal SpriteAlignment GetSpriteAlignment(Sprite SpriteObject)
        {
            //BoxCollider2D MyBoxCollider = SpriteObject.AddComponent<BoxCollider2D>();
            //float colX = MyBoxCollider.center.x;
            //float colY = MyBoxCollider.center.y;
            float colX = SpriteObject.bounds.center.x;
            float colY = SpriteObject.bounds.center.y;
            if (colX > 0f && colY < 0f)
                return (SpriteAlignment.TopLeft);
            else if (colX < 0 && colY < 0)
                return (SpriteAlignment.TopRight);
            else if (colX == 0 && colY < 0)
                return (SpriteAlignment.TopCenter);
            else if (colX > 0 && colY == 0)
                return (SpriteAlignment.LeftCenter);
            else if (colX < 0 && colY == 0)
                return (SpriteAlignment.RightCenter);
            else if (colX > 0 && colY > 0)
                return (SpriteAlignment.BottomLeft);
            else if (colX < 0 && colY > 0)
                return (SpriteAlignment.BottomRight);
            else if (colX == 0 && colY > 0)
                return (SpriteAlignment.BottomCenter);
            else if (colX == 0 && colY == 0)
                return (SpriteAlignment.Center);
            else
                return (SpriteAlignment.Custom);
        }
#endif
    }
}