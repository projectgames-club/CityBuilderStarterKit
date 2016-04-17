using UnityEngine;
using System.Collections;

namespace CBSK
{
    /**
     * Abstract class extended by all "manager" type objects.
     * Managers have only one instance in a scene and this class helps to enforce that.
     */
    public abstract class Manager<T> : MonoBehaviour where T : Manager<T>
    {

        /** Static reference to this manager. */
        protected static T manager;
        /** Has this manager been initialised. */
        protected bool initialised = false;

        /**
         * Get the instance of the manager class or create if one has not yet been created.
         * 
         * @returns An instance of the manager class.
         */
        public static T GetInstance()
        {
            if (manager == null) Create();
            return manager;
        }

        /**
         * Create a new game object and attach an instance of the manager.
         */
        protected static void Create()
        {
            GameObject go = new GameObject();
            go.name = typeof(T).Name;
            manager = (T)go.AddComponent(typeof(T));
        }

        /**
         * If there is already a manager destroy self, else initialise and assign to the static reference.
         */
        void Awake()
        {
            if (manager == null)
            {
                if (!initialised) Init();
                manager = (T)this;
            }
            else if (manager != this)
            {
                Destroy(gameObject);
            }
        }

        /**
         * Initialise the manager. Override this to perform initialisation in sub-classes. Note this
         * initialisation should never rely on another manager instance being available as it is called from Awake().
         */
        virtual protected void Init()
        {
            initialised = true;
        }

    }

}