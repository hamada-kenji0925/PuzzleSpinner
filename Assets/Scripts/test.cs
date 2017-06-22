//using UnityEngine;
//using System.Collections;
//
//public class PuzzleController : MonoBehaviour
//{
//
//	//パズル管理-二次元配列宣言-
//	private PuzzleBlock[,] PuzzleBlockAry;
//
//	[SerializeField]
//	private PuzzleBlock _puzzleBlockPrefab;
//
//	[SerializeField]
//	private Transform _puzzleBlockParent;
//
//	//パズル列数はInspector上で指定
//	[SerializeField]
//	private int PuzzleX = 1;
//	[SerializeField]
//	private int PuzzleY = 1;
//
//
//	// Use this for initialization
//	void Start ()
//	{
//		//指定した列数のパズルブロック要求
//		GetPuzzleBlocks(PuzzleX,PuzzleY);
//	}
//
//	// Update is called once per frame
//	void Update ()
//	{
//		//クリックしたオブジェクト情報の取得
//		GameObject go = GetClickPzzleBlock();
//		//この時点ではクリックしたオブジェクト情報は正常に取得できています。
//
//
//		//クリック判定
//		if (go != null) {
//			//クリックされたパズルピース判定
//			bool Judgement = JudgePuzzleBlock(go);
//		}
//	}
//
//	/// <summary>
//	/// クリックされたパズルブロックの縦横上下が同色かつ３つ以上存在するかを判定する関数
//	/// </summary>
//	/// <returns>The puzzle block.</returns>
//	/// <param name="puzzleBlock">Puzzle block.</param>
//	private bool JudgePuzzleBlock(GameObject puzzleBlock){
//		//同色カウント変数
//		int count = 1;
//
//		//選択されたBlockのポジション(二次元配列の要素数)を取得
//		int selectX = (int)puzzleBlock.GetComponent<PuzzleBlock>().BlockPosition.x;
//		int selectY = (int)puzzleBlock.GetComponent<PuzzleBlock>().BlockPosition.y;
//		int selectColor = puzzleBlock.GetComponent<PuzzleBlock> ().ColorNum;
//
//		//選択された上下左右のパズルを確認に同色かを判断
//		for (int i = 0; i < PuzzleY; i++) {
//			for (int j = 0; j < PuzzleX; j++) {
//
//			}
//		}
//
//		//forで回すよりピンポイントに上下左右を確認する
//		//上確認
//		if (PuzzleBlockAry [selectY, selectX+1].GetComponent<PuzzleBlock>().ColorNum == selectColor) {
//			Debug.Log ("上の色は選択されたオブジェクトと同色です");
//		}
//		//下確認
//
//		//左確認
//
//		//右確認
//
//		//未実装（暫定的にtrueを返す
//		return true;
//	}
//
//	/// <summary>
//	/// クリックしたオブジェクト情報を返す関数
//	/// </summary>
//	/// <returns>The click object.</returns>
//	private GameObject GetClickPzzleBlock() {
//		GameObject result = null;
//		// 左クリックされた場所のオブジェクトを取得
//		if(Input.GetMouseButtonDown(0)) {
//			Vector2 tapPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//			Collider2D collision2d = Physics2D.OverlapPoint(tapPoint);
//			if (collision2d) {
//				result = collision2d.transform.gameObject;
//			}
//		}
//		return result;
//	}
//
//	/// <summary>
//	/// 縦横列の数を与えるとその数に応じたパズルブロックを画面上に生成する関数
//	/// 引数：パズル縦列x、横列y
//	/// 戻り値：void
//	/// </summary>
//	private void GetPuzzleBlocks(int x,int y){
//		//Inspectorで指定された縦＊横で二次元配列宣言
//		PuzzleBlockAry = new PuzzleBlock[y,x];
//
//		//ブロックとブロックの隙間
//		float margin = 5f;
//
//		//ブロックの大きさ
//		float blockLength = 80f;
//
//		//宣言された二次元配列にランダムに_puzzleBlockPrefab格納
//		for (int i = 0; i < y; i++) {
//			for (int j = 0; j < x; j++) {
//				//PuzzleBlockプレハブを生成して親オブジェクトを指定する
//				PuzzleBlock pz = Instantiate (_puzzleBlockPrefab, _puzzleBlockParent);
//				//位置を決定
//				pz.transform.localPosition = new Vector2 (
//					i * (blockLength + margin),
//					j * (blockLength + margin)
//				);
//				//初期化する(色(0~4)情報を渡す、マス目位置を渡す)
//				pz.Init (Random.Range (0, 5), new Vector2 (i, j));
//			}
//		}
//	}
//
//
//}
