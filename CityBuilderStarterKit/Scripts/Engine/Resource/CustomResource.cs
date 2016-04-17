using UnityEngine;
using System.Collections;

/// <summary>
/// A custom resource type like wood, etc.
/// </summary>
namespace CBSK
{
    [System.Serializable]
    public class CustomResource
    {
        /// <summary>
        /// Name of the resource.
        /// </summary>
        public string id;

        /// <summary>
        /// The number the player has.
        /// </summary>
        public int amount;

        /**
         * Default constructor.
         */
        public CustomResource() { }

        /**
         * Convenience constructor.
         */
        public CustomResource(string id, int amount)
        {
            this.id = id;
            this.amount = amount;
        }
    }
}