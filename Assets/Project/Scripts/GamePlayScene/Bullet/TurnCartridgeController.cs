﻿using System;
using System.Collections;
using Project.Scripts.GamePlayScene.BulletWarning;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Library.Extension;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bullet
{
    public class TurnCartridgeController : NormalCartridgeController
    {
        /// <summary>
        /// 曲がる方向の配列
        /// </summary>
        private int[] _turnDirection;

        /// <summary>
        /// 曲がる行(または列)の配列
        /// </summary>
        private int[] _turnLine;

        /// <summary>
        /// NormalWarningのPrefab
        /// </summary>
        [SerializeField] private GameObject _normalCartridgeWarningPrefab;

        private GamePlayDirector _gamePlayDirector;

        /// <summary>
        /// 警告を表示するフレームの配列
        /// </summary>
        private int[] _warningTiming;

        /// <summary>
        /// 次に表示する警告のインデックス
        /// </summary>
        private int _warningIndex = 0;

        /// <summary>
        /// 曲がる方向を示す警告の座標の配列
        /// </summary>
        private Vector2[] _turnPoint;

        /// <summary>
        /// 1フレームあたりの回転角度
        /// </summary>
        private float[] _turnAngle;

        /// <summary>
        /// 回転に要するフレーム数
        /// </summary>
        private const int _COUNT = 50;

        /// <summary>
        /// Unity GUIで設定したfps
        /// </summary>
        private const float _FRAME_RATE = 50;

        /// <summary>
        /// 回転中の銃弾の速さ (円周(= 回転半径 * 2 * pi)の4分の1をフレーム数で割る)
        /// </summary>
        private float _rotatingSpeed =
            ((PanelSize.WIDTH - CartridgeSize.WIDTH) / 2f) * 2f * (float) Math.PI / 4f / _COUNT;

        /// <summary>
        /// 銃弾が回転しているかどうかを表す状態
        /// </summary>
        private bool _rotating = false;

        /// <summary>
        /// 警告を表示し、銃弾を回転させるcoroutineの配列
        /// </summary>
        private IEnumerator[] rotateCoroutines;

        /// <summary>
        /// 曲がる方向に応じて表示分けする警告画像の名前
        /// </summary>
        /// <value></value>
        private enum _ETurnWarning {
            turnLeft,
            turnRight,
            turnUp,
            turnBottom
        }

        protected override void Awake()
        {
            base.Awake();
            _gamePlayDirector = FindObjectOfType<GamePlayDirector>();
        }

        protected override void FixedUpdate()
        {
            if (_gamePlayDirector.state == GamePlayDirector.EGameState.Playing) {
                // 警告を表示するタイミングかどうかを毎フレーム監視する
                _warningTiming[_warningIndex]--;
                if (_warningTiming[_warningIndex] == 0) {
                    // 警告を表示、その後、銃弾の回転
                    StartCoroutine(rotateCoroutines[_warningIndex]);
                    // 次の警告の表示タイミングを監視する
                    if (_warningIndex != _turnDirection.Length - 1)
                        _warningIndex++;
                }

                if (_rotating) {
                    // 回転中
                    transform.Translate(motionVector * _rotatingSpeed, Space.World);
                } else {
                    // 直進中
                    transform.Translate(motionVector * speed, Space.World);
                }
            }
        }

        /// <summary>
        /// 銃弾の移動ベクトル、曲がる方向、曲がる行(または列)、初期座標を設定する
        /// 警告を表示するフレームを計算する
        /// </summary>
        /// <param name="direction"> 移動方向 </param>
        /// <param name="line"> 移動する行(または列) </param>
        /// <param name="motionVector"> 移動ベクトル </param>
        /// <param name="turnDirection">　曲がる方向 </param>
        /// <param name="turnLine"> 曲がる行(または列) </param>
        public void Initialize(ECartridgeDirection direction, int line, Vector2 motionVector,
            int[] turnDirection, int[] turnLine)
        {
            // 銃弾に必要な引数を受け取る
            Initialize(direction, line, motionVector);
            this._turnDirection = turnDirection;
            this._turnLine = turnLine;
            // 表示する全ての警告の座標および回転の角度を計算
            Vector2 cartridgePosition = transform.position;
            Vector2 cartridgeMotionVector = motionVector;
            _turnAngle = new float[turnDirection.Length];
            _turnPoint = new Vector2[turnDirection.Length];
            for (var index = 0; index < turnDirection.Length; index++) {
                // 警告の座標
                _turnPoint[index] = cartridgePosition * cartridgeMotionVector.Transposition().Abs() + new Vector2(
                        TileSize.WIDTH * (turnLine[index] - 2),
                        WindowSize.HEIGHT * 0.5f - (TileSize.MARGIN_TOP + TileSize.HEIGHT * 0.5f) -
                        TileSize.HEIGHT * (turnLine[index] - 1)) * cartridgeMotionVector.Abs();
                // 1フレームあたりの回転の角度
                _turnAngle[index] = _turnDirection[index] % 2 == 1 ? 90 : -90;
                _turnAngle[index] = (cartridgeMotionVector.x + cartridgeMotionVector.y) * _turnAngle[index];
                _turnAngle[index] = _turnAngle[index] / (_COUNT - 1) / 180.0f * Mathf.PI;
                // 求めた警告の座標を銃弾の座標として次の警告の座標を求める
                cartridgePosition = _turnPoint[index];
                cartridgeMotionVector = cartridgeMotionVector.Rotate(_turnAngle[index] * (_COUNT - 1));
            }

            // 1つ目の警告を表示させるタイミングを求める
            // 警告座標に到達する時間(= 銃弾が進む距離 / 銃弾の速さ)のNフレーム前が表示タイミング
            _warningTiming = new int[turnDirection.Length + 1];
            _warningTiming[0] = (int)Math.Round((Vector2.Distance(transform.position, _turnPoint[0]) - (PanelSize.WIDTH - CartridgeSize.WIDTH) / 2f) / speed - _FRAME_RATE * BulletWarningController.WARNING_DISPLAYED_TIME, MidpointRounding.AwayFromZero);
            // 警告の表示および銃弾が回転する挙動を特定のタイミングで発火できるようにcoroutineにセットする
            rotateCoroutines = new IEnumerator[turnDirection.Length];
            rotateCoroutines[0] = DisplayTurnWarning(_turnPoint[0], _turnDirection[0], _turnAngle[0]);
            for (var index = 1; index < turnDirection.Length; index++) {
                // 1つ前の警告の表示タイミングから何フレーム後に表示させるかを求める
                // 警告座標に到達する時間(= 1つ前の表示タイミングのNフレーム後 + 銃弾が回転にかかるフレーム数 + (警告と警告との距離) / 銃弾の速さ)のNフレーム前
                _warningTiming[index] = (int)Math.Round(_COUNT + (Vector2.Distance(_turnPoint[index - 1], _turnPoint[index]) - (PanelSize.WIDTH - CartridgeSize.WIDTH)) / speed, MidpointRounding.AwayFromZero);
                rotateCoroutines[index] = DisplayTurnWarning(_turnPoint[index], _turnDirection[index], _turnAngle[index]);
            }
        }

        /// <summary>
        /// 銃弾を回転させるcoroutine
        /// </summary>
        /// <param name="turnAngle"> 1フレームあたりの回転角 </param>
        /// <returns></returns>
        private IEnumerator RotateCartridge(float turnAngle)
        {
            _rotating = true;
            // 回転はじめのフレーム
            motionVector = motionVector.Rotate(turnAngle / 2f);
            transform.Rotate(new Vector3(0, 0, turnAngle / 2f / Mathf.PI * 180f), Space.World);
            yield return new WaitForFixedUpdate();
            // 回転中のフレーム
            for (var index = 0; index < _COUNT - 2; index++) {
                motionVector = motionVector.Rotate(turnAngle);
                transform.Rotate(new Vector3(0, 0, turnAngle / Mathf.PI * 180f), Space.World);
                yield return new WaitForFixedUpdate();
            }
            // 回転終わりのフレーム
            motionVector = motionVector.Rotate(turnAngle / 2f);
            transform.Rotate(new Vector3(0, 0, turnAngle / 2f / Mathf.PI * 180f), Space.World);
            yield return new WaitForFixedUpdate();
            _rotating = false;
            yield break;
        }

        /// <summary>
        /// 警告を表示するcoroutine
        /// </summary>
        /// <param name="warningPosition"> 警告の座標 </param>
        /// <param name="turnDirection"> 回転の方向 </param>
        /// <param name="turnAngle"> 1フレームあたりの回転角 </param>
        /// <returns></returns>
        private IEnumerator DisplayTurnWarning(Vector2 warningPosition, int turnDirection, float turnAngle)
        {
            var _warning = Instantiate(_normalCartridgeWarningPrefab);
            // 同レイヤーのオブジェクトの描画順序の制御
            _warning.GetComponent<Renderer>().sortingOrder = gameObject.GetComponent<Renderer>().sortingOrder;
            // warningの位置・大きさ等の設定
            var warningScript = _warning.GetComponent<CartridgeWarningController>();
            warningScript.Initialize(warningPosition, Enum.GetName(typeof(_ETurnWarning), turnDirection - 1));
            // 警告の表示時間だけ待つ
            for (var index = 0; index < _FRAME_RATE; index++) yield return new WaitForFixedUpdate();
            // 警告を削除する
            Destroy(_warning);
            // 銃弾を回転させる
            StartCoroutine(RotateCartridge(turnAngle));
            yield break;
        }

        protected override void OnFail()
        {
            base.OnFail();
            StopAllCoroutines();
            motionVector = new Vector2(0, 0);
        }
    }
}
