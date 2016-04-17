#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml.Serialization;

namespace CBSK
{
	/// <summary>
	/// Stores user settings. 
	/// </summary>
	[System.Serializable]
	public class CityBuilderSettings
	{
		#region serialised fields

		/// <summary>
		/// Should we show the welcome screen on startup?
		/// </summary>
		public bool showTipOnStartUp = true;

		#endregion

		public const string RelativeDataPath = "CityBuilderSettings.xml";

		public static CityBuilderSettings instance;

		/// <summary>
		/// Gets the current settings or loads them if null.
		/// </summary>
		/// <value>The instance.</value>
		public static CityBuilderSettings Instance {
			get
			{
				if (instance == null) Load();
				return instance;
			}
		}

		/// <summary>
		/// Load the data.
		/// </summary>
		protected static void Load()
		{
			try 
			{
				using (StreamReader reader = new StreamReader(Application.dataPath + Path.DirectorySeparatorChar + RelativeDataPath))
				{
					XmlSerializer serializer = new XmlSerializer (typeof(CityBuilderSettings));
					instance = (CityBuilderSettings)serializer.Deserialize (reader);
				}
			}
			catch (IOException)
			{
				instance = new CityBuilderSettings();
			}
		}

		/// <summary>
		/// Save the data.
		/// </summary>
		public static void Save()
		{
			if (instance != null)
			{
				using (StreamWriter writer = new StreamWriter(Application.dataPath + Path.DirectorySeparatorChar + RelativeDataPath))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(CityBuilderSettings));
					serializer.Serialize(writer, instance);
				}
			}
		}
	}
}
#endif