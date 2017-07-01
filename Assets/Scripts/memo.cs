///// <summary>
///// 引数で受け取ったパズルブロックを消して、上にあるブロックが折りてくるように見せるアニメーション
///// </summary>
///// <param name="searchNormalBlock">一致したブロック座標</param>
//private void ReplacePuzzleBlock(List<Vector2> replaceBlock){
//
//	//①消す必要のあるブロック座標とX、Y軸番号を記憶
//	//リスト-Pos X,Y-
//	List<float> deleteBlockX = new List<float>();
//	List<float> deleteBlockY = new List<float>();
//	//リスト-XY座標-
//	List<Vector2> deleteXY = new List<Vector2>();
//
//	//リスト-GameObject-
//	List<GameObject> moveBlock = new List<GameObject>();
//
//	//一致したいブロック座標があるだけloopして記憶
//	for (int i = 0; i < replaceBlock.Count; i++) {
//
//		//Vector2情報を元に該当するゲームオブジェクトに当たるまでloop
//		foreach(PuzzleBlock replacePuzzle in PuzzleBlockAry){
//			//Vector2情報の比較(一致したブロック座標　＝＝　パズル配列のblockPosition)
//			if(replaceBlock[i] == replacePuzzle.BlockPosition){
//
//				//消していくブロックの座標を記憶
//				deleteBlockX.Add(replacePuzzle.transform.localPosition.x);
//				deleteBlockY.Add(replacePuzzle.transform.localPosition.y);
//				//消していくブロックのblockPositionを記憶
//				deleteXY.Add (replacePuzzle.BlockPosition);
//
//			}
//
//		}
//	}
//
//	//②一致したパズル座標を上へ持っていく && ③移動させたブロック色をランダムに変更
//	for(int i = 0;i < replaceBlock.Count;i++){
//
//		//			//該当するブロック配列のGameObject情報を取得
//		moveBlock.Add(PuzzleBlockAry [(int)deleteXY [i].y, (int)deleteXY [i].x].gameObject);
//		//			//取得したGameObject情報を元にPositionを設定
//		//			moveBlock[i].transform.localPosition = new Vector2 (
//		//				deleteBlockX [i],
//		//				(float)(PuzzleY + i) * (margin + blockLength)
//		//			);
//		//
//		//			//色をランダムに設定
//		//			PuzzleBlockAry [(int)deleteXY [i].y, (int)deleteXY [i].x].Init(
//		//				Random.Range(0,maxPiece),
//		//				new Vector2(replaceBlock[i].x,replaceBlock[i].y)
//		//			);
//		Destroy(moveBlock[i]);
//	}
//
//	//⬇️アニメーションは正常に動くがPuzzleBlockAryのオブジェクト情報が入れ替わっていない為
//	//上に移動させたブロックがおりてきてしまうので
//	//上に移動させたときに配列を詰める処理をしないと正常に動かない
//
//	//③整列させたい
//	//とりあえず今はそのままにしておこう・・・
//
//	//createBlockPosの要素番号カウント
//	int count = 0;
//
//	//④最後ブロック移動アニメーションを・・・
//	//		for (int i = 0; i < (PuzzleY); i++) {
//	//			for (int j = 0; j < (PuzzleX); j++) {
//	//
//	//				//PuzzleBlockAry配列を順番に比較し生成された時のポジションとズレがあれば
//	//				//上ブロックを下へ移動させる
//	//				if (createBlockPos [count] == PuzzleBlockAry [i, j].transform.localPosition) {
//	//					//Debug.Log ("配列番号[" + i + "," + j + "]でポジション一致しました");
//	//					//Debug.Log("createBlockPos" + createBlockPos [count]);
//	//					//Debug.Log("PuzzleBlockAry" + PuzzleBlockAry[i,j].transform.localPosition);
//	//				//上ブロックPositionが生成された時と一致していれば上ブロックを消したブロック位置に移動
//	//				} else if (createBlockPos [count + PuzzleX] == PuzzleBlockAry [i + 1, j].transform.localPosition){
//	//					//生成されたポジションと現在ポジションに差がある場合
//	//					//アニメーションにより移動させる
//	//					GameObject PUZZLE = PuzzleBlockAry [i + 1, j].gameObject;
//	//					PUZZLE.GetComponent<RectTransform> ().DOLocalMoveY (
//	//						createBlockPos [count].y,
//	//						1,
//	//						false
//	//					);
//	//
//	//				}
//	//
//	//				//Debug.Log("createBlockPos" + createBlockPos [count]);
//	//				//Debug.Log("PuzzleBlockAry" + PuzzleBlockAry[i,j].transform.localPosition);
//	//
//	//
//	//				//createBlockPosの要素番号インクリメント
//	//				count++;
//	//			}
//	//		}
//
//	//⑤blockPositionの番号を整理し直す
//
//	//消していくブロックを画面外に一旦移動
//}