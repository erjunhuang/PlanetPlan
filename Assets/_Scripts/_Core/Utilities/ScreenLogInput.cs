using UnityEngine.UI;
using Core.Utilities;
public class ScreenLogInput:Singleton<ScreenLogInput>{
    private Text showText;
	// Use this for initialization
	void Start () {
        showText = GetComponent<Text>();
        showText.text = "";
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void showLog( string str) {
        if (showText) {
            showText.text = str;
        }
    }
}
