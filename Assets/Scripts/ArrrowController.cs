using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrrowController : MonoBehaviour {

	/// <summary>
	/// イメージコンポーネント
	/// </summary>
	[SerializeField]
	private Image _arrowImage;

	/// <summary>
	/// 画像のリスト 
	/// </summary>
	[SerializeField]
	private List<Sprite> _arrowSpriteList;

	// Use this for initialsization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		_arrowImage.sprite = _arrowSpriteList [0];
	}
}
