using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Bubble : MonoBehaviour {

	//создаем кешированную ссылку до transform
	private RectTransform myTr;
	private RectTransform me { get { if (myTr == null) myTr = GetComponent<RectTransform>(); return myTr; } }
	
	public RawImage img;
	public CircleCollider2D colider;
	public float scale;
	public float speed = 1;

	void Start(){
		EventAggregator.bubbles.Subscribe(ChangeState); //подписываемся на изменения
	}

	public void Init(float s, Texture2D tx){
		float _scale = Game.defaultSpriteScale * s;
		float x = Random.Range((-Screen.width * 0.45f), (Screen.width * 0.45f));
		me.anchoredPosition = new Vector2(Random.Range(-Screen.width * 0.5f, Screen.width * 0.5f), 250);
		scale = s;
		me.sizeDelta = new Vector2(_scale, _scale);
		colider.radius = _scale * 0.5f;
		img.texture = tx;
		speed = Game.curSpeedDelta * Mathf.Lerp(Game.maxSpeed, Game.minSpeed, scale / Game.maxRadius); //дельта линейная, чем меньше обьект, тем он быстрее
	}

	void FixedUpdate(){
		//двигаем обьект вниз, 
		me.position += Vector3.down * speed;

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
	}
}
