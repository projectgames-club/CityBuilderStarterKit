#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections;


namespace CBSK
{
	[InitializeOnLoad]
	public class CityBuilderWelcomePopUp : EditorWindow
	{
		#region static variables

		/// <summary>
		/// Reference to the window
		/// </summary>
		public static CityBuilderWelcomePopUp window;

		/// <summary>
		/// The URL for the documentation.
		/// </summary>
		const string DOC_URL = "https://jnamobile.zendesk.com/hc/en-gb/categories/202587617-City-Builder-Starter-Kit";

		/// <summary>
		/// The URL for the tutorial videos.
		/// </summary>
		const string TUTORIAL_URL = "https://www.youtube.com/playlist?list=PLbnzW2Y4qytKNT25h8D_505VoYCVBux5s";

		/// <summary>
		/// The URL for submitting a support request.
		/// </summary>
		const string SUPPORT_URL = "https://jnamobile.zendesk.com/hc/en-gb/requests/new";

		/// <summary>
		/// The version as a string.
		/// </summary>
		const string VERSION = "v2.0.0";

		/// <summary>
		/// Cache show on startup state.
		/// </summary>
		protected static bool alreadyShown = false;

		/// <summary>
		/// The width of the welcome image.
		/// </summary>
		protected static int imageWidth = 512;

		#endregion

		/// <summary>
		/// Holds the CBSK intro texture
		/// </summary>
		protected Texture2D popUpTexture;

		static CityBuilderWelcomePopUp ()
		{ 
			if (!EditorApplication.isPlayingOrWillChangePlaymode)
			{
				EditorApplication.update += CheckShowWelcome;
			}
		}

		[MenuItem ("Assets/CBSK/Show Welcome")]
		public static void ShowWelcomeScreen()
		{
			// Lets assume that everyone window is at least 520 x 448
			float windowWidth = imageWidth + 8;
			float windowHeight = (imageWidth * 0.75f) + 64;
			Rect rect = new Rect((Screen.currentResolution.width - windowWidth) / 2.0f,
								 (Screen.currentResolution.height - windowHeight) / 2.0f,
								 windowWidth , windowHeight);
			window = (CityBuilderWelcomePopUp) EditorWindow.GetWindowWithRect(typeof(CityBuilderWelcomePopUp), rect, true, "Welcome to City Builder Starter Kit");
			window.minSize = new Vector2 (windowWidth, windowHeight);
			window.maxSize = new Vector2 (windowWidth, windowHeight);
			window.position = rect;
			window.Show();
			alreadyShown = true;
		}

		/// <summary>
		/// Check if we should show the welcome screen and show it if we should.
		/// </summary>
		protected static void CheckShowWelcome()
		{
			EditorApplication.update -= CheckShowWelcome;
			// When we compile we lose out static settings this is just a simple workaround to avoid annoying user with constant pop-ups
			if (Time.realtimeSinceStartup > 3) alreadyShown = true;
			if (!alreadyShown && CityBuilderSettings.Instance.showTipOnStartUp) 
			{

				if (!EditorApplication.isPlayingOrWillChangePlaymode)
				{
					ShowWelcomeScreen();
				}
			}
		}

		/// <summary>
		/// Draw the intro window
		/// </summary>
		void OnGUI ()
		{
			if (popUpTexture == null) popUpTexture = Resources.Load <Texture2D> ("CBSK_TitleScreen");
			GUILayout.Box (popUpTexture, GUILayout.Width(imageWidth), GUILayout.Height(imageWidth * 0.75f));

			bool showOnStartUp = GUILayout.Toggle (CityBuilderSettings.Instance.showTipOnStartUp, "Show this window on project open?");
			if (CityBuilderSettings.Instance.showTipOnStartUp != showOnStartUp)
			{
				CityBuilderSettings.Instance.showTipOnStartUp = showOnStartUp;
				CityBuilderSettings.Save();
			}
			GUILayout.FlexibleSpace ();
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Documentation")) Application.OpenURL (DOC_URL);
			if (GUILayout.Button ("Tutorials")) Application.OpenURL (TUTORIAL_URL);
			if (GUILayout.Button ("Support Request")) Application.OpenURL (SUPPORT_URL);
			GUILayout.EndHorizontal ();


			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			GUILayout.Label (VERSION);
			GUILayout.EndHorizontal ();
			GUILayout.FlexibleSpace ();

		}
	}

}

#endif