﻿// Builds an asset bundle from the selected objects in the project view.
// Once compiled go to "Menu" -> "Assets" and select one of the choices
// to build the Asset Bundle

using UnityEngine;
using UnityEditor;

public class ExportAssetBundles {

	[MenuItem("Assets/Build AssetBundle/iPhone")]
	static void ExportResourceNoTrack () {
		ExportResourceNoTrack(BuildTarget.iPhone);
	}
	static void ExportResourceNoTrack (BuildTarget target) {
		// Bring up save panel
		string path = EditorUtility.SaveFilePanel ("Save Resource", "", "New Resource", "unity3d");
		if (path.Length != 0) {
			// Build the resource file from the active selection.

			BuildPipeline.BuildAssetBundle(Selection.activeObject, Selection.objects, path, BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets, target);
		}
	}
}