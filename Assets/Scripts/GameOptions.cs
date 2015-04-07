using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class GameOptions : MonoBehaviour {
	public GameObject pref;

	void Awake(){
		EventAggregator.updateGameState.Subscribe(UpdateState);
	}

	void UpdateState(GameState state){
		Game.state = state;
	}

	void Update(){
		if (Game.state == GameState.inGame){
			Game.userTime += Time.deltaTime;
		}

	}

}

public enum GameState{
	none,
	loadAssets,
	openAssets,
	waitStart,
	inGame
}

public static class Game{
	//игровые параметры
	public static readonly float minRadius = 1f; 		// минимальный радиус круга
	public static readonly float maxRadius = 5f; 		// максимальный радиус
	public static readonly float speedDelta = 1f; 		// дельта скорости
	public static readonly float timeSpeedDelta = 0.1f; // дельта увеличения скорости от времени
	public static readonly float scoreDelta = 2f;		// дельта очков опыта от скорости

	public static GameState state = GameState.none; // храним состояние игры

	//параметры ui
	public static float UI_loadingProgressBar = 0;

	//динамические игровые параметры
	public static int userScore;
	public static float userTime;



	public static string GetReadableTime(int time){
		float seconds = (float)(time);
		float minutes = Mathf.Floor(seconds / 60);
		seconds = Mathf.RoundToInt(seconds % 60);
  
		if (minutes < 1)
			return string.Format("{0}", seconds.ToString("00"));
		else return string.Format("{0}:{1}",minutes.ToString("00"), seconds.ToString("00"));
	}

}

public class EventAggregator : MonoBehaviour{
	public static UpdateGameState updateGameState = new UpdateGameState();
}

public class UpdateGameState{
	private readonly List<Action<GameState>> _callbacks = new List<Action<GameState>>();
	
	public void Subscribe(Action<GameState> callback){
		_callbacks.Add (callback);
	}
	
	public void Del(Action<GameState> callback){
		_callbacks.Remove(callback);
	}
	
	public void Publish (GameState state){
		foreach (Action<GameState> callback in _callbacks){
			if (callback != null)callback (state);
		}
	}
}

