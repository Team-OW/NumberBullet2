using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Treevel.Common.Networks.Database;
using Treevel.Common.Utils;

namespace Treevel.Common.Networks.Requests
{
    public abstract class ServerRequestBase<T>
    {
        /// <summary>
        /// リモートサーバへ問い合わせするためのサービスクラスのインスタンス
        /// </summary>
        protected static IDatabaseService remoteDatabaseService = new PlayFabDatabaseService();

        /// <summary>
        /// リクエストを実行
        /// </summary>
        /// <returns>機能に応じて返す型を定義</returns>
        public abstract UniTask<T> Execute();
    }

    public abstract class GetServerRequestBase<TResult> : ServerRequestBase<TResult> where TResult: new()
    {
        protected string key;

        public override async UniTask<TResult> Execute()
        {
            // TODO: リクエスト数を減らして、リモートにアクセスする
            return PlayerPrefsUtility.GetObject<TResult>(key);
        }
    }

    public abstract class GetListServerRequestBase<T> : ServerRequestBase<IEnumerable<T>>
    {
        protected IEnumerable<string> keys;

        public override async UniTask<IEnumerable<T>> Execute()
        {
            return await remoteDatabaseService.GetListDataAsync<T>(keys);
        }
    }

    public abstract class UpdateServerRequestBase<TDataType> : ServerRequestBase<bool>
    {
        protected string key;

        protected TDataType data;

        public override async UniTask<bool> Execute()
        {
            if (await remoteDatabaseService.UpdateDataAsync(key, data)) {
                // 成功したらローカルも更新する
                PlayerPrefsUtility.SetObject(key, data);
            }

            return false;
        }
    }
}
