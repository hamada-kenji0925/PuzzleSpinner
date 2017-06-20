using UnityEngine;
using System.Collections;

public class PuzzleController : MonoBehaviour
{

	//パズル管理-二次元配列宣言-
	private PuzzleBlock[,] PuzzleBlockAry;

	[SerializeField]
	private PuzzleBlock _puzzleBlockPrefab;

	[SerializeField]
	private Transform _puzzleBlockParent;

	//パズル列数は外部入力
	public int PuzzleX = 1;
	public int PuzzleY = 1;

	// Use this for initialization
	void Start ()
	{
		//Inspectorで指定された縦＊横で二次元配列宣言
		this.PuzzleBlockAry = new PuzzleBlock[this.PuzzleX, this.PuzzleY];

		//ブロックとブロックの隙間
		float margin = 5f;

		//ブロックの大きさ
		float blockLength = 80f;

		//宣言された二次元配列にランダムに[Piece]sprite格納
		for (int i = 0; i < this.PuzzleY; i++) {
			for (int j = 0; j < this.PuzzleX; j++) {
				//PuzzleBlockプレハブを生成して親オブジェクトを指定する
				PuzzleBlock pz = Instantiate (_puzzleBlockPrefab, _puzzleBlockParent);
				//位置を決定
				pz.transform.localPosition = new Vector2 (
					i * (blockLength + margin),
					j * (blockLength + margin)
				);
				//初期化する(色(0~4)情報を渡す、マス目位置を渡す)
				pz.Init (Random.Range (0, 5), new Vector2 (i, j));
			}
		}
		Debug.Log (this.PuzzleBlockAry [0, 0]);
	}

	// Update is called once per frame
	void Update ()
	{
	
	}

	//	指定された要素数のパズルブロックを返す関数
	//	private void GetPuzzleBlock(int x,int y){
	//		this.PuzzleBlockAry = new Sprite [x, y];
	//	}
}
