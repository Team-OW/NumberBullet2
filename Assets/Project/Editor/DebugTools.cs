using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Treevel.Common.Entities;
using Treevel.Common.Managers;
using UnityEditor;
using UnityEngine;

namespace Treevel
{
    public class DebugTools : MonoBehaviour
    {
        [MenuItem("Tools/Debug Command/Open Error Window")]
        private static void OpenErrorWindow()
        {
            if (!EditorApplication.isPlaying) {
                Debug.LogWarning("Only available in play mode");
                return;
            }

            var errorCodes = Enum.GetValues(typeof(EErrorCode)) as EErrorCode[];
            var rand = new System.Random();
            var errorCode = errorCodes?.ElementAt(rand.Next(errorCodes.Length)) ?? EErrorCode.UnknownError;

            UIManager.Instance.ShowErrorMessageAsync(errorCode).Forget();
        }
    }
}
