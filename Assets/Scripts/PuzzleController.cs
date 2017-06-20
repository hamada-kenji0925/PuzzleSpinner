using UnityEngine;
using System.Collections;

public class PuzzleController : MonoBehaviour {

//Prefabで管理してみる
//	//パズル格納用二次元配列
//	private GameObject[,] PuzzleBlockAry;
//	public Sprite[] PuzzleSprite;
//
//	//パズル縦・横数指定
//	public int PuzzleX;
//	public int PuzzleY;
//
//	void Start () {
//		//Inspectorで指定された縦＊横で二次元配列宣言
//		this.PuzzleBlockAry = new GameObject[this.PuzzleY,this.PuzzleX];
//
//		//宣言された二次元配列にランダムに[Piece]sprite格納
//		for (int i = 0; i < this.PuzzleY; i++) {
//			for (int j = 0; j < this.PuzzleX; j++) {
//				//PuzzleBlockAry配列に[Piece]spriteの中身をランダムに格納
//				//this.PuzzleBlockAry [i, j] = this.PuzzleSprite[Random.Range(0,this.PuzzleSprite.Length-1)];
//			}
//		}
//	
//	}

	//パズル管理-二次元配列宣言-
	private Sprite[,] PuzzleBlockAry;
	//Sprite格納用
	private Sprite[] PuzzleSprite;

	//パズル列数は外部入力
	public int PuzzleX = 1;
	public int PuzzleY = 1;

	//spriteのGameObjectを宣言
	public GameObject gameObject;

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

		//Prefabを生成
		this.gameObject = (GameObject)Resources.Load (this.PuzzleBlockAry [0, 0].name);

		Debug.Log (this.PuzzleBlockAry[0,0].name);

		GameObject go = Instantiate (gameObject) as GameObject;

//		
//		go.transform.position = new Vector2 (0, 0);
	}

	// Update is called once per frame
	void Update () {
	
	}

//	指定された要素数のパズルブロックを返す関数
//	private void GetPuzzleBlock(int x,int y){
//		this.PuzzleBlockAry = new Sprite [x, y];
//	}
}
