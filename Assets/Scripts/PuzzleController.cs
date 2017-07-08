using UnityEngine;
using System.Collections;
//Listの為、定義
using System.Collections.Generic;
//DOTweenの為、定義
using DG.Tweening;

public class PuzzleController : MonoBehaviour
{

	//パズル管理-二次元配列宣言-
	private PuzzleBlock[,] PuzzleBlockAry;

	[SerializeField]
	private PuzzleBlock _puzzleBlockPrefab;

	[SerializeField]
	private Transform _puzzleBlockParent;

	//ブロックとブロックの隙間
	private float margin = 5f;
	//ブロックの大きさ
	private float blockLength = 80f;

	//パズル列数はInspector上で指定
	[SerializeField]
	private int PuzzleX = 1;
	[SerializeField]
	private int PuzzleY = 1;

	//生成されるPuzzleBlockの種類
	private int maxPiece = 4;

	//Puzzleの一致判定数
	private int matchPuzzlePiece = 3;

	//-SerchBlock関数-探索が終わった後のパズル座標を格納するListの宣言（後に得点計算などで使用するかも
	private List<Vector2> searchAfterBlock = new List<Vector2>();

	//選択したパズルが３つ以上存在するか判定
	private bool Judge;

	private int matchCount;

	//ブロック生成した際のオブジェクトPositionを記憶する配列
	private Vector3[,] createBlockPos;

	//ブロックが落ちるアニメーション時間
	private float animeTime = 1f;

	// Use this for initialization
	void Start ()
	{
		//生成時の座標を格納する二次元配列の要素数宣言
		createBlockPos = new Vector3[PuzzleY,PuzzleX];

		//指定した列数のパズルブロック要求
		GetPuzzleBlocks(PuzzleX,PuzzleY);
	}

	// Update is called once per frame
	void Update ()
	{

		//クリックしたオブジェクト情報の取得
		GameObject go = GetClickPuzzleBlock();
		//クリック判定
		if (go != null && Judge == false) {
			Judge = SearchBlock(go);

		}

		if (Judge) {

			//
			ReplacePuzzleBlock(searchAfterBlock);

			//スコアスクリプトへ値渡しするのに関数コール
			GetPuzzleScore();

			//Judge変数の初期化
			Judge = false;
		}

		//得点を随時更新していく
		matchCount = searchAfterBlock.Count;

		//使用済みListの初期化
		searchAfterBlock.Clear ();

	}

	/// <summary>
	/// 一致したパズル数を他スクリプトへ値渡しする関数
	/// </summary>
	/// <value>searchAfterBlockに格納されているCount数</value>
	public int GetPuzzleCount{
		get{ return matchCount; }
		private set{ matchCount = searchAfterBlock.Count; }
	}

	/// <summary>
	/// 一致したパズル数を他スクリプトへ値渡しする関数
	/// </summary>
	/// <returns>The puzzle score.</returns>
	/// <param name="matchPuzzleNumber">searchAfterBlockに格納されているCount数</param>
	public int GetPuzzleScore(){
		return searchAfterBlock.Count;
	}


	/// <summary>
	/// 縦横列の数を与えるとその数に応じたパズルブロックを画面上に生成する関数
	/// </summary>
	/// <param name="y">パズル縦列</param>
	/// <param name="x">パズル横列</param>
	private void GetPuzzleBlocks(int y,int x){
		//Inspectorで指定された縦＊横で二次元配列宣言
		PuzzleBlockAry = new PuzzleBlock[y,x];

		//宣言された二次元配列にランダムに_puzzleBlockPrefab格納
		for (int i = 0; i < y; i++) {
			for (int j = 0; j < x; j++) {
				//PuzzleBlockプレハブを生成して親オブジェクトを指定する
				PuzzleBlock pz = Instantiate (_puzzleBlockPrefab, _puzzleBlockParent);
				//位置を決定
				pz.transform.localPosition = new Vector2 (
					//i * (blockLength + margin),
					//j * (blockLength + margin),
					j * (blockLength + margin),
					i * (blockLength + margin)
				);

				//replacePuzzleBlock関数で使用する為、格納
				createBlockPos [i, j] = pz.transform.localPosition;
				//初期化する(色(0~4)情報を渡す、マス目位置を渡す)
				pz.Init (Random.Range (0, maxPiece), new Vector2 (j, i));

				//生成した情報を配列に格納
				PuzzleBlockAry[i,j] = pz;
			}
		}
	}

	/// <summary>
	/// クリックしたオブジェクト情報を返す関数
	/// </summary>
	/// <returns>クリックされた<GameObject型>のパズル情</returns>
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
	/// 上下左右のしきい値を格納する配列
	/// </summary>
	Vector2[] puzzleAllOver = new Vector2[]{
		new Vector2(0,1),
		new Vector2(1,0),
		new Vector2(0,-1),
		new Vector2(-1,0)
	};

	/// <summary>
	/// 引数でもらったGameObjectを元に同色ブロックを探索する
	/// </summary>
	/// <returns><c>true</c>, ３色以上並んでいる, <c>false</c> ３色以上並んでいない.</returns>
	/// <param name="puzzleBlock">Puzzle block.</param>
	private bool SearchBlock(GameObject puzzleBlock){

		//一致数をカウントする変数宣言
		int countPuzzle = 0;

		//引数で受け取ったGameObjectをVector2型、int型へ分解・代入
		int selectX = (int)puzzleBlock.GetComponent<PuzzleBlock>().BlockPosition.x;
		int selectY = (int)puzzleBlock.GetComponent<PuzzleBlock>().BlockPosition.y;
		Vector2 judgePuzzleBlock = new Vector2 (selectX, selectY);
		//Vector2 judgePuzzleBlock = new Vector2 (selectY, selectX);

		int judgePuzzleColor = puzzleBlock.GetComponent<PuzzleBlock> ().ColorNum;

		//探索必要なパズル座標を格納するListを宣言
		List<Vector2> searchPuzzleBlock = new List<Vector2>();

		//判定される座標を格納=>初回必ず引数を代入
		this.searchAfterBlock.Add(judgePuzzleBlock);

		//探索必要なパズル座標を追加
		searchPuzzleBlock.Add(judgePuzzleBlock);

		//探索必要なパズル座標が存在する限りloop
		while (searchPuzzleBlock.Count != 0) {

			//探索必要なパズル座標Listを要素数分loop
			for (int i = 0; i < searchPuzzleBlock.Count; i++) {

				//探索する軸となるパズル座標を宣言・代入
				Vector2 baseIndex = searchPuzzleBlock[i];

				//これから探索するパズル座標が重複して判定されないようListから削除する
				searchPuzzleBlock.Remove(baseIndex);

				//軸となるパズル座標から上下左右分4回loop
				for (int j = 0; j < puzzleAllOver.Length; j++) {

					//判定したいパズル座標を宣言
					Vector2 targetIndex = baseIndex + puzzleAllOver[j];

					//判定したいパズル座標は既に判定された座標か確認
					if (searchAfterBlock.Contains (targetIndex)) {
						//searchAfterBlockにtargetIndex座標が含まれていれば処理をスキップする
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
					PuzzleBlock targetPuzzleBlock = this.PuzzleBlockAry[(int)targetIndex.y,(int)targetIndex.x];			//< =========== ここでPuzzleBlockAryを元に判定したいブロック情報を取得している為、二回め以降異常が発生

					//判定したいブロック色を取得
					int targetColor = targetPuzzleBlock.ColorNum;

					//軸ブロックの色と一致判定したいブロック色が同色か確認
					if (judgePuzzleColor == targetColor) {

						//判定が終了したので一致座標を格納するListに追加
						searchAfterBlock.Add(targetIndex);
						//新たに軸として判定すべきパズル座標がある為、Listへ追加
						searchPuzzleBlock.Add(targetIndex);

					}
				}
			}

		}

		//探索終了したパズルブロック数を代入
		countPuzzle = searchAfterBlock.Count;
		Debug.Log ("SearchBlock関数を抜ける際のパズル一致数は" + countPuzzle);

		//関数内で使用したListの初期化(searchAfterBlockはグローバル変数の為update内にてClear処理実施
		searchPuzzleBlock.Clear ();

		//一致したパズル数がmatchPuzzlePiece以上ならばtrueを返す
		if (countPuzzle >= matchPuzzlePiece) {
			return true;
		} else {
			return false;
		}


	}

	/// <summary>
	/// 引数で受け取ったパズルブロックを消して、上にあるブロックが折りてくるように見せるアニメーション
	/// </summary>
	/// <param name="searchNormalBlock">一致したブロック座標</param>
	private void ReplacePuzzleBlock(List<Vector2> replaceBlock){

		//①消す必要のあるブロック座標とX、Y軸番号を記憶
		//リスト-Pos X,Y-
		List<float> deleteBlockX = new List<float>();
		List<float> deleteBlockY = new List<float>();

		//リスト-GameObject-
		List<GameObject> moveBlock = new List<GameObject>();

		//①引数とPuzzleBlockAry配列を比較し、一致すれば
		//移動させるブロックPosX,YとGameObjectをListへ格納する
		for (int i = 0; i < replaceBlock.Count; i++) {

			//Vector2情報を元に該当するゲームオブジェクトに当たるまでloop
			foreach(PuzzleBlock replacePuzzle in PuzzleBlockAry){
				//Vector2情報の比較(一致したブロック座標　＝＝　パズル配列のblockPosition)
				if(replaceBlock[i] == replacePuzzle.BlockPosition){

					//消していくブロックの座標を記憶(PosX,Y）									<=======いらないかも以後使ってない・・・・
					deleteBlockX.Add(replacePuzzle.transform.localPosition.x);
					deleteBlockY.Add(replacePuzzle.transform.localPosition.y);

					//該当するブロック配列のGameObject情報を取得
					moveBlock.Add (PuzzleBlockAry [(int)replacePuzzle.BlockPosition.y,
						(int)replacePuzzle.BlockPosition.x].gameObject);

				}

			}
		}

		//②一致したブロックを同じX軸の上段へ移動させる
		//X軸分loop
		for (int i = 0; i < PuzzleX; i++) {
			//積み上げていく数（初期値：0段目）
			int Reserve = 0;

			foreach (GameObject go in moveBlock) {
				//一致したブロックのX座標とiが一致すれば移動
				if (go.GetComponent<PuzzleBlock>().BlockPosition.x == i) {

					//一致したブロックをY軸の上段へ移動させる
					go.transform.localPosition = new Vector2 (
						go.transform.localPosition.x,
						(float)(PuzzleY + Reserve) * (margin + blockLength));

					//移動させたタイミングでカラーをランダムに変更
					go.GetComponent<PuzzleBlock> ().Init (
						Random.Range (0, 5),
						new Vector2 (
							go.GetComponent<PuzzleBlock> ().BlockPosition.x,
							go.GetComponent<PuzzleBlock> ().BlockPosition.y
						)
					);



					//同じX軸なら積み上げ段数をインクリメント
					Reserve++;
				}

			}
		}

		//③座標の移動・整列
		//PuzzleBlockAryを順番に確認していく
		for (int i = 0; i < PuzzleY; i++) {
			for (int j = 0; j < PuzzleX; j++) {

				//ブロック生成時のPosと現在のPosにズレがないか比較する
				if (createBlockPos [i, j] != PuzzleBlockAry [i, j].transform.localPosition) {
					//Debug.Log ("座標が一致しなかったブロックは" + createBlockPos [i, j]);

					//ズレているブロックの上を確認していく
					int upPosY = i+1;	//確認する時のY軸の一つ上を元にする

					//ズレていたBlockPositionから上をloopで確認
					while(upPosY < PuzzleY){

						//生成時のBlockPositionと現在のPuzzleBlockAryのPositionを比較
						if (PuzzleBlockAry [upPosY, j].transform.localPosition == createBlockPos [upPosY, j]) {
//							Debug.Log ("上ブロックは" + createBlockPos [upPosY, j]);
//							Debug.Log ("上ブロックを移動させる先の座標は" + createBlockPos [i, j]);
							//上に存在するゲームオブジェクトを下へ移動させる

							//該当ブロックのゲームオブジェクトを取得
//							GameObject go = PuzzleBlockAry [upPosY, j].gameObject;
//							go.GetComponent<RectTransform>().transform.DOLocalMove(
//								new Vector3(createBlockPos[i,j].x,createBlockPos[i,j].y),
//								animeTime,
//								false
//							).OnComplete(() => {
//								EndAnime = true;
//								Debug.Log(EndAnime);
//							});

							//StartCoroutine(ProcessAnime(go,i,j));

							//普通にトランスフォームさせると移動できている
							PuzzleBlockAry [upPosY, j].transform.localPosition = 
								new Vector3 (createBlockPos [i, j].x, createBlockPos [i, j].y);

							break;
							
						}

						upPosY++;

					}
				}

			}
			
		}

		//上段にあげたブロックを下げる処理
		for (int i = 0; i < PuzzleX; i++) {
			//ブロックを下ろす数
			int Down = 1;

			//上段へ積み上げたブロックを全て確認
			foreach (GameObject go in moveBlock) {

				//X軸が一致したブロックを下げる
				if (go.GetComponent<PuzzleBlock> ().BlockPosition.x == i) {

					//一段下げる
					go.transform.localPosition = new Vector2 (
						go.transform.localPosition.x,
						(float)(PuzzleY - Down) * (margin + blockLength));

					//同じX軸なら積み下げ段数をインクリメント
					Down++;
				}
			}
		}

		//正しいBlockPotionを設定する際に、順番にカラーを記憶しておく
		List<int> memoryColor = new List<int>();

		//PuzzleBlockAryのBlockPositionを正す
		for (int i = 0; i < PuzzleY;i++) {
			for (int j = 0; j < PuzzleX; j++) {

				//生成時ポジションと同じ位置にあるBlockのBlockPositionを書き換える為、PuzzleBlockAryをloopさせて確認
				foreach (PuzzleBlock pb in PuzzleBlockAry) {
					
					//1loopにつき、必ず一回は一致する
					if (createBlockPos [i, j] == pb.transform.localPosition) {

						//正しいBlockPositionを設定
						pb.Init (
							pb.ColorNum,
							new Vector2 (j, i)
						);

						//配置替えを行った最終的なColorNumを順番に格納（[0,0],[0,1],[0,2]〜
						memoryColor.Add (pb.ColorNum);
								
					}
				}

			}
		}

		//GameObjectの並びがバラバラになる為、一旦配列に格納されているGameObjectを削除 <==GameObjectの並び替え方法がわかればそちらを採用した方が断然良い
		for (int i = 0; i < PuzzleY; i++) {
			for (int j = 0; j < PuzzleX; j++) {
				Destroy (PuzzleBlockAry [i, j].gameObject);

			}
		}

		//List<memoryColor>の要素カウント用
		int colorCount = 0;

		//宣言された二次元配列にランダムに_puzzleBlockPrefab格納
		for (int i = 0; i < PuzzleY; i++) {
			for (int j = 0; j < PuzzleX; j++) {
				//PuzzleBlockプレハブを生成して親オブジェクトを指定する
				PuzzleBlock pz = Instantiate (_puzzleBlockPrefab, _puzzleBlockParent);
				//位置を決定
				pz.transform.localPosition = new Vector2 (
					j * (blockLength + margin),
					i * (blockLength + margin)
				);

				//replacePuzzleBlock関数で使用する為、格納
				createBlockPos [i, j] = pz.transform.localPosition;
				//初期化する(色(0~4)情報を渡す、マス目位置を渡す)
				pz.Init (memoryColor[colorCount], new Vector2 (j, i));

				colorCount++;

				//生成した情報を配列に格納
				PuzzleBlockAry[i,j] = pz;
			}
		}


	
	}



	private IEnumerator ProcessAnime(GameObject blockObject,int y,int x){
		//該当ブロックのゲームオブジェクトを取得
		blockObject.GetComponent<RectTransform>().transform.DOLocalMove(
			new Vector3(createBlockPos[y,x].x,createBlockPos[y,x].y),
			animeTime,
			false
		);

		yield return new WaitForSeconds(animeTime);
	}
}

