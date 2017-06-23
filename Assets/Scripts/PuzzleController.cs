using UnityEngine;
using System.Collections;
//Listの為、定義
using System.Collections.Generic;

public class PuzzleController : MonoBehaviour
{

	//パズル管理-二次元配列宣言-
	private PuzzleBlock[,] PuzzleBlockAry;

	[SerializeField]
	private PuzzleBlock _puzzleBlockPrefab;

	[SerializeField]
	private Transform _puzzleBlockParent;

	//パズル列数はInspector上で指定
	[SerializeField]
	private int PuzzleX = 1;
	[SerializeField]
	private int PuzzleY = 1;

	//3色判定が終わった後のブロック座標を格納
	private List<Vector2> zahyoBlock = new List<Vector2>();

	//一致パズル数カウント用
	private int count;

	// Use this for initialization
	void Start ()
	{
		//指定した列数のパズルブロック要求
		GetPuzzleBlocks(PuzzleX,PuzzleY);
	}

	// Update is called once per frame
	void Update ()
	{
		//クリックしたオブジェクト情報の取得
		GameObject go = GetClickPzzleBlock();
		//クリック判定
		if (go != null) {
			bool judge = JudgePuzzleBlock(go);
		}
	}

	/// <summary>
	/// クリックされたパズルブロックの縦横上下が同色かつ３つ以上存在するかを判定する関数
	/// </summary>
	/// <returns>The puzzle block.</returns>
	/// <param name="puzzleBlock">Puzzle block.</param>
	private bool JudgePuzzleBlock(GameObject puzzleBlock){
		//初回クリックされた時には１代入
		count = 1;
		//カウントインクリメントで繰り返し処理を実行させる為、現在のカウント数を退避させる
		int countTmp = count;

		//選択されたBlockのポジション(二次元配列の要素数)を取得
		int selectX = (int)puzzleBlock.GetComponent<PuzzleBlock>().BlockPosition.x;
		int selectY = (int)puzzleBlock.GetComponent<PuzzleBlock>().BlockPosition.y;
		int selectColor = puzzleBlock.GetComponent<PuzzleBlock> ().ColorNum;

		//-初回-色判定したパズル座標を格納しておく配列の確認
		zahyoBlock.Add(new Vector2(selectY,selectX));

		//タッチされたパズルの上下左右の色を取得(重複しない99を指定
		int upColor = 99;
		int downColor = 99;
		int leftColor = 99;
		int rightColor = 99;

		upColor = GetUpColor (selectX, selectY);
		downColor = GetDownColor (selectX, selectY);
		leftColor = GetLeftColor (selectX, selectY);
		rightColor = GetRightColor (selectX, selectY);

		//タッチしたパズル色と上下左右に該当する色があるかどうか
		if (selectColor == upColor) {
			count += 1;
			//<List>zahyoBlockに色マッチした配列番号を格納
			selectY += 1;
			zahyoBlock.Add(new Vector2(selectY,selectX));
			//現在のカウント数を退避
			countTmp = count;
			count = BranchPuzzleBlock (new Vector2 (selectY, selectX), selectColor);
			//同色カウントインクリメントされるうちは繰り返し一致数を確認する
			while (countTmp < count) {
				count = BranchPuzzleBlock (new Vector2 (selectY, selectX), selectColor);
				countTmp = count;
			}

		}
		if (selectColor == downColor) {
			count += 1;
			//<List>zahyoBlockに色マッチした配列番号を格納
			selectY -= 1;
			zahyoBlock.Add(new Vector2(selectY,selectX));
			//現在のカウント数を退避
			countTmp = count;
			count = BranchPuzzleBlock (new Vector2 (selectY, selectX), selectColor);
			//同色カウントインクリメントされるうちは繰り返し一致数を確認する
			while (countTmp < count) {
				count = BranchPuzzleBlock (new Vector2 (selectY, selectX), selectColor);
				countTmp = count;
			}

		}
		if (selectColor == leftColor) {
			count += 1;
			//<List>zahyoBlockに色マッチした配列番号を格納
			selectX -= 1;
			zahyoBlock.Add(new Vector2(selectY,selectX));
			//現在のカウント数を退避
			countTmp = count;
			count = BranchPuzzleBlock (new Vector2 (selectY, selectX), selectColor);
			//同色カウントインクリメントされるうちは繰り返し一致数を確認する
			while (countTmp < count) {
				count = BranchPuzzleBlock (new Vector2 (selectY, selectX), selectColor);
				countTmp = count;
			}

		}
		if (selectColor == rightColor) {
			count += 1;
			//<List>zahyoBlockに色マッチした配列番号を格納
			selectX += 1;
			zahyoBlock.Add(new Vector2(selectY,selectX));
			//現在のカウント数を退避
			countTmp = count;
			count = BranchPuzzleBlock (new Vector2 (selectY, selectX), selectColor);
			//同色カウントインクリメントされるうちは繰り返し一致数を確認する
			while (countTmp < count) {
				count = BranchPuzzleBlock (new Vector2 (selectY, selectX), selectColor);
				countTmp = count;
			}

		}

		Debug.Log ("クリックされた同色の数は" + count);
		//３以上であれば正常判定を返す
//		if (count >= 3) {
//			return true;
//		} else {
//			return false;
//		}

		//Listの初期化
		zahyoBlock.Clear();

		//暫定処理
		return true;
	}

	private int BranchPuzzleBlock(Vector2 vec,int selectColor){
		int selectX = (int)vec.x;
		int selectY = (int)vec.y;

		//タッチされたパズルの上下左右の色を取得(重複しない99を指定
		int upColor = 99;
		int downColor = 99;
		int leftColor = 99;
		int rightColor = 99;

		upColor = GetUpColor (selectX, selectY);
		downColor = GetDownColor (selectX, selectY);
		leftColor = GetLeftColor (selectX, selectY);
		rightColor = GetRightColor (selectX, selectY);

		//タッチしたパズル色と上下左右に該当する色があるかどうか && 以前判定した座標かどうか
		if (selectColor == upColor && zahyoBlock.Exists(zahyoBlock => zahyoBlock != vec)) {
			count += 1;
			//<List>zahyoBlockに色マッチした配列番号を格納
			selectY += 1;
			zahyoBlock.Add(new Vector2(selectY,selectX));

		}
		if (selectColor == downColor && zahyoBlock.Exists(zahyoBlock => zahyoBlock != vec)) {
			count += 1;
			//<List>zahyoBlockに色マッチした配列番号を格納
			selectY -= 1;
			zahyoBlock.Add(new Vector2(selectY,selectX));
		}
		if (selectColor == leftColor && zahyoBlock.Exists(zahyoBlock => zahyoBlock != vec)) {
			count += 1;
			//<List>zahyoBlockに色マッチした配列番号を格納
			selectX -= 1;
			zahyoBlock.Add(new Vector2(selectY,selectX));
		}
		if (selectColor == rightColor && zahyoBlock.Exists(zahyoBlock => zahyoBlock != vec)) {
			count += 1;
			//<List>zahyoBlockに色マッチした配列番号を格納
			selectX += 1;
			zahyoBlock.Add(new Vector2(selectY,selectX));
		}


		return count;
	}


	/// <summary>
	/// クリックしたオブジェクト情報を返す関数
	/// </summary>
	/// <returns>The click object.</returns>
	private GameObject GetClickPzzleBlock() {
		GameObject result = null;
		// 左クリックされた場所のオブジェクトを取得
		if(Input.GetMouseButtonDown(0)) {
			Vector2 tapPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Collider2D collision2d = Physics2D.OverlapPoint(tapPoint);
			if (collision2d) {
				result = collision2d.transform.gameObject;
			}
		}
		return result;
	}

	/// <summary>
	/// 縦横列の数を与えるとその数に応じたパズルブロックを画面上に生成する関数
	/// 引数：パズル縦列x、横列y
	/// 戻り値：void
	/// </summary>
	private void GetPuzzleBlocks(int y,int x){
		//Inspectorで指定された縦＊横で二次元配列宣言
		PuzzleBlockAry = new PuzzleBlock[y,x];

		//ブロックとブロックの隙間
		float margin = 5f;

		//ブロックの大きさ
		float blockLength = 80f;

		//宣言された二次元配列にランダムに_puzzleBlockPrefab格納
		for (int i = 0; i < y; i++) {
			for (int j = 0; j < x; j++) {
				//PuzzleBlockプレハブを生成して親オブジェクトを指定する
				PuzzleBlock pz = Instantiate (_puzzleBlockPrefab, _puzzleBlockParent);
				//位置を決定
				pz.transform.localPosition = new Vector2 (
					i * (blockLength + margin),
					j * (blockLength + margin)
				);
				//初期化する(色(0~4)情報を渡す、マス目位置を渡す)
				pz.Init (Random.Range (0, 5), new Vector2 (i, j));

				//生成した情報を配列に格納
				PuzzleBlockAry[i,j] = pz;
			}
		}
	}

	/// <summary>
	/// x,y座標を引数にそのパズルの上方向の色を返す関数(該当なければ99を返す
	/// </summary>
	/// <returns>The up color.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	private int GetUpColor(int x,int y){
		int color;
		//要素が最大値を超えて参照しないようにする(例：PuzzleY=3の場合、要素2は一番上となる為参照してはいけない)
		if (y < (this.PuzzleY - 1)) {
			color = this.PuzzleBlockAry [x, y + 1].ColorNum;
		} else {
			color = 99;
		}

		return color;
	}

	/// <summary>
	/// x,y座標を引数にそのパズルの下方向の色を返す関数(該当なければ99を返す
	/// </summary>
	/// <returns>The down color.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	private int GetDownColor(int x,int y){
		int color;
		//要素が0を下回らないようにする
		if (y != 0) {
			color = this.PuzzleBlockAry [x, y - 1].ColorNum;
		} else {
			color = 99;
		}

		return color;
	}

	/// <summary>
	/// x,y座標を引数にそのパズルの左方向の色を返す関数(該当なければ99を返す
	/// </summary>
	/// <returns>The left color.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	private int GetLeftColor(int x,int y){
		int color;
		//要素が0を下回らないようにする
		if (x != 0) {
			color = this.PuzzleBlockAry [x - 1, y].ColorNum;
		} else {
			color = 99;
		}

		return color;
	}

	/// <summary>
	/// x,y座標を引数にそのパズルの右方向の色を返す関数(該当なければ99を返す
	/// </summary>
	/// <returns>The right color.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	private int GetRightColor(int x,int y){
		int color;

		//要素が最大値を超えて参照しないようにする(例：PuzzleX=3の場合、要素2は一番端となる為参照してはいけない)
		if (x < (this.PuzzleX-1)) {
			color = this.PuzzleBlockAry [x + 1, y].ColorNum;
		} else {
			color = 99;
		}

		return color;
	}

}
