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

	//-SerchBlock関数-探索が終わった後のパズル座標を格納するListの宣言（後に得点計算などで使用するかも
	private List<Vector2> serchAfterBlock = new List<Vector2>();

	//選択したパズルが３つ以上存在するか判定
	private bool Judge;

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
		GameObject go = GetClickPuzzleBlock();
		//クリック判定
		if (go != null) {
			Judge = SearchBlock(go);
		}

		if (Judge) {
			Debug.Log ("選択されたパズルは３つ以上存在します");
			Judge = false;
		}
	}

	/// <summary>
	/// 上下左右のしきい値を格納する配列
	/// </summary>
	Vector2[] puzzleAllOver = new Vector2[]{
		new Vector2(0,1),
		new Vector2(1,0),
		new Vector2(0,-1),
		new Vector2(-1,0)
	};

	/// <summary>
	/// 引数でもらったタッチ座標とカラーNoを元に同色ブロックを探索する
	/// </summary>
	/// <returns><c>true</c>, if block was serched, <c>false</c> otherwise.</returns>
	/// <param name="judgePuzzleBlock">Judge puzzle block.</param>
	/// <param name="judgePuzzleColor">Judge puzzle color.</param>
//	private bool SerchBlock(Vector2 judgePuzzleBlock,int judgePuzzleColor){
	private bool SearchBlock(GameObject puzzleBlock){

		//一致数をカウントする変数宣言
		int countPuzzle = 0;

		//引数で受け取ったGameObjectをVector2型、int型へ分解・代入
		int selectX = (int)puzzleBlock.GetComponent<PuzzleBlock>().BlockPosition.x;
		int selectY = (int)puzzleBlock.GetComponent<PuzzleBlock>().BlockPosition.y;
		Vector2 judgePuzzleBlock = new Vector2 (selectY, selectX);

		int judgePuzzleColor = puzzleBlock.GetComponent<PuzzleBlock> ().ColorNum;

		//探索必要なパズル座標を格納するListを宣言
		List<Vector2> searchPuzzleBlock = new List<Vector2>();

		//判定される座標を格納=>初回必ず引数を代入
		this.serchAfterBlock.Add(judgePuzzleBlock);

		//探索必要なパズル座標を追加
		searchPuzzleBlock.Add(judgePuzzleBlock);

		//探索必要なパズル座標が存在する限りloop
		while (searchPuzzleBlock.Count != 0) {

			//探索必要なパズル座標Listを要素数分loop
			for (int i = 0; i < searchPuzzleBlock.Count; i++) {

				//探索する軸となるパズル座標を宣言・代入
				Vector2 baseIndex = searchPuzzleBlock[i];
				//探索するパズル座標が重複しないようListから削除する
				searchPuzzleBlock.Remove(baseIndex);

				//軸となるパズル座標から上下左右分4回loop
				for (int j = 0; j < puzzleAllOver.Length; j++) {

					//判定したいパズル座標を宣言
					Vector2 targetIndex = baseIndex + puzzleAllOver[j];

					//判定したいパズル座標は既に判定された座標か確認
					if (serchAfterBlock.Contains (targetIndex)) {
						//serchAfterBlockにtargetIndex座標が含まれていれば処理をスキップする
						continue;
					}

					//判定したいパズル座標はパズル配列のMin・Max要素をオーバーしているか確認
					if (targetIndex.x > PuzzleX - 1 ||
					   targetIndex.x < 0 ||
					   targetIndex.y > PuzzleY - 1 ||
					   targetIndex.y < 0) {

						//オーバーしていれば処理をスキップする
						continue;
					}

					//判定したいブロックを取得
					PuzzleBlock targetPuzzleBlock = this.PuzzleBlockAry[(int)targetIndex.y,(int)targetIndex.x];
					//判定したいブロック色を取得
					int targetColor = targetPuzzleBlock.ColorNum;

					//軸ブロックの色と一致判定したいブロック色がしているか確認
					if (judgePuzzleColor == targetColor) {
						
						//判定が終了したので判定される座標を格納するListに追加
						serchAfterBlock.Add(targetIndex);
						//新たに軸として判定すべきパズル座標がある為、Listへ追加
						searchPuzzleBlock.Add(targetIndex);

					}
				}
			}

		}

		//ログ出力
		Debug.Log("一致したパズル数は"+serchAfterBlock.Count);

		//探索終了したパズルブロック数を代入
		countPuzzle = serchAfterBlock.Count;

		//関数内で使用したListの初期化
		serchAfterBlock.Clear();
		searchPuzzleBlock.Clear ();

		//一致したパズル数が３以上ならばtrueを返す
		if (countPuzzle >= 3) {
			return true;
		} else {
			return false;
		}


	}

	/// <summary>
	/// クリックしたオブジェクト情報を返す関数
	/// </summary>
	/// <returns>The click object.</returns>
	private GameObject GetClickPuzzleBlock() {
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
}
