using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UISlider : UIWindow {
	public RectTransform foreground;

	void LateUpdate(){
		foreground.localScale = new Vector3(Game.UI_loadingProgressBar, 1, 1);
	}
}
