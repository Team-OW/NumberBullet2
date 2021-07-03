using System.Collections;
using System.Linq;
using Treevel.Common.Entities;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Utils;
using Treevel.Modules.GamePlayScene.Bottle;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Treevel.Modules.GamePlayScene.Gimmick
{
    public class MeteoriteController : GimmickControllerBase
    {
        /// <summary>
        /// 目標座標
        /// </summary>
        private Vector2 _targetPos;

        /// <summary>
        /// 目標座標からどれだけ離れた位置に出現させるか
        /// </summary>
        private Vector2 _startPosMargin;

        /// <summary>
        ///
        /// 隕石部分innsekibubunn
        /// </summary>
        [SerializeField] private GameObject _meteorite;

        /// <summary>
        /// 速さ
        /// </summary>
        [SerializeField] private float _speed = 0.05f;

        /// <summary>
        /// 警告オブジェクトインスタンス（ゲーム終了時片付けするためにメンバー変数として持つ）
        /// </summary>
        private GameObject _crack;

        /// <summary>
        /// 隕石の画像の表示時間
        /// </summary>
        [SerializeField] private float _meteoriteDisplayTime = 1.5f;

        /// <summary>
        /// 隕石本体移動させる用フラグ
        /// </summary>
        private bool _isMoving = false;

        private SpriteRenderer _renderer;

        private Animator _animator;

        private void Awake()
        {
            base.Awake();
            _startPosMargin = new Vector2(2.5f * GameWindowController.Instance.GetTileWidth(), 1.5f * GameWindowController.Instance.GetTileWidth());

            _renderer = _meteorite.GetComponent<SpriteRenderer>();
            _animator = _meteorite.GetComponent<Animator>();
            this.OnTriggerEnter2DAsObservable()
                .Where(_ => transform.position.z < 0)
                .Select(other => other.GetComponent<BottleControllerBase>())
                .Where(bottle => bottle && bottle.IsAttackable && !bottle.IsInvincible)
                .Subscribe(bottle => HandleCollision(bottle.gameObject))
                .AddTo(this);
            GamePlayDirector.Instance.GameSucceeded.Subscribe(_ => Destroy(gameObject)).AddTo(this);
            GamePlayDirector.Instance.GameFailed.Subscribe(_ => _animator.speed = 0f);
        }

        protected override void HandleCollision(GameObject other)
        {
            // TODO: Crackのアニメーションに遷移しないようにTriggerを起動する
        }

        public override void Initialize(GimmickData gimmickData)
        {
            base.Initialize(gimmickData);

            // TODO: 特殊なTileを狙わないように指定する
            // MeteoriteとAimingMeteoriteでClassを分ける

            switch (gimmickData.type) {
                case EGimmickType.Meteorite:
                    if (gimmickData.isRandom) {
                        var row = GimmickLibrary.SamplingArrayIndex(gimmickData.randomRow.ToArray());
                        var column = GimmickLibrary.SamplingArrayIndex(gimmickData.randomColumn.ToArray());
                        _targetPos = BoardManager.Instance.GetTilePos(column, row);
                    } else {
                        _targetPos =
                            BoardManager.Instance.GetTilePos((int)gimmickData.targetColumn,
                                                             (int)gimmickData.targetRow);
                    }

                    break;
                case EGimmickType.AimingMeteorite:
                    if (gimmickData.isRandom) {
                        // 乱数インデックスを重みに基づいて取得
                        var randomIndex =
                            GimmickLibrary.SamplingArrayIndex(gimmickData.randomAttackableBottles.ToArray());
                        // 乱数インデックスをボトルIDに変換
                        var targetId = CalcBottleIdByRandomArrayIndex(randomIndex);
                        _targetPos = BoardManager.Instance.GetBottlePosById(targetId);
                    } else {
                        _targetPos = BoardManager.Instance.GetBottlePosById(gimmickData.targetBottle);
                    }

                    break;
                default:
                    throw new System.NotImplementedException("不正なギミックタイプです");
            }

            var position = transform.position;
            position = _targetPos + _startPosMargin;
            position = new Vector2(Mathf.Min(position.x, (GameWindowController.Instance.GetGameCoreSpaceWidth() - _renderer.bounds.size.x) / 2f), Mathf.Min(position.y, (GameWindowController.Instance.GetGameCoreSpaceHeight() - _renderer.bounds.size.y) / 2f));
            transform.position = position;
        }

        public override IEnumerator Trigger()
        {
            _renderer.enabled = true;
            // TODO: 一定時間待機する
            // TODO: 落下アニメーション
            // TODO: Crackを出す
            yield return null;
        }

        /// <summary>
        /// 乱数配列のインデックスをボトルのIdに変換する
        /// </summary>
        /// <param name="index">_randomAttackableBottlesから取ったインデックス</param>
        /// <returns>ボトルのID</returns>
        private static int CalcBottleIdByRandomArrayIndex(int index)
        {
            var bottles = BottleLibrary.OrderedAttackableBottles;
            var bottleAtIndex = bottles.ElementAt(index);
            return bottleAtIndex.Id;
        }
    }
}
