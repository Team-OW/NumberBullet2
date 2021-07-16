using UniRx;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Bottle
{
    [RequireComponent(typeof(Animator))]
    public class BottleAttributeControllerBase : GameObjectControllerBase
    {
        protected Animator animator;

        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();
        }
    }
}
