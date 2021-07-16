using System;
using System.Linq;
using Treevel.Common.Entities;
using Treevel.Common.Utils;
using UniRx;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Bottle
{
    public class GhostAttributeController : BottleAttributeControllerBase
    {

        [SerializeField] private SpriteRenderer _ghostRenderer;
        [SerializeField] private SpriteRenderer _backgroundRenderer;
        [SerializeField] private ParticleSystem _ghostParticle;
        [SerializeField] private ParticleSystemRenderer _ghostParticleRenderer;

        private DynamicBottleController _bottleController;

        private static readonly int _ANIMATOR_PARAM_FLOAT_SPEED = Animator.StringToHash("GhostSpeed");

        protected override void Awake()
        {
            base.Awake();
            GamePlayDirector.Instance.StagePrepared
                .Subscribe(_ => _ghostRenderer.enabled = true)
                .AddTo(compositeDisposableOnGameEnd, this);
            GamePlayDirector.Instance.GameStart.Subscribe(_ => {
                animator.enabled = true;
            }).AddTo(compositeDisposableOnGameEnd, this);
            GamePlayDirector.Instance.GameEnd.Subscribe(_ => {
                animator.enabled = false;
                _ghostParticle.Pause();
            }).AddTo(this);
            // 描画順序の設定
            var sortingOrder = EBottleAttributeType.Ghost.GetOrderInLayer();
            _backgroundRenderer.sortingOrder = sortingOrder;
            _ghostParticleRenderer.sortingOrder = sortingOrder + 1;
            _ghostRenderer.sortingOrder = sortingOrder + 2;
        }

        public void Initialize(DynamicBottleController bottleController)
        {
            transform.parent = bottleController.transform;
            transform.localPosition = Vector3.zero;
            _bottleController = bottleController;

            // イベントに処理を登録する
            _bottleController.EndMove.Subscribe(_ => animator.SetFloat(_ANIMATOR_PARAM_FLOAT_SPEED, 1f))
                .AddTo(compositeDisposableOnGameEnd, this);
            _bottleController.pressGesture.OnPress.AsObservable()
                .Subscribe(_ => animator.SetFloat(_ANIMATOR_PARAM_FLOAT_SPEED, 0f))
                .AddTo(compositeDisposableOnGameEnd, this);
            _bottleController.releaseGesture.OnRelease.AsObservable()
                .Subscribe(_ => animator.SetFloat(_ANIMATOR_PARAM_FLOAT_SPEED, 1f))
                .AddTo(compositeDisposableOnGameEnd, this);
        }

        /// <summary>
        /// Particleを発生させる(Animationから呼び出し)
        /// </summary>
        public void PlayParticle()
        {
            _ghostParticle.Play();
        }

        /// <summary>
        /// 空いている方向にBottleを移動させる(Animationから呼び出し)
        /// </summary>
        public void MoveToFreeDirection()
        {
            // ボトルの位置を取得する
            var tileNum = BoardManager.Instance.GetBottlePos(_bottleController);
            var (x, y) = BoardManager.Instance.TileNumToXY(tileNum).Value;

            var canMoveDirections = new int[Enum.GetNames(typeof(EDirection)).Length];
            // 空いている方向を確認する
            // 左
            if (BoardManager.Instance.IsEmptyTile(x - 1, y)) canMoveDirections[(int)EDirection.ToLeft] = 1;
            // 右
            if (BoardManager.Instance.IsEmptyTile(x + 1, y)) canMoveDirections[(int)EDirection.ToRight] = 1;
            // 上
            if (BoardManager.Instance.IsEmptyTile(x, y - 1)) canMoveDirections[(int)EDirection.ToUp] = 1;
            // 下
            if (BoardManager.Instance.IsEmptyTile(x, y + 1)) canMoveDirections[(int)EDirection.ToDown] = 1;

            // 空いている方向からランダムに1方向を選択する
            if (canMoveDirections.Sum() == 0) {
                return;
            }
            var direction =
                (EDirection)Enum.ToObject(typeof(EDirection), GimmickLibrary.SamplingArrayIndex(canMoveDirections));
            switch (direction) {
                case EDirection.ToLeft:
                    BoardManager.Instance.MoveAsync(_bottleController, tileNum - 1, Vector2Int.left);
                    break;
                case EDirection.ToRight:
                    BoardManager.Instance.MoveAsync(_bottleController, tileNum + 1, Vector2Int.right);
                    break;
                case EDirection.ToUp:
                    BoardManager.Instance.MoveAsync(_bottleController, tileNum - Constants.StageSize.COLUMN, Vector2Int.up);
                    break;
                case EDirection.ToDown:
                    BoardManager.Instance.MoveAsync(_bottleController, tileNum + Constants.StageSize.COLUMN,
                                               Vector2Int.down);
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }
    }
}
