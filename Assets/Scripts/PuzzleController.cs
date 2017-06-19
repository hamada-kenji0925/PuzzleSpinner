using UnityEngine;
using System.Collections;

public class PuzzleController : MonoBehaviour {

	//パズル管理-二次元配列宣言-
	private Sprite[,] PuzzleBlockAry;
	//Sprite格納用
	private Sprite[] PuzzleSprite;

	//パズル列数は外部入力
	public int PuzzleX = 1;
	public int PuzzleY = 1;

	//パズル二次元配列-要素数格納-
	//public Vector2 Puzzle_ver_sid;

	// Use this for initialization
	void Start () {
		//Inspectorで指定された縦＊横で二次元配列宣言
		this.PuzzleBlockAry = new Sprite[this.PuzzleX,this.PuzzleY];
		//[Piece]spriteをSliceした分格納
		this.PuzzleSprite = Resources.LoadAll<Sprite> ("Piece");

		//宣言された二次元配列にランダムに[Piece]sprite格納
		for (int i = 0; i < this.PuzzleY; i++) {
			for (int j = 0; j < this.PuzzleX; j++) {
				//PuzzleBlockAry配列に[Piece]spriteの中身をランダムに格納
				this.PuzzleBlockAry [i, j] = this.PuzzleSprite[Random.Range(0,this.PuzzleSprite.Length-1)];
			}
		}

		Debug.Log (this.PuzzleBlockAry[0,0]);


	}

	// Update is called once per frame
	void Update () {
	
	}

//	指定された要素数のパズルブロックを返す関数
//	private void GetPuzzleBlock(int x,int y){
//		this.PuzzleBlockAry = new Sprite [x, y];
//	}
}
