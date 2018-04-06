using System;
using System.IO;
using System.Linq;
using Automatine;
using UnityEditor;
using UnityEngine;


[InitializeOnLoad] public class TestIgniter {
	private static string dataPath = Automatine.FileController.PathCombine(Application.dataPath, "AutomatineTest/Editor", "buildCount.txt");

	static TestIgniter () {
		CountupBuildNum();
		
		var testContext = new Test();
		testContext.RunTests();

		// ExportAutomatine();
	}

	private static void CountupBuildNum() {
		
		
		var count = 0;
		if (File.Exists(dataPath)) { 
			count = ReadBuildCount(dataPath);
			count = count + 1;
		}
		
		using (var sw = new StreamWriter(dataPath)) {
			sw.Write(string.Empty + count);
		}
	}
	
	public static int ReadBuildCount (string currentDataPath) {
		var count = 0;
		using (var sr = new StreamReader(currentDataPath)) {
			Int32.TryParse(sr.ReadToEnd(), out count);
		}
		return count;
	}
	
	
	[MenuItem("Window/AutomatineExport")] public static void Export () {
		ExportAutomatine();
	}

	public static void ExportAutomatine () {
		var count = ReadBuildCount(dataPath);
		
		// Automatine itself.
		var automatinePaths = FileController.FilePathsInFolder("Assets/Automatine");
		AssetDatabase.ExportPackage(
			automatinePaths
				.Where(
					path => !path.StartsWith("Assets/Automatine/Runtime/Generated")
				)
				.Where(
					path => !path.StartsWith("Assets/Automatine/Editor/Data")
				)
				.ToArray(), 
			"Automatine_build_" + count + ".unitypackage"
		);
		
		
		
		// sample scene.
		var samplePaths = FileController.FilePathsInFolder("Assets/SampleProject");
		samplePaths.Add("Assets/Automatine/Editor/Data/automatine.json");
		
		AssetDatabase.ExportPackage(
			samplePaths.ToArray(), 
			"Automatine_sample_scene" + ".unitypackage"
		);
	}
}