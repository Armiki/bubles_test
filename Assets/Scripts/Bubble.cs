using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Bubble : MonoBehaviour {

	//создаем кешированную ссылку до transform
	private RectTransform myTr;
	private RectTransform me { get { if (myTr == null) myTr = GetComponent<RectTransform>(); return myTr; } }

	private Texture2D texture;
	public RawImage img;
	public CircleCollider2D colider;
	public float scale;


	void Start(){
		EventAggregator.bubbles.Subscribe(ChangeState); //подписываемся на изменения
	}

	public void Init(float s){
		float _scale = Game.defaultSpriteScale * s;
		float x = Random.Range((-Screen.width * 0.5f) + _scale, (Screen.width * 0.5f) - _scale);
		me.anchoredPosition = new Vector2(Random.Range(-Screen.width * 0.5f, Screen.width * 0.5f), 50);
		scale = s;
		me.sizeDelta = new Vector2(_scale, _scale);
		colider.radius = _scale * 0.5f;

		img.color = new Color(Random.Range(0f,1f),Random.Range(0,1f),Random.Range(0,1f));
	}

	void Update(){
		//двигаем обьект вниз, дельта линейная, чем меньше обьект, тем он быстрее
		me.position += Vector3.down * Game.curSpeedDelta * Mathf.Lerp(Game.maxSpeed, Game.minSpeed, scale / Game.maxRadius); 

		//после того как обьект скрылся удаляем
		if (-me.anchoredPosition.y > Screen.height + (Game.defaultSpriteScale * scale)){
			Destroy(gameObject);
		}
	}

	public void OnClick(){
		if (Game.state == GameState.inGame){
			Game.userScore += (int)Mathf.Lerp(Game.maxSpeed, Game.minSpeed, scale / Game.maxRadius); //за клики назначаем очки, взял теже цифры что и скорость
			Destroy(gameObject);
		}
	}

	void ChangeState(GameState state){ //изменился стейт - удаляем все круги
		Destroy(gameObject);
	}

	void OnDestroy(){
		EventAggregator.bubbles.Del(ChangeState); //отписываемся
		if (texture != null) Destroy(texture);
	}
}
