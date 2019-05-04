﻿using System.Collections;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bullet
{
	public class NormalHoleController : BulletController
	{
		protected int row;
		protected int column;

		protected override void Awake()
		{
			base.Awake();
			transform.localScale = new Vector2(HoleSize.WIDTH / originalWidth, HoleSize.HEIGHT / originalHeight) *
			                       LOCAL_SCALE;
		}

		// コンストラクタがわりのメソッド
		public virtual void Initialize(int row, int column, Vector2 holeWarningPosition)
		{
			this.row = row;
			this.column = column;
			transform.position = holeWarningPosition;
		}

		protected override void OnFail()
		{
		}

		// 当たり判定(holeの表示場所に、tileがあるかどうかを確認する)
		public IEnumerator Delete()
		{
			var gamePlayDirector = FindObjectOfType<GamePlayDirector>();
			var tile = GameObject.Find("Tile" + ((row - 1) * 3 + column));
			// check whether panel exists on the tile
			if (tile.transform.childCount != 0)
			{
				gameObject.GetComponent<SpriteRenderer>().color = Color.red;
				gamePlayDirector.Dispatch(GamePlayDirector.GameState.Failure);
			}
			else
			{
				// display the hole betweeen tile layer and panel layer
				gameObject.GetComponent<Renderer>().sortingLayerName = "Hole";
				yield return new WaitForSeconds(0.5f);
				Destroy(gameObject);
			}
		}
	}
}
