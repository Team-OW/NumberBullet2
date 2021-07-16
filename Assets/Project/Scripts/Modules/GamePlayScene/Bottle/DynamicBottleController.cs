﻿using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TouchScript.Gestures;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Extensions;
using Treevel.Common.Managers;
using Treevel.Common.Utils;
using UniRx;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Bottle
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(FlickGesture))]
    [RequireComponent(typeof(PressGesture))]
    [RequireComponent(typeof(ReleaseGesture))]
    public class DynamicBottleController : BottleControllerBase
    {
        private FlickGesture _flickGesture;
        public PressGesture pressGesture;
        public ReleaseGesture releaseGesture;

        /// <summary>
        /// 動くことができる状態か
        /// </summary>
        public bool IsMovable = true;

        /// <summary>
        /// フリックが反転するかどうか
        /// </summary>
        private bool _isReverse = false;

        /// <summary>
        /// 移動開始時の処理
        /// </summary>
        public IObservable<Unit> StartMove => _startMoveSubject;

        private readonly Subject<Unit> _startMoveSubject = new Subject<Unit>();

        /// <summary>
        /// 移動終了時の処理
        /// </summary>
        public IObservable<Unit> EndMove => _endMoveSubject;

        private readonly Subject<Unit> _endMoveSubject = new Subject<Unit>();

        /// <summary>
        /// フリック 時のパネルの移動速度
        /// </summary>
        private const float _SPEED = 30f;

        public int flickNum;

        protected override void Awake()
        {
            base.Awake();
            #if UNITY_EDITOR
            name = Constants.BottleName.DYNAMIC_DUMMY_BOTTLE;
            #endif
            // FlickGesture の設定
            _flickGesture = GetComponent<FlickGesture>();
            _flickGesture.MinDistance = 0.2f;
            _flickGesture.FlickTime = 0.2f;
            Observable.FromEvent<EventHandler<EventArgs>, Tuple<object, EventArgs>>(h => (x, y) => h(Tuple.Create(x, y)), x => _flickGesture.Flicked += x, x => _flickGesture.Flicked -= x)
                .Select(x => x.Item1 as FlickGesture)
                .Where(_ => GamePlayDirector.Instance.State == GamePlayDirector.EGameState.Playing)
                .Where(_ => IsMovable)
                .Where(gesture => gesture != null && gesture.State == FlickGesture.GestureState.Recognized)
                .Subscribe(async gesture => {
                    // 移動方向を単一方向の単位ベクトルに変換する ex) (0, 1)
                    var directionInt = Vector2Int.RoundToInt(gesture.ScreenFlickVector.NormalizeDirection());
                    if (_isReverse) directionInt *= -1;

                    // ボトルのフリック情報を伝える
                    await BoardManager.Instance.FlickBottleAsync(this, directionInt);
                }).AddTo(compositeDisposableOnGameEnd, this);

            // PressGesture の設定
            pressGesture = GetComponent<PressGesture>();
            pressGesture.UseUnityEvents = true;
            // ReleaseGesture の設定
            releaseGesture = GetComponent<ReleaseGesture>();
            releaseGesture.UseUnityEvents = true;
        }

        public override async UniTask InitializeAsync(BottleData bottleData)
        {
            await base.InitializeAsync(bottleData);

            // set handlers
            if (bottleData.isGhost) {
                var ghostAttribute =
                    await AddressableAssetManager.Instantiate(Constants.Address.GHOST_ATTRIBUTE_PREFAB).ToUniTask();

                ghostAttribute.GetComponent<GhostAttributeController>().Initialize(this);
            }

            if (bottleData.isReverse) {
                var reverseAttribute =
                    await AddressableAssetManager.Instantiate(Constants.Address.REVERSE_ATTRIBUTE_PREFAB).ToUniTask();
                reverseAttribute.GetComponent<ReverseAttributeController>().Initialize(this);
                _isReverse = true;
            }
        }

        /// <summary>
        /// アタッチされているTouchScriptイベントの状態を変更する
        /// </summary>
        /// <param name="isEnable"></param>
        private void SetGesturesEnabled(bool isEnable)
        {
            _flickGesture.enabled = isEnable;
            pressGesture.enabled = isEnable;
            releaseGesture.enabled = isEnable;
        }

        public async UniTask MoveAsync(Vector3 targetPosition, CancellationToken token)
        {
            try {
                SetGesturesEnabled(false);
                _startMoveSubject.OnNext(Unit.Default);

                SoundManager.Instance.PlaySE(ESEKey.Bottle_Move);
                while (transform.position != targetPosition) {
                    transform.position = Vector2.MoveTowards(transform.position, targetPosition, _SPEED);
                    await UniTask.Yield(PlayerLoopTiming.FixedUpdate, token);
                }

                _endMoveSubject.OnNext(Unit.Default);
                SetGesturesEnabled(true);
            } catch (OperationCanceledException) { }
        }

        /// <summary>
        /// ワープ開始時のイベント発火
        /// </summary>
        public void OnStartWarp()
        {
            _startMoveSubject.OnNext(Unit.Default);
        }

        /// <summary>
        /// ワープ終了時のイベント発火
        /// </summary>
        public void OnEndWarp()
        {
            _endMoveSubject.OnNext(Unit.Default);
        }
    }
}
