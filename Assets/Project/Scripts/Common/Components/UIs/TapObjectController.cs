using UnityEngine;

namespace Treevel.Common.Components.UIs
{
    [RequireComponent(typeof(Animator))]
    public class TapObjectController : MonoBehaviour
    {
        /// <summary>
        /// 自身を削除する(Animatorから呼び出し)
        /// </summary>
        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}
