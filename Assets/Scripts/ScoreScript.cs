using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreScript : MonoBehaviour {

	//スクリプト宣言
	public PuzzleController puzzleController;

	//ScoreTextオブジェクト宣言
	private GameObject scoreText;

	//SCORE
	private int score = 0;

	//SCORE加算
	private int getScore = 0;

	// Use this for initialization
	void Start () {

		//GameObject取得
		this.scoreText = GameObject.Find("ScoreText");
	}

	// Update is called once per frame
	void Update (){

		this.getScore = puzzleController.GetPuzzleCount;

		//scoreが０以上であれば随時インクリメントしていく
		if (0 < this.getScore) {
			//PuzzleController.csより一致カウント数を取得
			score += getScore;
		}
		
		//表示
		this.scoreText.GetComponent<Text> ().text = "Score：" + score;
		
	}
}
