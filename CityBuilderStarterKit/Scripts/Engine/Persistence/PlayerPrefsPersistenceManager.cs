using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Text;


/**
 * Saves and loads data from PlayerPrefs
 */
namespace CBSK
{
    public class PlayerPrefsPersistenceManager : PersistenceManager
    {

        public string playerPrefName = "SAVED_GAME";

        protected bool willSave;

        protected System.Xml.Serialization.XmlSerializer serializer;

        void LateUpdate()
        {
            // Only save once per frame at the end of the frame to avoid race conditions
            if (willSave)
            {
                DoSave();
                willSave = false;
            }
        }

        /**
         * Used to determine if there is a game that should be loaded.
         */
        override public bool SaveGameExists()
        {
            if (PlayerPrefs.GetString(playerPrefName, "").Length > 0) return true;
            return false;
        }

        override public void Save()
        {
            willSave = true;
        }

        virtual protected void DoSave()
        {
            InitTypes();
            if (serializer == null) serializer = new System.Xml.Serialization.XmlSerializer(typeof(SaveGameData), types);
            SaveGameData dataToSave = GetSaveGameData();
            if (dataToSave != null)
            {
                try
                {
                    using (var stream = new StringWriter())
                    {
                        serializer.Serialize(stream, dataToSave);
                        PlayerPrefs.SetString(playerPrefName, stream.ToString());
                    }
                }
                catch (System.IO.IOException e)
                {
                    Debug.LogError("Error saving buildings:" + e.Message);
                }
            }
            else
            {
                Debug.LogWarning("Nothing to save");
            }
        }

        override public SaveGameData Load()
        {
            SaveGameData result;
            InitTypes();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(SaveGameData), types);
            String savedGame = PlayerPrefs.GetString(playerPrefName, "");
            if (savedGame.Length < 1)
            {
                Debug.LogError("No saved game data present.");
                return null;
            }
            using (StringReader stream = new StringReader(savedGame))
            {
                result = (SaveGameData)serializer.Deserialize(stream);
            }
            return result;
        }
    }
}