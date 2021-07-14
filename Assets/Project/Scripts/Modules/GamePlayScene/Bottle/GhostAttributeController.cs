﻿using System;
using System.Linq;
using Treevel.Common.Entities;
using Treevel.Common.Utils;
using UniRx;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Bottle
{
    public class GhostAttributeController : BottleAttributeControllerBase
    {
        private DynamicBottleController _bottleController;

        private Animator _bottleAnimator;
        private static readonly int _ANIMATOR_PARAM_TRIGGER_BE_GHOST_IDLE = Animator.StringToHash("BeGhostIdle");
        private static readonly int _ANIMATOR_PARAM_TRIGGER_BE_GHOST_ACTION = Animator.StringToHash("BeGhostAction");
        private static readonly int _ANIMATOR_PARAM_FLOAT_SPEED = Animator.StringToHash("GhostSpeed");

        protected override void Awake()
        {
            base.Awake();
            GamePlayDirector.Instance.StagePrepared.Subscribe(_ => spriteRenderer.enabled = true).AddTo(compositeDisposableOnGameEnd, this);
            GamePlayDirector.Instance.GameStart.Subscribe(_ => {
                animator.enabled = true;
                animator.SetTrigger(_ANIMATOR_PARAM_TRIGGER_BE_GHOST_ACTION);
                _bottleAnimator.SetTrigger(_ANIMATOR_PARAM_TRIGGER_BE_GHOST_ACTION);
            }).AddTo(compositeDisposableOnGameEnd, this);
            GamePlayDirector.Instance.GameEnd.Subscribe(_ => {
                animator.enabled = false;
                _bottleAnimator.SetFloat(_ANIMATOR_PARAM_FLOAT_SPEED, 0f);
            }).AddTo(this);
            // 描画順序の設定
            spriteRenderer.sortingOrder = EBottleAttributeType.Ghost.GetOrderInLayer();
        }

        public void Initialize(DynamicBottleController bottleController)
        {
            transform.parent = bottleController.transform;
            transform.localPosition = Vector3.zero;
            _bottleController = bottleController;
            _bottleAnimator = bottleController.GetComponent<Animator>();

            // イベントに処理を登録する
            _bottleController.StartMove.Subscribe(_ => QuitAnimation()).AddTo(this);
            _bottleController.EndMove.Subscribe(_ => RestartAnimation()).AddTo(this);
            _bottleController.pressGesture.OnPress.AsObservable().Subscribe(_ => PauseAnimation()).AddTo(compositeDisposableOnGameEnd, this);
            _bottleController.releaseGesture.OnRelease.AsObservable().Subscribe(_ => ResumeAnimation()).AddTo(compositeDisposableOnGameEnd, this);
            GamePlayDirector.Instance.GameEnd.Subscribe(_ => {
                _bottleAnimator.SetFloat(_ANIMATOR_PARAM_FLOAT_SPEED, 0f);
            }).AddTo(this);
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
                RestartAnimation();
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

        /// <summary>
        /// アニメーションを一時停止する
        /// </summary>
        private void PauseAnimation()
        {
            animator.SetFloat(_ANIMATOR_PARAM_FLOAT_SPEED, 0f);
            _bottleAnimator.SetFloat(_ANIMATOR_PARAM_FLOAT_SPEED, 0f);
        }

        /// <summary>
        /// アニメーションを途中から再開する
        /// </summary>
        private void ResumeAnimation()
        {
            animator.SetFloat(_ANIMATOR_PARAM_FLOAT_SPEED, 1f);
            _bottleAnimator.SetFloat(_ANIMATOR_PARAM_FLOAT_SPEED, 1f);
        }

        /// <summary>
        /// Ghostアニメーションを終了する
        /// </summary>
        private void QuitAnimation()
        {
            animator.SetTrigger(_ANIMATOR_PARAM_TRIGGER_BE_GHOST_IDLE);
            _bottleAnimator.SetTrigger(_ANIMATOR_PARAM_TRIGGER_BE_GHOST_IDLE);
        }

        /// <summary>
        /// Ghostアニメーションを最初から開始する
        /// </summary>
        private void RestartAnimation()
        {
            animator.SetTrigger(_ANIMATOR_PARAM_TRIGGER_BE_GHOST_ACTION);
            animator.SetFloat(_ANIMATOR_PARAM_FLOAT_SPEED, 1f);
            _bottleAnimator.SetTrigger(_ANIMATOR_PARAM_TRIGGER_BE_GHOST_ACTION);
            _bottleAnimator.SetFloat(_ANIMATOR_PARAM_FLOAT_SPEED, 1f);
        }
    }
}
