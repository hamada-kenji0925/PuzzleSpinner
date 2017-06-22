using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleBlock : MonoBehaviour
{
	/// <summary>
	/// イメージコンポーネント 
	/// </summary>
	[SerializeField]
	private Image _blockImage;

	/// <summary>
	/// 画像のリスト 
	/// </summary>
	[SerializeField]
	private List<Sprite> _blockSpriteList;

	/// <summary>
	/// マス目位置 
	/// </summary>
	[SerializeField]
	private Vector2 _blockPosition;

	/// <summary>
	/// getしか指定されていないので外部のクラスから情報を取得できるが、書き込みはできない 
	/// </summary>
	public Vector2 BlockPosition {
		get {
			return _blockPosition;
		}	
	}

	/// <summary>
	/// 色情報
	/// </summary>
	[SerializeField]
	private int _colorNum;

	/// <summary>
	/// getしか指定されていないので外部のクラスから情報を取得できるが、書き込みはできない 
	/// </summary>
	public int ColorNum {
		get{ return _colorNum; }
	}

	/// <summary>
	/// 初期化の関数
	/// </summary>
	/// <param name="colorNum">Color number.</param>
	public void Init (
		int colorNum,
		Vector2 blockPosition
	)
	{
		//画像を設定する
		_blockImage.sprite = _blockSpriteList [colorNum];	
		//色情報（数字を保持)
		_colorNum = colorNum;
		//現在のマス目位置を設定
		_blockPosition = blockPosition;
	}

}
