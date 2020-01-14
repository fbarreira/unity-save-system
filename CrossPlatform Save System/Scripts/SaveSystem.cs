using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace JustKrated.CrossPlatformSaveSystem
{
	public class SaveSystem
	{

#if UNITY_WEBGL
	[DllImport ("__Internal")]
	private static extern void SyncFiles ();

	[DllImport ("__Internal")]
	private static extern void WindowAlert (string message);
#endif

		#region Persistent Path
		/// <summary>
		///  Saves a file with a given name.
		/// </summary>
		/// <returns><c>true</c>, if data was saved, <c>false</c> otherwise.</returns>
		/// <param name="fileData">Game state file.</param>
		/// <param name="filename">Name of file to be saved.</param>
		public static bool Save (SaveFile fileData, string filename)
		{
			return SaveFileToPath (fileData, GetSavePath (filename));
		}

		/// <summary>
		/// Loads the game state with a given file name.
		/// </summary>
		/// <returns>The game state.</returns>
		/// <param name="fileName">Name of file to be loaded.</param>
		public static SaveFile Load (string fileName)
		{
			return LoadFileFromPath (GetSavePath (fileName));
		}

		/// <summary>
		/// Saves file to given path
		/// </summary>
		/// <param name="fileData"></param>
		/// <param name="filePath"></param>
		/// <returns></returns>
		private static bool SaveFileToPath (SaveFile fileData, string filePath)
		{
			if (Application.platform == RuntimePlatform.WebGLPlayer)
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter ();
				FileStream fileStream;

				try
				{
					if (DoesFileExists (filePath))
					{
						File.WriteAllText (filePath, string.Empty);
						fileStream = File.Open (filePath, FileMode.Open);
					}
					else
					{
						fileStream = File.Create (filePath);
					}

					binaryFormatter.Serialize (fileStream, fileData);
					fileStream.Close ();

#if UNITY_WEBGL
				SyncFiles ();
#endif

					return true;
				}
				catch (Exception e)
				{
					PlatformSafeMessage ("Failed to Save: " + e.Message);
					return false;
				}
			}
			else
			{
				//Directory.CreateDirectory (Application.streamingAssetsPath);

				BinaryFormatter formatter = new BinaryFormatter ();

				using (FileStream stream = new FileStream (filePath, FileMode.Create))
				{
					try
					{
						formatter.Serialize (stream, fileData);
					}
					catch (Exception)
					{
						Debug.LogWarning ("Could not save file at: " + filePath);
						return false;
					}
					return true;
				}
			}
		}

		/// <summary>
		/// Loads file from given path
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		private static SaveFile LoadFileFromPath (string filePath)
		{
			if (Application.platform == RuntimePlatform.WebGLPlayer)
			{
				SaveFile data = null;

				try
				{
					if (DoesFileExists (filePath))
					{
						BinaryFormatter binaryFormatter = new BinaryFormatter ();
						FileStream fileStream = File.Open (filePath, FileMode.Open);

						data = (SaveFile)binaryFormatter.Deserialize (fileStream);
						fileStream.Close ();
					}
					else
					{
						PlatformSafeMessage ("File not found.");
					}
				}
				catch (Exception e)
				{
					PlatformSafeMessage ("Failed to Load: " + e.Message);
				}
				return data;
			}
			else
			{
				if (DoesFileExists (filePath))
				{
					BinaryFormatter formatter = new BinaryFormatter ();

					using (FileStream stream = new FileStream (filePath, FileMode.Open))
					{
						try
						{
							return formatter.Deserialize (stream) as SaveFile;
						}
						catch (Exception)
						{
							Debug.LogWarning ("Could not open file. Make sure file exists or was not altered outside Editor.");
							return null;
						}
					}
				}
				else if (filePath.Contains ("://"))
				{
					Debug.Log (" android file: " + filePath);

					WWW www = new WWW (filePath); ///Deprecated in Unity 2019
					while (!www.isDone) { }

					BinaryFormatter formatter = new BinaryFormatter ();

					using (MemoryStream ms = new MemoryStream (www.bytes))
					{
						try
						{
							return formatter.Deserialize (ms) as SaveFile;
						}
						catch (Exception)
						{
							Debug.LogWarning ("Could not open file. Make sure file exists or was not altered outside Editor.");
							return null;
						}
					}
				}
				else
				{
					Debug.LogWarningFormat ("File do not exist: {0}", filePath);
					return null;
				}
			}
		}
		#endregion

		#region Custom Path
		/// <summary>
		/// Saves the data in a custom location.
		/// </summary>
		/// <returns><c>true</c>, if data was saved, <c>false</c> otherwise.</returns>
		/// <param name="fileData">File data.</param>
		/// <param name="filePath">File path.</param>
		public static bool SaveToPath (SaveFile fileData, string fileName, string filePath)
		{
			string path = string.Format ("{0}/{1}", filePath, fileName);

			return SaveFileToPath (fileData, path);
		}

		public static bool SaveToPath (SaveFile fileData, string filePath)
		{
			return SaveFileToPath (fileData, filePath);
		}

		/// <summary>
		/// Loads the data of a file from a custom location.
		/// </summary>
		/// <returns>The data.</returns>
		/// <param name="filePath">File path.</param>
		public static SaveFile LoadFromPath (string fileName, string filePath)
		{
			string path = string.Format ("{0}/{1}", filePath, fileName);

			return LoadFileFromPath (path);
		}

		public static SaveFile LoadFromPath (string filePath)
		{
			return LoadFileFromPath (filePath);
		}
		#endregion

		#region Auxiliar Functions
		private static bool DoesFileExists (string path)
		{
			return File.Exists (path);
		}

		/// <summary>
		/// Returns path of saved files.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		private static string GetSavePath (string name)
		{
			if (Application.platform == RuntimePlatform.WebGLPlayer)
			{
				return string.Format ("{0}/{1}.sav", Application.persistentDataPath, name);
			}
			else
			{
				return Path.Combine (Application.persistentDataPath, name + ".sav");
			}
		}

		private static void PlatformSafeMessage (string message)
		{
			if (Application.platform == RuntimePlatform.WebGLPlayer)
			{
#if UNITY_WEBGL
			WindowAlert (message);
#endif
			}
			else
			{
				Debug.LogWarning (message);
			}
		}
		#endregion
	}
}