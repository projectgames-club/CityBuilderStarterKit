//This is just a variation of the TiledSprite class that attaches the UIBackground script to the generated tiles.
//The purpose of that script is to register clicks on the background
using UnityEngine;
namespace CBSK
{
    public class BackgroundTiledSprite : TiledSprite
    {
#if UNITY_EDITOR
        public override void Instantiate()
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
                    newObject.layer = gameObject.layer;
                    newObject.transform.SetParent(gameObject.transform);
                    newObject.transform.position = new Vector3(gameObject.transform.position.x + startingPosition.x + worldSize.x * x, gameObject.transform.position.y + startingPosition.y - worldSize.y * y, gameObject.transform.position.z);
                    newObject.transform.localScale = Vector3.one;
                    newSprite = newObject.AddComponent<SpriteRenderer>();
                    newSprite.sprite = spriteRenderer.sprite;
                    newSprite.color = spriteRenderer.color;
                    newSprite.sortingLayerID = spriteRenderer.sortingLayerID;
                    newSprite.sortingOrder = spriteRenderer.sortingOrder;
                    newObject.AddComponent<UIBackground>();
                    newObject.AddComponent<BoxCollider2D>();
                }
            }

        }
#endif
    }
}