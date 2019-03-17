﻿using Project.Scripts.Utils.Definitions;
using UnityEngine;
using System;

namespace Project.Scripts.GamePlayScene.BulletWarning
{
	public class NormalCartridgeWarningController : BulletWarningController
	{
		protected override void Awake()
		{
			base.Awake();
			transform.localScale =
				new Vector2(CartridgeWarningSize.WIDTH / originalWidth, CartridgeWarningSize.HEIGHT / originalHeight);
		}

		// 警告のpositionを計算する
		// 銃弾の移動方向(bulletMotionVector)が副次的に計算されるので、その値を返す
		public Vector2 Initialize(CartridgeType cartridgeType, CartridgeDirection direction, int line)
		{
			switch (cartridgeType)
			{
				case CartridgeType.Normal:
					break;
				case CartridgeType.Turn:
					var sprite = Resources.Load<Sprite>("Textures/BulletWarning/TurnWarning");
					gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
					break;
				default:
					throw new NotImplementedException();
			}

			Vector2 bulletMotionVector;
			Vector2 warningPosition;
			switch (direction)
			{
				case CartridgeDirection.ToLeft:
					warningPosition = new Vector2(WindowSize.WIDTH / 2,
						WindowSize.HEIGHT * 0.5f - (TileSize.MARGIN_TOP + TileSize.HEIGHT * 0.5f) -
						TileSize.HEIGHT * (line - 1));
					bulletMotionVector = Vector2.left;
					break;
				case CartridgeDirection.ToRight:
					warningPosition = new Vector2(-WindowSize.WIDTH / 2,
						WindowSize.HEIGHT * 0.5f - (TileSize.MARGIN_TOP + TileSize.HEIGHT * 0.5f) -
						TileSize.HEIGHT * (line - 1));
					bulletMotionVector = Vector2.right;
					break;
				case CartridgeDirection.ToUp:
					warningPosition = new Vector2(TileSize.WIDTH * (line - 2),
						-WindowSize.HEIGHT / 2);
					bulletMotionVector = Vector2.up;
					break;
				case CartridgeDirection.ToBottom:
					warningPosition = new Vector2(TileSize.WIDTH * (line - 2),
						WindowSize.HEIGHT / 2);
					bulletMotionVector = Vector2.down;
					break;
				default:
					throw new NotImplementedException();
			}

			warningPosition = warningPosition + Vector2.Scale(bulletMotionVector,
				                  new Vector2(CartridgeWarningSize.POSITION_X, CartridgeWarningSize.POSITION_Y)) / 2;
			transform.position = warningPosition;
			return bulletMotionVector;
		}
	}
}
