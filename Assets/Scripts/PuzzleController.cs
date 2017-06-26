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
	private List<Vector2> searchAfterBlock = new List<Vector2>();

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

			//3つ以上一致した場合、該当パズルの配列要素をnullにする
			deletePuzzleBlock (searchAfterBlock);

			//Judge変数の初期化
			Judge = false;
		}

		//使用済みListの初期化
		searchAfterBlock.Clear ();

	}

	/// <summary>
	/// 縦横列の数を与えるとその数に応じたパズルブロックを画面上に生成する関数
	/// </summary>
	/// <param name="y">パズル縦列</param>
	/// <param name="x">パズル横列</param>
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
		Vector2 judgePuzzleBlock = new Vector2 (selectY, selectX);

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

		//関数内で使用したListの初期化(searchAfterBlockはグローバル変数の為update内にてClear処理実施
		searchPuzzleBlock.Clear ();

		//一致したパズル数が３以上ならばtrueを返す
		if (countPuzzle >= 3) {
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
			PuzzleBlockAry [blockAryY, blockAryX].Init (5, new Vector2 (blockAryY, blockAryX));
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

					if (upBlockColor != 5) {

						//取得したブロック色を自ブロックに設定
						block.Init (upBlockColor, new Vector2 (blockY, blockX));
						//上ブロックの色を自ブロック色に設定(自ブロックは必ず透明(5)の為、直打ち設定
						PuzzleBlockAry [blockY + 1, blockX].Init (5, new Vector2 (blockY, blockX));
					} else {
						for (int i = 0; i < (PuzzleY - 1); i++) {

							//Y軸がオーバーフローして参照されないように条件指定する
							if ((blockY + i) < (PuzzleY - 1)) {

								//上ブロック色を取得
								int BlockColor = PuzzleBlockAry [blockY + i, blockX].ColorNum;

								if(BlockColor != 5){
									//取得したブロック色を自ブロックに設定
									block.Init (BlockColor, new Vector2 (blockY, blockX));
									//上ブロックの色を自ブロック色に設定(自ブロックは必ず透明(5)の為、直打ち設定
									PuzzleBlockAry [blockY + i, blockX].Init (5, new Vector2 (blockY + i, blockX));
								}
							}

						}
					}


					//確認するブロックのY軸が最大要素数か？ && ブロックカラーが透明か？　なら色をランダムに指定
				} else if (blockY == (PuzzleY - 1) && block.ColorNum == 5) {
					PuzzleBlockAry [blockY, blockX].Init (Random.Range (0, 5), new Vector2 (blockY, blockX));
				}
			}


			//配列をもう一度見直し、img透明が含まれていないことを確認する
			foreach (PuzzleBlock pb in PuzzleBlockAry) {

				//img透明が存在するか確認
				if (pb.ColorNum != 5) {
					NotTransparent = true;
				} else {
					Debug.Log ("一つでも透明ブロックが存在している");
					//一つでも透明ブロックがあればフラグをfalse
					NotTransparent = false;
					continue;
				}
			}

		}

	}
}

//			//PuzzleBlockAry要素数分loop
//			foreach (PuzzleBlock block in PuzzleBlockAry) {
//
//				//比較の為、配列からX・Y軸、色番号を取得
//				int blockX = (int)block.BlockPosition.x;
//				int blockY = (int)block.BlockPosition.y;
//				Debug.Log ("blockY;" + blockY);
//				Debug.Log ("blockX;" + blockX);
//
//				//block自体のimgが透明か確認
//				if (block.ColorNum == 5) {
//
//					//確認するブロックY軸が配列番号をオーバーしていないか確認
//					if (blockY < PuzzleY - 1) {
//
//						//上座標ブロックの色を取得
//						int upBlockColor = PuzzleBlockAry [blockY + 1, blockX].ColorNum;
//
//						//入れ替える上座標ブロックがある為、上ブロックの色を代入
//						PuzzleBlockAry [blockY + 1, blockX].Init (block.ColorNum, new Vector2 (blockY + 1, blockX));
//						block.Init (upBlockColor, new Vector2 (blockY, blockX)); 
//					} else {
//						//入れ替える上座標ブロックがない為、色をランダムに指定
//						block.Init (Random.Range (0, 5), new Vector2 (blockY, blockX));
//					}
//				}
//			}
//
//
//			//配列をもう一度見直し、img透明が含まれていないことを確認する
//			foreach (PuzzleBlock pb in PuzzleBlockAry) {
//				//比較の為、配列からX・Y軸、色番号を取得
//				int blockX = (int)pb.BlockPosition.x;
//				int blockY = (int)pb.BlockPosition.y;
//				int BlockColor = PuzzleBlockAry [blockY, blockX].ColorNum;
//
//				//img透明が存在するか確認
//				if (BlockColor != 5) {
//					NotTransparent = true;
//				} else {
//					//一つでも透明ブロックがあればフラグをfalse
//					NotTransparent = false;
//					continue;
//				}
//			}

//		}

//		//透明画像の存在フラグ
//		bool NotTransparent = false;
//		//loop回数カウント用
//		int loopCount = 0;
//
//
//		//透明画像が存在する限りloop
//		while (NotTransparent == false) {
//			
//			//PuzzleBlockAryの要素数分をloop
//			foreach (PuzzleBlock block in PuzzleBlockAry) {
//
//				//判定するブロック座標がY軸Maxではないか && 軸ブロックimgは透明か確認
//				if (block.BlockPosition.y < PuzzleY - 1 && block.ColorNum == 5) {
//						
//					//上座標のブロックimgと透明ブロックimgの差し替え
//					int blockX = (int)block.BlockPosition.x;
//					int blockY = (int)block.BlockPosition.y;
//					int upBlockColor = PuzzleBlockAry [blockY + 1, blockX].ColorNum;
//					Debug.Log ("block座標：" + block.BlockPosition);
//
//					//上座標のブロックimgが透明ならloop
////					while (upBlockColor == 5) {
////						Debug.Log ("upBlockColor_loop");
////						//透明ブロックimgと上ブロックimgの差し替えし
////						PuzzleBlockAry [blockY, blockX].Init (upBlockColor, new Vector2 (blockY + loopCount, blockX));
////						PuzzleBlockAry [blockY + 1, blockX].Init (5, new Vector2 (blockY, blockX));
////
////						//loopCountが指定したパズルY軸を越えれば
////						if (loopCount > PuzzleY) {
////
////							//処理をスキップする
////							continue;
////						}
////
////						//loop回数カウント
////						loopCount++;
////				
////					}
//
//					//上座標のブロックimgが透明か確認
//					if (upBlockColor != 5) {
//						//透明ブロックimgと上ブロックimgの差し替え
//						PuzzleBlockAry [blockY, blockX].Init (upBlockColor, new Vector2 (blockY, blockX));
//
//						PuzzleBlockAry [blockY + 1, blockX].Init (5, new Vector2 (blockY + 1, blockX));
//
//					}
//
//				} else {
//
//					//差し替えるブロック座標が存在しない為、Randomでimg設定する
//					Debug.Log("OK");
//
//				}
//
//			}
//
//			//img入れ替え後、透明imgは存在しないかを確認
//			foreach (PuzzleBlock block in PuzzleBlockAry) {
//
//				//透明ブロックが存在しなければ
//				if (block.ColorNum != 5) {
//					NotTransparent = true;
//				} else {
//					NotTransparent = false;
//				}
//			}
//
//			//暫定処理
//			NotTransparent = true;
//		}
//
//	}
//
//}
