using UnityEngine;
using System.Collections;

public class UIWindow : MonoBehaviour {

	[Tooltip("При каком стейте игры отображать это окно")]
	public GameState visState = GameState.none;

	//создаем кешированную ссылку до обьекта
	private GameObject myGo;
	private GameObject go { get { if (myGo == null) myGo = gameObject; return myGo; } }

	void Awake(){
		EventAggregator.updateGameState.Subscribe(UpdateState);
	}

	void UpdateState(GameState state){
		if (state == visState){
			go.SetActive(true);
		}
		else go.SetActive(false);
	}
}
