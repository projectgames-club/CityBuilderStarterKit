using UnityEngine;
using System.Collections;

/// <summary>
/// Data about a custom resource type.
/// </summary>
namespace CBSK
{
    [System.Serializable]
    public class CustomResourceType
    {
        /// <summary>
        /// Id of the resource.
        /// </summary>
        public string id;

        /// <summary>
        /// Human readable name.
        /// </summary>
        public string name;

        /// <summary>
        /// Name of the sprite to use for this resource.
        /// </summary>
        public string spriteName;

        /// <summary>
        /// How many of the resource you get in a new game.
        /// </summary>
        public int defaultAmount;

    }
}