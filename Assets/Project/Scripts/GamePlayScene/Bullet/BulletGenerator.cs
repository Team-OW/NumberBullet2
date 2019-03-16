﻿using System;
using System.Collections;
using System.Collections.Generic;
using Project.Scripts.GamePlayScene.BulletWarning;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bullet
{
	public class BulletGenerator : MonoBehaviour
	{
		private GamePlayDirector gamePlayDirector;

		// 銃弾および警告のprefab
		public GameObject normalCartridgePrefab;
		public GameObject normalCartridgeWarningPrefab;
		public GameObject normalHolePrefab;
		public GameObject normalHoleWarningPrefab;

		// Generatorが作成された時刻
		public float startTime;

		// 生成された銃弾のID(sortingOrder)
		private short bulletId = -32768;

		private List<IEnumerator> coroutines;

		private void OnEnable()
		{
			GamePlayDirector.OnSucceed += OnSucceed;
			GamePlayDirector.OnFail += OnFail;
		}

		private void OnDisable()
		{
			GamePlayDirector.OnSucceed -= OnSucceed;
			GamePlayDirector.OnFail -= OnFail;
		}

		public void CreateBullets(int stageId)
		{
			gamePlayDirector = FindObjectOfType<GamePlayDirector>();
			coroutines = new List<IEnumerator>();
			startTime = Time.time;
			switch (stageId)
			{
				case 1:
					// coroutineのリスト
					coroutines.Add(CreateCartridge(CartridgeType.NormalCartridge, CartridgeDirection.ToLeft,
						(int) ToLeft.First, 1.0f, 1.0f));
					coroutines.Add(CreateCartridge(CartridgeType.NormalCartridge, CartridgeDirection.ToRight,
						(int) ToRight.Second, 2.0f, 4.0f));
					break;
				case 2:
					coroutines.Add(CreateCartridge(CartridgeType.NormalCartridge, CartridgeDirection.ToRight,
						(int) ToRight.Fifth, 2.0f, 0.5f));
					coroutines.Add(CreateHole(HoleType.NormalHole, 1.0f, 2.0f,
						(int) Row.Second, (int) Column.Left));
					break;
				default:
					throw new NotImplementedException();
			}

			foreach (var coroutine in coroutines) StartCoroutine(coroutine);
		}

		// 指定した行(or列)の端から一定の時間間隔(interval)で弾丸を作成するメソッド
		private IEnumerator CreateCartridge(CartridgeType cartridgeType, CartridgeDirection direction, int line,
			float appearanceTime, float interval)
		{
			var currentTime = Time.time;

			// wait by the time the first bullet warning emerge
			// 1.0f equals to the period which the bullet warning is emerging
			yield return new WaitForSeconds(appearanceTime - BulletWarningController.WARNING_DISPLAYED_TIME -
			                                (currentTime - startTime));

			// the number of bullets which have emerged
			var sum = 0;

			while (true)
			{
				sum++;
				var tempBulletId = bulletId;

				// 作成するcartidgeの種類で分岐
				GameObject warning;
				switch (cartridgeType)
				{
					case CartridgeType.NormalCartridge:
						warning = Instantiate(normalCartridgeWarningPrefab);
						break;
					default:
						throw new NotImplementedException();
				}

				// 同レイヤーのオブジェクトの描画順序の制御
				warning.GetComponent<Renderer>().sortingOrder = tempBulletId;

				// warningの位置・大きさ等の設定
				var warningScript = warning.GetComponent<CartridgeWarningController>();
				var bulletMotionVector = warningScript.Initialize(direction, line);
				// warningの表示が終わる時刻を待ち、cartidgeを作成する
				StartCoroutine(CreateCartridge(warning, cartridgeType, direction, line, bulletMotionVector,
					tempBulletId));

				// 作成する銃弾の個数の上限チェック
				try
				{
					bulletId = checked((short) (bulletId + 1));
				}
				catch (OverflowException)
				{
					gamePlayDirector.Dispatch(GamePlayDirector.GameState.Failure);
				}

				// 次の銃弾を作成する時刻まで待つ
				currentTime = Time.time;
				yield return new WaitForSeconds(appearanceTime - BulletWarningController.WARNING_DISPLAYED_TIME +
				                                interval * sum - (currentTime - startTime));
			}
		}

		// warningの表示が終わる時刻を待ち、cartridgeを作成するメソッド
		private IEnumerator CreateCartridge(GameObject warning, CartridgeType cartridgeType,
			CartridgeDirection direction, int line, Vector2 motionVector, short cartridgeId)
		{
			// 警告の表示時間だけ待つ
			yield return new WaitForSeconds(BulletWarningController.WARNING_DISPLAYED_TIME);
			// 警告を削除する
			Destroy(warning);

			// ゲームが続いているなら銃弾を作成する
			if (gamePlayDirector.state == GamePlayDirector.GameState.Playing)
			{
				GameObject cartridge;
				switch (cartridgeType)
				{
					case CartridgeType.NormalCartridge:
						cartridge = Instantiate(normalCartridgePrefab);
						break;
					default:
						throw new NotImplementedException();
				}

				// 変数の初期設定
				var cartridgeScript = cartridge.GetComponent<CartridgeController>();
				cartridgeScript.Initialize(direction, line, motionVector);
				// 同レイヤーのオブジェクトの描画順序の制御
				cartridge.GetComponent<Renderer>().sortingOrder = cartridgeId;
			}
		}

		// 指定したパネルに一定の時間間隔(interval)で撃ち抜く銃弾を作成するメソッド
		private IEnumerator CreateHole(HoleType holeType, float appearanceTime, float interval, int row = 0,
			int column = 0)
		{
			var currentTime = Time.time;
			yield return new WaitForSeconds(appearanceTime - BulletWarningController.WARNING_DISPLAYED_TIME -
			                                (currentTime - startTime));

			var sum = 0;

			while (true)
			{
				sum++;
				var tempBulletId = bulletId;

				GameObject warning;
				switch (holeType)
				{
					case HoleType.NormalHole:
						warning = Instantiate(normalHoleWarningPrefab);
						break;
					default:
						throw new NotImplementedException();
				}

				warning.GetComponent<Renderer>().sortingOrder = tempBulletId;

				var warningScript = warning.GetComponent<HoleWarningController>();
				warningScript.Initialize(row, column);

				StartCoroutine(CreateHole(warning, holeType, row, column, warning.transform.position, tempBulletId));

				try
				{
					bulletId = checked((short) (bulletId + 1));
				}
				catch (OverflowException)
				{
					gamePlayDirector.Dispatch(GamePlayDirector.GameState.Failure);
				}

				// 一定時間(interval)待つ
				currentTime = Time.time;
				yield return new WaitForSeconds(appearanceTime - BulletWarningController.WARNING_DISPLAYED_TIME +
				                                interval * sum - (currentTime - startTime));
			}
		}

		// warningの表示が終わる時刻を待ち、holeを作成するメソッド
		private IEnumerator CreateHole(GameObject warning, HoleType holeType, int row, int column,
			Vector3 holeWarningPosition, short holeId)
		{
			yield return new WaitForSeconds(BulletWarningController.WARNING_DISPLAYED_TIME);
			Destroy(warning);

			if (gamePlayDirector.state == GamePlayDirector.GameState.Playing)
			{
				GameObject hole;
				switch (holeType)
				{
					case HoleType.NormalHole:
						hole = Instantiate(normalHolePrefab);
						break;
					default:
						throw new NotImplementedException();
				}

				var holeScript = hole.GetComponent<HoleController>();
				holeScript.Initialize(row, column, holeWarningPosition);

				hole.GetComponent<Renderer>().sortingOrder = holeId;
				StartCoroutine(holeScript.Delete());
			}
		}

		private void OnSucceed()
		{
			GameFinish();
		}

		private void OnFail()
		{
			GameFinish();
		}

		private void GameFinish()
		{
			foreach (var coroutine in coroutines) StopCoroutine(coroutine);
		}
	}
}
