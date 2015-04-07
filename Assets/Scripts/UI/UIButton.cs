using UnityEngine;
using System.Collections;

public class UIButton : UIWindow {
	[Tooltip("Выбираем на какой стейт меняем при клике")]
	public GameState changeToState = GameState.none;

	public void OnClick(){
		EventAggregator.updateGameState.Publish(changeToState);
	}
}
