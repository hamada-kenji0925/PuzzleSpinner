//回答有難うございます。
//
//関数を条件によって再度呼び出したい場合などは
//繰り返したい関数外で繰り返し処理を実装する方が設計上良いということでしょうか？
//```
//void Start(){
//	int count;
//	bool judge = SampleFunc();
//	while(judge == false){
//		SampleFunc();
//
//		count++;
//		if(count > 25){
//			//25回以上繰り返す場合はbreakで抜ける
//			break;
//		}
//	}
//}
//
//private bool SampleFunc(){
//
//	何らかの処理
//
//	if(条件){
//		return true; 
//	}else{
//		return false;
//	}
//}
//```

//抽象的な質問で申し訳ないのですが
//関数内で条件を満たしてなければ、その関数の頭に戻るような
//処理を実装できることはUnity上で可能なのでしょうか？
//```
//private bool SampleFunc(){
//
//	何らかの処理
//
//	if(条件){
//		return true; 
//	}else{
//		SampleFuncの頭に戻って処理を再開する
//	}
//}
//```
//連続で質問すみません。
//Vector2型に格納した座標を分解して
//使用したいのですが可能なのでしょうか？
//
//例）
//```
//private Vector2 vec = new Vector2(1.0f,1,0f);
//float x = vecのX座標;
//float y = vecのY座標;
//```
//
//宜しくお願いします。

//回答有難うございます。
//
//今確認したのですがタップされた際に
//Listへ追加する際は正常に格納できているのですが
//2回目の格納時にエラーとなりうまく格納できていませんでした。
//
//```
//	zahyoBlock.Add(new Vector2(selectY,selectX));
//	selectY += 1;
//	zahyoBlock.Add(new Vector2(selectY,selectX));
//	Debug.Log (zahyoBlock [1]);		//正常に表示される
//	Debug.Log (zahyoBlock [2]);		//エラーとなって表示される
//```
//1回目と2回目の定義に違うはないと思うのですが
//どうでしょうか？


//本日も宜しくお願いします。
//
//Listの扱い方についてお聞きしたいのですが
//グローバル変数でVector2型のListを宣言し
//```
//public class PuzzleController : MonoBehaviour
//{
//	//3色判定が終わった後のブロック座標を格納
//	private List<Vector2> zahyoBlock = new List<Vector2>();
//```
//Update内のマウスクリックイベント関数内にて
//List名.Addとしているのですがエラーが発生して原因がわからず困っています。
//```
//	//selectX,Yには座標を指定
//	zahyoBlock.Add(new Vector2(selectY,selectX));
//```
//
//アドバイス頂けないでしょうか？

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
