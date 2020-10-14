using Treevel.Common.Components.UIs;
using Treevel.Common.Entities;
using Treevel.Common.Patterns.Singleton;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Treevel.Common.Managers
{
    /// <summary>
    /// 全体ゲームに出現するUI（プログレスバー、メッセージダイアログ等）
    /// を制御するマネージャークラス
    /// </summary>
    public class UIManager : SingletonObject<UIManager>
    {
        /// <summary>
        /// プログレスバーのプレハブ
        /// </summary>
        [SerializeField]
        private AssetReferenceGameObject _progressBar;

        /// <summary>
        /// エラーメッセージのポップアップ
        /// 普段使われない予想なので、使うときだけロード、実体化させる
        /// </summary>
        [SerializeField]
        private AssetReferenceGameObject _errorMessageBoxRef;

        /// <summary>
        /// プログレスバーのインスタンス
        /// </summary>
        public ProgressBar ProgressBar
        {
            get;
            private set;
        }

        /// <summary>
        /// 初期化済みかどうか
        /// </summary>
        public bool Initialized
        {
            get;
            private set;
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            #if !UNITY_EDITOR && UNITY_ANDROID
            // ステータスバーを表示する
            StatusBarController.Show();
            #endif

            // キャンバスがなければ作る
            if (GetComponentInChildren<Canvas>() == null) {
                gameObject.AddComponent<Canvas>();
            }

            var canvas = GetComponentInChildren<Canvas>().transform;

            // キャンバスの下にプログレスバーの実体を生成
            _progressBar.InstantiateAsync(canvas).Completed += (obj) => {
                ProgressBar = obj.Result.GetComponentInChildren<ProgressBar>();
                Initialized = true;
            };
        }

        /// <summary>
        /// エラーメッセージを表示
        /// </summary>
        /// <param name="errorCode">対応するエラーコード</param>
        public void ShowErrorMessage(EErrorCode errorCode)
        {
            var canvas = GetComponentInChildren<Canvas>().transform;
            _errorMessageBoxRef.InstantiateAsync(canvas).Completed += (op) => {
                var messageBoxObj = op.Result;

                // テキスト、エラーコードを設定
                messageBoxObj.GetComponent<ErrorMessageBox>().ErrorCode = errorCode;
            };
        }
    }
}