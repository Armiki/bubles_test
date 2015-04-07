using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class GameOptions : MonoBehaviour {
	public GameObject pref;
	private float spawnCd;

	//принимаем к свединию, что: 0: x32; 1: x64, 2: x128; 3: x256
	private Texture2D[] texturePack = new Texture2D[4];

	void Awake(){
		EventAggregator.updateGameState.Subscribe(UpdateState);
	}

	void UpdateState(GameState state){
		Game.state = state;
		EventAggregator.bubbles.Publish(state);
		if (Game.state == GameState.inGame){
			Game.userScore = 0;
			Game.userTime = 0;
			Game.curSpeedDelta = Game.speedDelta;
			spawnCd = 3;
		}
	}

	void Update(){
		if (Game.state == GameState.inGame){
			Game.userTime += Time.deltaTime;
			Game.curSpeedDelta += Time.deltaTime * Game.timeSpeedDelta;
		}

		if (Game.state == GameState.waitStart || Game.state == GameState.inGame){
			if (spawnCd <= 0){
				GameObject bubble = Instantiate(pref) as GameObject;
				bubble.transform.parent = transform;
				bubble.GetComponent<Bubble>().Init(Random.Range(Game.minRadius, Game.maxRadius));
				spawnCd = Random.Range(0, 2f);
			}
			else spawnCd -= Time.deltaTime;
		}

	}

	IEnumerator GenerateTexturePack(){
		//Clear textures
		foreach(Texture2D t in texturePack){
			if (t != null) Destroy(t);
		}
		//

		//Generate new Textures
		for (int i = 0; i < 4; i ++){
			int scale = 32;
			Texture2D tx = new Texture2D(scale, scale);
			Color32[] pix = new Color32[scale * scale];
			//gen pic

			//
			tx.SetPixels32(pix);
			tx.Apply(false);
			texturePack[i] = tx;
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
	public static readonly float defaultSpriteScale = 30;
	public static readonly float minRadius = 1f; 		// минимальный радиус круга
	public static readonly float maxRadius = 4f; 		// максимальный радиус
	public static readonly float minSpeed = 1;			// минимальная скорость
	public static readonly float maxSpeed = 4;			// максимальная скорость
	public static readonly float speedDelta = 1f; 		// дельта скорости
	public static readonly float timeSpeedDelta = 0.04f; // дельта увеличения скорости от времени
	public static readonly float scoreDelta = 2f;		// дельта очков опыта от скорости

	public static GameState state = GameState.none; // храним состояние игры

	//параметры ui
	public static float UI_loadingProgressBar = 0;

	//динамические игровые параметры
	public static float curSpeedDelta = 1;
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
	public static UpdateGameState bubbles = new UpdateGameState();
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

