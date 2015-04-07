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
	private Texture2D[] old_texturePack = new Texture2D[4];
	private Transform root;
	private bool newPack;
	private Camera cam;

	void Awake(){
		EventAggregator.updateGameState.Subscribe(UpdateState);
		root = transform.FindChild("root");
		cam = Camera.main;
	}

	void UpdateState(GameState state){
		Game.state = state;
		EventAggregator.bubbles.Publish(state);
		if(Game.state == GameState.waitStart){
			Game.userTime = 0;
			StartCoroutine("GenerateTexturePack");
		}
		else if (Game.state == GameState.inGame){
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

			if ((int)(Game.userTime % 15) == 0){ //каждые 10 сек обновляем пак
				if(!newPack){
					StartCoroutine("GenerateTexturePack"); 
					newPack = true;
				}
			} else newPack = false;
		}

		if (Game.state == GameState.waitStart || Game.state == GameState.inGame){
			if (spawnCd <= 0){
				GameObject bubble = Instantiate(pref) as GameObject;
				bubble.transform.parent = root;
				float radius = Random.Range(Game.minRadius, Game.maxRadius);
				bubble.GetComponent<Bubble>().Init(radius, texturePack[(int)radius]);
				spawnCd = Random.Range(0, Mathf.Lerp(0.2f, 2, 7 / Game.userTime)); //увеличиваем кол-во шариков со временем
			}
			else spawnCd -= Time.deltaTime;
		}
		
	}
	
	IEnumerator GenerateTexturePack(){
		Debug.Log("GenerateTexturePack");
		Texture2D[] _texturePack = new Texture2D[4]; //временно создаем
		//Generate new Textures
		for (int i = 0; i < 4; i ++){
			int scale  = 32;
			if 		(i == 0) scale = 32;
			else if (i == 1) scale = 64;
			else if (i == 2) scale = 128;
			else if (i == 3) scale = 256;
			Texture2D tx = new Texture2D(scale, scale, TextureFormat.ARGB32, false);
			Color32[] pix = new Color32[scale * scale];
			Color top = new Color(Random.Range(0f,1f),Random.Range(0,1f),Random.Range(0,1f));
			Color down = new Color(Random.Range(0f,1f),Random.Range(0,1f),Random.Range(0,1f));
			//gen pic
			Vector2 picCenter = new Vector2(scale / 2, scale / 2);
			for (int y = 0; y < scale; y ++){
				for (int x = 0; x < scale; x ++){
					if (Vector2.Distance(picCenter, new Vector2(x, y)) < picCenter.x){
						pix[y * scale + x] = Color.Lerp(top,down, y / (float)scale);
					}
					else pix[y * scale + x] = new Color(0,0,0,0);
				}
			}
			//
			tx.SetPixels32(pix);
			tx.Apply(false);
			_texturePack[i] = tx;
		}
		//Clear textures
		foreach(Texture2D t in old_texturePack){
			if (t != null) Destroy(t);
		}
		//
		old_texturePack = texturePack;
		texturePack = _texturePack;
		_texturePack = null;
		yield return null;	
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
	public static readonly float defaultSpriteScale = 50;
	public static readonly float minRadius = 1f; 		// минимальный радиус круга
	public static readonly float maxRadius = 4f; 		// максимальный радиус
	public static readonly float minSpeed = 1;			// минимальная скорость
	public static readonly float maxSpeed = 4;			// максимальная скорость
	public static readonly float speedDelta = 1f; 		// дельта скорости
	public static readonly float timeSpeedDelta = 0.02f; // дельта увеличения скорости от времени
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

