using UnityEngine;

namespace Treevel.Common.Components.UIs
{
    [RequireComponent(typeof(Canvas))]
    public class TapObjectGenerator : MonoBehaviour
    {
        public GameObject tapObjectController;
        private Vector3 _touchPosition;

        void Update()
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) {
                _touchPosition = Input.mousePosition;
                var tapObject = Instantiate(tapObjectController, transform, false);
                tapObject.transform.position = _touchPosition;
            }
        }
    }
}
