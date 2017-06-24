using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListController_test : MonoBehaviour {

	private List<Vector2> veclist = new List<Vector2>();
	private Vector2 hoge = new Vector2 (2, 2);

	// Use this for initialization
	void Start () {
		veclist.Add(new Vector2(1,1));
		veclist.Add(new Vector2(2,2));
		veclist.Add(new Vector2 (3, 3));

		for (int i = 0; i < 3; i++) {
			Debug.Log (veclist [i]);
		}

		if (veclist.Contains (hoge) == true) {
			Debug.Log ("hogeが見つかりました");
		} else {
			Debug.Log ("見つかりません！！！！");
		}

		Debug.Log ("要素数は" + veclist.Count);

		//		if (data.Contains(textBox_Input.Text) == true) {
//			textBox_Output.Text += "一致している要素がありました。\r\n";
//		}
//		else {
//			textBox_Output.Text += "一致している要素はありませんでした。\r\n";
//		}
//
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
