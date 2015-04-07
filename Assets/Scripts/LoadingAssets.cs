using UnityEngine;
using System.Collections;
using System;

public class LoadingAssets : MonoBehaviour {

	private readonly string BundleURL = "http://app.npc-games.com/bubles_assets_ios.unity3d";
	private string AssetName = "Game";
	private int version = 2;

	public GameObject game;
	void Start() {
		StartCoroutine (DownloadAndCache());
	}
	
	IEnumerator DownloadAndCache (){

#if !UNITY_EDITOR
		// Wait for the Caching system to be ready
		while (!Caching.ready)
			yield return null;
		
		// Load the AssetBundle file from Cache if it exists with the same version or download and store it in the cache
		using(WWW www = WWW.LoadFromCacheOrDownload (BundleURL, version)){
			yield return www;
			if (www.error != null)
				throw new Exception("WWW download had an error:" + www.error);
			AssetBundle bundle = www.assetBundle;
			if (AssetName == "")
				Instantiate(bundle.mainAsset);
			else
				Instantiate(bundle.Load(AssetName));
			// Unload the AssetBundles compressed contents to conserve memory
			bundle.Unload(false);
			www.Dispose();
			
		} // memory is freed from the web stream (www.Dispose() gets called implicitly)

#else
		Instantiate(game);
#endif
		EventAggregator.updateGameState.Publish(GameState.waitStart);
		yield return null;
	}
}
