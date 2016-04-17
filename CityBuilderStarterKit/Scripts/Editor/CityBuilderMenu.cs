#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections;

namespace CBSK
{
	public class PersistenceMenuItem  {


		[MenuItem ("Assets/CBSK/Persistence/Reset All Player Prefs")]
		public static void ResetAllPlayerPrefs()
		{
			PlayerPrefs.DeleteAll ();
		}
	}
}

#endif