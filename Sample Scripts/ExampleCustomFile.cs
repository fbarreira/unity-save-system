using UnityEngine;
using UnityEngine.UI;

namespace JustKrated.CrossPlatformSaveSystem
{
	public class ExampleCustomFile : MonoBehaviour
	{
		public Text plot;
		public Text trigger1;
		public Text trigger2;

		public void ButtonSetSampleFiles ()
		{
			int index = 0;
			CreateOption (index, "Apples are red.", true, true);
			index++;
			CreateOption (index, "Pigs can fly.", false, false);
			index++;
			CreateOption (index, "Even a broken clock is right 2 times a day.", true, false);
		}

		public void ButtonOption (int index)
		{
			string fileName = string.Format ("SampleOption{0}.test", index);

			//Loads the custom file from StreamingAssets folder by given name
			SampleCustomFile customFile = (SampleCustomFile)SaveSystem.LoadFromPath (fileName, Application.streamingAssetsPath);

			//If file exists, displays data
			if (customFile != null)
			{
				//Display data from custom file
				plot.text = customFile.plot;
				trigger1.text = string.Format ("Trigger 1: {0}", customFile.trigger1);
				trigger2.text = string.Format ("Trigger 2: {0}", customFile.trigger2);
			}
		}

		private void CreateOption (int index, string plot, bool trigger1, bool trigger2)
		{
			//In case StreamingAssets Folder doesn't exist yet, 
			//SaveSystem will create one, and all subfolder if required.
			//For more information about StreamingAssets folder check the refence at
			//https://docs.unity3d.com/ScriptReference/Application-streamingAssetsPath.html

			//Creates a new instance of the SampleCustomFile
			SampleCustomFile customFile = new SampleCustomFile
			{
				ID = index,
				plot = plot,
				trigger1 = trigger1,
				trigger2 = trigger2
			};

			//When saving on StreamingAssets you can set the extension to whatever you want,
			//in this case it will be '.test'
			//Don't forget that when loading from StreamingAssets 
			//you should match the exact extension used when saving
			string fileName = string.Format ("SampleOption{0}.test", index);

			//Save the newly created custom file at StreamingAssets folder
			//You can add a subfolder in StreamingAssets for better organization
			SaveSystem.SaveToPath (customFile, fileName, Application.streamingAssetsPath);
		}

	}
}

