using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CBSK
{
    /**
     * An extension of the manager class which adds the ability to manage the state
     * of a specific list of game objects. Specifically this means being able
     * to load and save the game state data.
     */
    public abstract class GameStateManager<T, U> : Manager<T> where T : Manager<T>
    {

        /**
         * A collection of items that will be stored.
         */
        abstract protected List<U> StateData { get; set; }

        /**
         * An identifier to associate this data with the manager. In the 
         * default implementation this will be the name of the game object.
         */
        virtual protected string DataId
        {
            get { return this.GetType().Name; }
        }

        /**
         * Saves the state of this manager. This (default) implementation saves
         * data to PlayerPrefs using XMLSerializer. If you override it ensure you
         * also override <LoadState> and <ClearState>.
         */
        virtual public void SaveState()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<U>));
            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, StateData);
                PlayerPrefs.SetString(DataId, writer.ToString());
            }
        }

        /**
         * Loads the state of this manager. This (default) implementation loads
         * data from PlayerPrefs using XMLSerializer. If you override it ensure you
         * also override <SaveState> and <ClearState>.
         */
        virtual public void LoadState()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<U>));
            string data = PlayerPrefs.GetString(DataId, "");
            if (data.Length > 0)
            {
                using (StringReader reader = new StringReader(data))
                {
                    StateData = (List<U>)serializer.Deserialize(reader);
                }
            }
        }

        /**
         * Clear all state data. This (default) implementation deletes
         * the key from playerPrefs.
         */
        virtual public void ClearState()
        {
            PlayerPrefs.DeleteKey(DataId);
        }
    }
}
