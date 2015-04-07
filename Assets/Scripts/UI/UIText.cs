using UnityEngine.UI;

public class UIText : UIWindow {
	public Text text;
	public string cmd;

	void LateUpdate(){
		if (cmd.Equals("score")){
			text.text = "Score: " + Game.userScore;
		}
		else if (cmd.Equals("time")){
			text.text = "Time: " + Game.GetReadableTime((int)Game.userTime);
		}
	}

}
