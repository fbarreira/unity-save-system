using UnityEngine;

namespace JustKrated.CrossPlatformSaveSystem
{
	public static class SaveManager
	{
		//Declares a instance of autosave file
		private static AutoSave autoSaveFile;

		/// <summary>
		/// Load autosave file.
		/// </summary>
		public static void LoadAutoSave ()
		{
			//Loads an instance of autosave file
			autoSaveFile = (AutoSave)SaveSystem.Load ("autosave");

			//If file does not exists, creates a new one
			if (autoSaveFile == null)
			{
				Debug.Log ("Creating new savefile");

				//Creates new instance of save file.
				autoSaveFile = new AutoSave ();

				//Store autosave file
				SaveSystem.Save (autoSaveFile, "autosave");
			}
		}

		/// <summary>
		/// Update active file on autosave.
		/// </summary>
		/// <param name="fileName"></param>
		private static void UpdateActiveFileInAutoSave (string fileName)
		{
			//Updates current save file
			autoSaveFile.currentSaveFile = fileName;
			//Store autosave file
			SaveSystem.Save (autoSaveFile, "autosave");
		}

		/// <summary>
		/// Load data from active file on autosave.
		/// </summary>
		/// <returns></returns>
		public static SaveFile LoadDataFromAutosave ()
		{
			//Loads current savefile if is stored
			if (autoSaveFile.currentSaveFile != null)
				return LoadDataFromFile (autoSaveFile.currentSaveFile);
			else
			{
				LoadAutoSave ();
				return autoSaveFile;
			}
				
		}

		/// <summary>
		/// Saves data to savefile active in autosave.
		/// </summary>
		/// <param name="file"></param>
		public static void SaveDataToAutoSave (SaveFile file)
		{
			SaveDataToFile (file, autoSaveFile.currentSaveFile);
		}

		/// <summary>
		/// Saves data to file with given name.
		/// </summary>
		/// <param name="file"></param>
		/// <param name="fileName"></param>
		public static void SaveDataToFile (SaveFile file, string fileName)
		{
			if (file == null)
			{
				Debug.LogWarning ("File is null. Make sure to create a new instance before saving saving it");
				return;
			}

			Debug.Log ("Saving...");
			UpdateActiveFileInAutoSave (fileName);

			//Saves files with given name
			SaveSystem.Save (file, fileName);
		}

		/// <summary>
		/// Load data from file with given name.
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public static SaveFile LoadDataFromFile (string fileName)
		{
			Debug.Log ("Loading...");

			UpdateActiveFileInAutoSave (fileName);

			//load file in the abstract form

			return SaveSystem.Load (fileName);
		}

		/// <summary>
		/// The name of current file active in autosave.
		/// </summary>
		public static string CurrentSaveFile
		{
			get
			{
				return autoSaveFile.currentSaveFile;
			}
		}
	}
}
