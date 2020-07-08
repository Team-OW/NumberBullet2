﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.GameDatas;
using Project.Scripts.GamePlayScene.Bottle;
using Project.Scripts.Utils;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Library;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Project.Scripts.GamePlayScene.Gimmick
{
    public class TornadoController : AbstractGimmickController
    {
        /// <summary>
        /// 竜巻の移動速度
        /// </summary>
        [SerializeField] private float _speed = 3.0f;

        /// <summary>
        /// 警告のプレハブ
        /// </summary>
        [SerializeField] protected AssetReferenceGameObject _warningPrefab;

        /// <summary>
        /// 銃弾の移動方向
        /// </summary>
        private ECartridgeDirection[] _targetDirections;

        /// <summary>
        /// 攻撃する行／列
        /// </summary>
        private int[] _targetLines;

        /// <summary>
        /// 曲がる方向の重み
        /// </summary>
        private int[] _randomDirections = BulletLibrary.GetInitialArray(Enum.GetNames(typeof(ECartridgeDirection)).Length - 1);

        /// <summary>
        /// 曲がる行の重み
        /// </summary>
        private int[] _randomRow = BulletLibrary.GetInitialArray(Enum.GetNames(typeof(ERow)).Length - 1);

        /// <summary>
        /// 曲がる列の重み
        /// </summary>
        private int[] _randomColumn = BulletLibrary.GetInitialArray(Enum.GetNames(typeof(EColumn)).Length - 1);

        /// <summary>
        /// 警告表示座標のリスト
        /// </summary>
        private List<Vector2> _wanringPosList = new List<Vector2>();

        /// <summary>
        /// 今の目標インデックス
        /// </summary>
        private int _currentTargetIndex = 0;

        private Rigidbody2D _rigidBody;

        /// <summary>
        /// 警告オブジェクトインスタンス（ゲーム終了時片付けするためにメンバー変数として持つ）
        /// </summary>
        private GameObject _warningObj;

        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
        }

        /// <summary>
        /// ギミックの初期化
        /// </summary>
        /// <param name="gimmickData"> ギミックデータ </param>
        public override void Initialize(GimmickData gimmickData)
        {
            base.Initialize(gimmickData);
            _targetDirections = gimmickData.targetDirections.ToArray();
            _targetLines = gimmickData.targetLines.ToArray();
            if (gimmickData.randomDirection.Count > 0) _randomDirections = gimmickData.randomDirection.ToArray();
            if (gimmickData.randomColumn.Count > 0) _randomColumn = gimmickData.randomColumn.ToArray();
            if (gimmickData.randomRow.Count > 0) _randomRow = gimmickData.randomRow.ToArray();

            if (_targetDirections.Length <= 0 || _targetLines.Length <= 0 || _targetDirections.Length != _targetLines.Length) {
                throw new InvalidOperationException($"size of targetDirections = {_targetDirections.Length}, size of targetLines = {_targetLines.Length}");
            }

            InitializeDirectionsAndLines();

            SetInitialPosition(_targetDirections[0], _targetLines[0]);
            // 初期位置についたら表示する
            GetComponent<SpriteRenderer>().enabled = true;
            // GetComponent<Collider2D>().enabled = true;
        }

        /// <inheritdoc></inheritdoc>
        public override IEnumerator Trigger()
        {
            var currentDirection = _targetDirections[_currentTargetIndex];
            yield return ShowWarning(_wanringPosList[0], null, _warningDisplayTime);

            // 発射
            SetDirection(currentDirection);

            while (++_currentTargetIndex < _targetDirections.Length) {
                currentDirection = _targetDirections[_currentTargetIndex];

                // 警告位置
                var warningPos = _wanringPosList[_currentTargetIndex];

                // 警告表示時ギミックがいる位置＝警告表示位置＋（警告消した後からタイルいちに着くまでの時間＋警告表示時間）x速度x(-移動方向のベクトル)
                var warningStartDisplayPos = warningPos - _rigidBody.velocity * _warningDisplayTime * 1.5f;

                var diffVec = warningStartDisplayPos - (Vector2)transform.position;
                // 警告表示位置までまだ時間ある
                if (Vector2.Dot(diffVec, _rigidBody.velocity) > 0) {
                    // 表示するまでの所要時間
                    var warningStartWaitTime = diffVec.magnitude / _speed;
                    // 警告表示するタイミングまで待つ
                    yield return new WaitForSeconds(warningStartWaitTime);
                }

                // 警告を表示する
                StartCoroutine(ShowWarning(warningPos, currentDirection, _warningDisplayTime));

                // 目標位置についたら転向処理（竜巻だからそのままdirection変えればいいのか？）
                while (Vector2.Dot(_rigidBody.velocity, warningPos - (Vector2)transform.position) > 0) {
                    yield return new WaitForFixedUpdate();
                }

                SetDirection(currentDirection);
            }
        }

        /// <summary>
        /// 方向と行列に対して、ランダムなものがあれば有効な数値で入れ替える
        /// </summary>
        private void InitializeDirectionsAndLines()
        {
            var targetNum = _targetDirections.Length;
            for (var i = 0 ; i < targetNum ; i++) {
                var direction = _targetDirections[i];
                var line = _targetLines[i];
                if (direction == ECartridgeDirection.Random) {
                    if (i == 0) { // 最初の方向は制限ないのでそのまま乱数生成
                        direction = (ECartridgeDirection)Enum.ToObject(typeof(ECartridgeDirection), BulletLibrary.SamplingArrayIndex(_randomDirections) + 1);
                    } else { // それ以降は前回の結果に依存する
                        var previousLine = _targetLines[i - 1];
                        var previousDirection = _targetDirections[i - 1];

                        if (IsHorizontal(previousDirection)) { // 左右を移動している場合
                            if (previousLine == (int)ERow.Fifth) { // 最下行
                                direction = ECartridgeDirection.ToUp;
                            } else if (previousLine == (int)ERow.First) { // 最上行
                                direction = ECartridgeDirection.ToBottom;
                            } else {
                                var tempRandomDirections = _randomDirections.ToArray(); // 左右を除いた乱数配列
                                tempRandomDirections[(int)ECartridgeDirection.ToLeft - 1] = tempRandomDirections[(int)ECartridgeDirection.ToRight - 1] = 0;
                                direction = (ECartridgeDirection)Enum.ToObject(typeof(ECartridgeDirection), BulletLibrary.SamplingArrayIndex(tempRandomDirections) + 1);
                            }
                        } else if (IsVertical(previousDirection)) { // 上下を移動している場合
                            if (previousLine == (int)EColumn.Left) { // 最左列
                                direction = ECartridgeDirection.ToRight;
                            } else if (previousLine == (int)EColumn.Right) { // 最右列
                                direction = ECartridgeDirection.ToLeft;
                            } else {
                                var tempRandomDirections = _randomDirections.ToArray(); // 上下を除いた乱数配列
                                tempRandomDirections[(int)ECartridgeDirection.ToUp - 1] = tempRandomDirections[(int)ECartridgeDirection.ToBottom - 1] = 0;
                                direction = (ECartridgeDirection)Enum.ToObject(typeof(ECartridgeDirection), BulletLibrary.SamplingArrayIndex(tempRandomDirections) + 1);
                            }
                        }
                    }
                }

                if (line == -1) {
                    if (IsHorizontal(direction)) {
                        line = BulletLibrary.SamplingArrayIndex(_randomColumn) + 1;
                    } else if (IsVertical(direction)) {
                        line = BulletLibrary.SamplingArrayIndex(_randomRow) + 1;
                    }
                }

                _targetDirections[i] = direction;
                _targetLines[i] = line;

                // 警告の位置も計算しておく
                if (i == 0) {
                    _wanringPosList.Add(CalculateFirstWarningPos(direction, line));
                } else {
                    _wanringPosList.Add(CalculateOtherWarningPos(_targetDirections[i - 1], _targetLines[i - 1], _targetLines[i]));
                }
            }
        }

        /// <summary>
        /// 警告表示
        /// </summary>
        /// <param name="warningPos">表示する座標</param>
        /// <param name="direction">竜巻の次の移動方向</param>
        /// <param name="displayTime">表示時間</param>
        /// <returns></returns>
        protected IEnumerator ShowWarning(Vector2 warningPos, ECartridgeDirection? direction, float displayTime)
        {
            // 一個前の警告まで消えていない
            if (_warningObj != null) {
                _warningPrefab.ReleaseInstance(_warningObj);
            }

            string addressKey;
            switch (direction) {
                case ECartridgeDirection.ToLeft:
                    addressKey = Address.TURN_WARNING_LEFT_SPRITE;
                    break;
                case ECartridgeDirection.ToRight:
                    addressKey = Address.TURN_WARNING_RIGHT_SPRITE;
                    break;
                case ECartridgeDirection.ToUp:
                    addressKey = Address.TURN_WARNING_UP_SPRITE;
                    break;
                case ECartridgeDirection.ToBottom:
                    addressKey = Address.TURN_WARNING_BOTTOM_SPRITE;
                    break;
                default:
                    addressKey = Address.TURN_CARTRIDGE_WARNING_SPRITE;
                    break;
            }

            var sprite = AddressableAssetManager.GetAsset<Sprite>(addressKey);

            AsyncOperationHandle<GameObject> warningOp;
            yield return warningOp = _warningPrefab.InstantiateAsync(warningPos, Quaternion.identity);

            _warningObj = warningOp.Result;
            _warningObj.GetComponent<SpriteRenderer>().sprite = sprite;

            // 画像の切り替えでチラつくので切り替えの後に表示する
            _warningObj.GetComponent<SpriteRenderer>().enabled = true;

            // 警告終わるまで待つ
            yield return new WaitForSeconds(displayTime);

            _warningPrefab.ReleaseInstance(_warningObj);
        }

        private Vector2 CalculateOtherWarningPos(ECartridgeDirection currentDirection, int currentLine, int nextLine)
        {
            int col, row;
            if (IsHorizontal(currentDirection)) {
                row = currentLine;
                col = nextLine;
            } else if (IsVertical(currentDirection)) {
                col = currentLine;
                row = nextLine;
            } else {
                row = col = 1;
                throw new InvalidOperationException();
            }

            return BoardManager.Instance.GetTilePos(col - 1, row - 1);
        }

        private Vector2 CalculateFirstWarningPos(ECartridgeDirection direction, int line)
        {
            Vector2 bulletMotionVector;
            Vector2 warningPosition;
            if (IsHorizontal(direction)) {
                warningPosition = new Vector2(WindowSize.WIDTH / 2,
                    TileSize.HEIGHT * (StageSize.ROW / 2 + 1 - line));
                bulletMotionVector = direction == ECartridgeDirection.ToLeft ?
                    Vector2.left :
                    Vector2.right;
            } else if (IsVertical(direction)) {
                warningPosition = new Vector2(TileSize.WIDTH * (line - (StageSize.COLUMN / 2 + 1)),
                    -WindowSize.HEIGHT / 2);
                bulletMotionVector = direction == ECartridgeDirection.ToUp ?
                    Vector2.up :
                    Vector2.down;
            } else {
                throw new NotImplementedException();
            }
            warningPosition += Vector2.Scale(bulletMotionVector, new Vector2(CartridgeWarningSize.POSITION_X, CartridgeWarningSize.POSITION_Y)) / 2;
            return warningPosition;
        }

        /// <summary>
        /// 移動方向の設定
        /// </summary>
        /// <param name="direction">移動方向</param>
        private void SetDirection(ECartridgeDirection direction)
        {
            switch (direction) {
                case ECartridgeDirection.ToUp:
                    _rigidBody.velocity = Vector2.up * _speed;
                    break;
                case ECartridgeDirection.ToLeft:
                    _rigidBody.velocity = Vector2.left * _speed;
                    break;
                case ECartridgeDirection.ToRight:
                    _rigidBody.velocity = Vector2.right * _speed;
                    break;
                case ECartridgeDirection.ToBottom:
                    _rigidBody.velocity = Vector2.down * _speed;
                    break;
                default:
                    throw new System.NotImplementedException();
            }
        }


        /// <summary>
        /// 初期位置設定
        /// </summary>
        /// <param name="direction">初期の移動方向</param>
        /// <param name="line">攻撃する行（1~5）／列（1~3）</param>
        private void SetInitialPosition(ECartridgeDirection direction, int line)
        {
            float x = 0, y = 0;
            if (IsHorizontal(direction)) {
                // 目標列の一番右端のタイルのY座標を取得
                var tileNum = line * StageSize.COLUMN;
                y = BoardManager.Instance.GetTilePos(tileNum).y;

                if (direction == ECartridgeDirection.ToLeft) {
                    x = (WindowSize.WIDTH + CartridgeSize.WIDTH) / 2;
                } else {
                    x = -(WindowSize.WIDTH + CartridgeSize.WIDTH) / 2;
                }
            } else if (IsVertical(direction)) {
                // 目標行の一列目のタイルのx座標を取得
                var tileNum = line;
                x = BoardManager.Instance.GetTilePos(tileNum).x;

                if (direction == ECartridgeDirection.ToUp) {
                    y = -(WindowSize.HEIGHT + CartridgeSize.HEIGHT) / 2;
                } else {
                    y = (WindowSize.HEIGHT + CartridgeSize.HEIGHT) / 2;
                }
            } else {
                x = y = 0;
                throw new NotImplementedException();
            }
            transform.position = new Vector2(x, y);
        }

        protected void OnTriggerEnter2D(Collider2D other)
        {
            // 数字ボトルとの衝突以外は考えない
            var bottle = other.GetComponent<AbstractBottleController>();
            if (bottle == null || !bottle.IsAttackable || bottle.Invincible) return;

            // 衝突したオブジェクトは赤色に変える
            gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        }

        protected override void GameEnd()
        {
            _rigidBody.velocity = Vector2.zero;
            _warningPrefab.ReleaseInstance(_warningObj);
        }

        private bool IsHorizontal(ECartridgeDirection direction)
        {
            return direction == ECartridgeDirection.ToRight || direction == ECartridgeDirection.ToLeft;
        }

        private bool IsVertical(ECartridgeDirection direction)
        {
            return direction == ECartridgeDirection.ToUp || direction == ECartridgeDirection.ToBottom;
        }
    }
}