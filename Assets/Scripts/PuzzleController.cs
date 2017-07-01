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

	//ブロック生成した際のオブジェクトPositionを記憶するList生成
	private List<Vector3> createBlockPos = new List<Vector3>();

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

			//3つ以上一致した場合、該当パズルの配列要素をnullにする
			//deletePuzzleBlock (searchAfterBlock);

			//暫定処理（最終的にはこちら関数へ以降予定========================================>
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
				createBlockPos.Add(pz.transform.localPosition);
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
					PuzzleBlock targetPuzzleBlock = this.PuzzleBlockAry[(int)targetIndex.y,(int)targetIndex.x];
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
	/// 引数で渡されたListに入っている座標のパズル配列を削除する関数
	/// </summary>
	/// <param name="searchNormalBlock">探索後のパズル情報List</param>
	private void deletePuzzleBlock (List<Vector2> searchNormalBlock)
	{

		//透明画像の存在フラグ
		bool NotTransparent = false;

		//一致したパズルブロックのimgを透明画像に差し替える
		for (int i = 0; i < searchNormalBlock.Count; i++) {
			//一致したパズルブロック座標を分解して代入
			int blockAryX = (int)searchNormalBlock [i].x;
			int blockAryY = (int)searchNormalBlock [i].y;

			//該当するPuzzleBlockAry配列の画像に透明画像を設定する
			PuzzleBlockAry [blockAryY, blockAryX].Init (5, new Vector2 (blockAryX, blockAryY));
		}

		//透明画像が存在している場合にloop
		while (NotTransparent == false) {

			//PuzzleBlockAry要素数分loop
			foreach (PuzzleBlock block in PuzzleBlockAry) {

				//比較の為、配列からX・Y軸、色番号を取得
				int blockX = (int)block.BlockPosition.x;
				int blockY = (int)block.BlockPosition.y;

				//確認するブロックのY軸が要素数をオーバーしていないか && ブロックカラーは透明か確認
				if (blockY < (PuzzleY - 1) && block.ColorNum == 5) {

					//上ブロック色を取得
					int upBlockColor = PuzzleBlockAry [blockY + 1, blockX].ColorNum;

					//上のブロックが透明でなかった場合
					if (upBlockColor != 5) {

						//取得したブロック色を自ブロックに設定
						block.Init (upBlockColor, new Vector2 (blockX, blockY));
						//上ブロックの色を透明色に設定(自ブロックは必ず透明(5)の為、直打ち設定
						PuzzleBlockAry [blockY + 1, blockX].Init (5, new Vector2 (blockX, blockY + 1));
					} else {
						//上のブロックが透明だった場合
						for (int i = 0; i < (PuzzleY - 1); i++) {

							//Y軸がオーバーフローして参照されないように条件指定する
							if ((blockY + i) < (PuzzleY - 1)) {

								//上ブロック色を取得
								int BlockColor = PuzzleBlockAry [blockY + i, blockX].ColorNum;

								//上のブロックが透明でなかった場合
								if(BlockColor != 5){
									//取得したブロック色を自ブロックに設定
									block.Init (BlockColor, new Vector2 (blockX, blockY));
									//上ブロックの色を自ブロック色に設定(自ブロックは必ず透明(5)の為、直打ち設定
									PuzzleBlockAry [blockY + i, blockX].Init (5, new Vector2 (blockX, blockY + i));
								}
							}

						}
					}


					//確認するブロックのY軸が最大要素数か？ && ブロックカラーが透明か？　なら色をランダムに指定
				} else if (blockY == (PuzzleY - 1) && block.ColorNum == 5) {
					PuzzleBlockAry [blockY, blockX].Init (Random.Range (0, maxPiece), new Vector2 (blockX, blockY));
				}
			}


			//配列をもう一度見直し、img透明が含まれていないことを確認する
			foreach (PuzzleBlock pb in PuzzleBlockAry) {

				//img透明が存在するか確認
				if (pb.ColorNum != 5) {
					NotTransparent = true;
				} else {
					//一つでも透明ブロックがあればフラグをfalse
					NotTransparent = false;
					break;
				}
			}

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
		//移動させるブロックPosX,YとGameObjectをListへ格納し、適当な位置へ移動
		for (int i = 0; i < replaceBlock.Count; i++) {

			//Vector2情報を元に該当するゲームオブジェクトに当たるまでloop
			foreach(PuzzleBlock replacePuzzle in PuzzleBlockAry){
				//Vector2情報の比較(一致したブロック座標　＝＝　パズル配列のblockPosition)
				if(replaceBlock[i] == replacePuzzle.BlockPosition){

					//消していくブロックの座標を記憶(PosX,Y）
					deleteBlockX.Add(replacePuzzle.transform.localPosition.x);
					deleteBlockY.Add(replacePuzzle.transform.localPosition.y);

					//該当するブロック配列のGameObject情報を取得
					moveBlock.Add (PuzzleBlockAry [(int)replacePuzzle.BlockPosition.y,
						(int)replacePuzzle.BlockPosition.x].gameObject);

					//一致したブロックを適当なところへ退避
					PuzzleBlockAry [(int)replacePuzzle.BlockPosition.y,
						(int)replacePuzzle.BlockPosition.x].transform.position = new Vector2 (
						5, 5);
				}

			}
		}

		//①-②List.replaceBlockの中身をソート

		//暫定でreplaceBlock[0を最小値としてソートしていく
		int minNum;
		//sort時の入れ替え時に退避させる変数
		Vector2 tempBlock;

		Debug.Log ("ソート前");
		for (int i = 0; i < replaceBlock.Count; i++) {
			Debug.Log (replaceBlock [i]);
		}

		for (int i = 0; i < replaceBlock.Count; i++) {

			minNum = i;

			for(int j = i+1;j < replaceBlock.Count;j++) {
				//minBlockの値より
				if (
					((int)(replaceBlock [minNum].y * 10) + (int)replaceBlock [minNum].x) >
					(int)(replaceBlock [j].y * 10) + (int)(replaceBlock [j].x)) {

					//最小値だった要素を入れ替えておく
					minNum = j;
					 
				
				}
				tempBlock = replaceBlock [i];
				replaceBlock [i] = replaceBlock [minNum];
				replaceBlock [minNum] = tempBlock;


			}

		}

		Debug.Log ("ソート後");
		for (int i = 0; i < replaceBlock.Count; i++) {
			Debug.Log (replaceBlock [i]);
		}


		//②移動されたブロックを同じPosX上の冗談に順番に積み上げていく
//		for (int i = PuzzleY; i < (PuzzleY + PuzzleY); i++) {
//			for (int j = PuzzleX; i < (PuzzleX + PuzzleX); j++) {
//
//				//移動されているブロックを同じPosX上に移動させる（PosY+1~始める）
//				if(replaceBlock
//			}
//		}
			

//		//createBlockPosの要素番号カウント
//		int count = 0;
//
//		//一致したブロックの整列
//		foreach (GameObject go in moveBlock) {
//			for (int i = 0; i < PuzzleY; i++) {
//				for (int j = 0; j < PuzzleX; j++) {
//					if (createBlockPos [count] != PuzzleBlockAry [i, j].transform.localPosition) {
//						Debug.Log("一致しなかった配列は" + i + "," + j + "です");
//					}
//				}
//			}
//			count++;
//		}



		//②一致したパズル座標を上へ持っていく && ③移動させたブロック色をランダムに変更
//		for(int i = 0;i < replaceBlock.Count;i++){
//		
//			//取得したGameObject情報を元にPositionを設定
//			moveBlock[i].transform.localPosition = new Vector2 (
//				deleteBlockX [i],
//				(float)(PuzzleY + i) * (margin + blockLength)
//			);
//
//			//色をランダムに設定
//			PuzzleBlockAry [(int)deleteXY [i].y, (int)deleteXY [i].x].Init(
//				Random.Range(0,maxPiece),
//				new Vector2(replaceBlock[i].x,replaceBlock[i].y)
//			);
//
//		}

		//⬇️アニメーションは正常に動くがPuzzleBlockAryのオブジェクト情報が入れ替わっていない為
		//上に移動させたブロックがおりてきてしまうので
		//上に移動させたときに配列を詰める処理をしないと正常に動かない

		//③整列させたい
		//とりあえず今はそのままにしておこう・・・

		//createBlockPosの要素番号カウント
//		int count = 0;

		//④最後ブロック移動アニメーションを・・・
//		for (int i = 0; i < (PuzzleY); i++) {
//			for (int j = 0; j < (PuzzleX); j++) {
//
//				//PuzzleBlockAry配列を順番に比較し生成された時のポジションとズレがあれば
//				//上ブロックを下へ移動させる
//				if (createBlockPos [count] == PuzzleBlockAry [i, j].transform.localPosition) {
//					//Debug.Log ("配列番号[" + i + "," + j + "]でポジション一致しました");
//					//Debug.Log("createBlockPos" + createBlockPos [count]);
//					//Debug.Log("PuzzleBlockAry" + PuzzleBlockAry[i,j].transform.localPosition);
//				//上ブロックPositionが生成された時と一致していれば上ブロックを消したブロック位置に移動
//				} else if (createBlockPos [count + PuzzleX] == PuzzleBlockAry [i + 1, j].transform.localPosition){
//					//生成されたポジションと現在ポジションに差がある場合
//					//アニメーションにより移動させる
//					GameObject PUZZLE = PuzzleBlockAry [i + 1, j].gameObject;
//					PUZZLE.GetComponent<RectTransform> ().DOLocalMoveY (
//						createBlockPos [count].y,
//						1,
//						false
//					);
//
//				}
//
//				//Debug.Log("createBlockPos" + createBlockPos [count]);
//				//Debug.Log("PuzzleBlockAry" + PuzzleBlockAry[i,j].transform.localPosition);
//
//
//				//createBlockPosの要素番号インクリメント
//				count++;
//			}
//		}

		//⑤blockPositionの番号を整理し直す

		//消していくブロックを画面外に一旦移動
	}
}

