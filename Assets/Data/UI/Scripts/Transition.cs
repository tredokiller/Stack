using DG.Tweening;
using UnityEngine;

namespace UI
{
    public class Transition : MonoBehaviour
    {
        public delegate void TransitionFinished();
        
        public static void MakeTransitionFromTo(Transform obj, Vector3 from , Vector3 to , float duration , TransitionFinished callback = null)
        {
            obj.transform.position = from;

            if (callback == null)
            {
                obj.transform.DOMove(to, duration).SetEase(Ease.InSine);
            }
            else
            {
                obj.transform.DOMove(to, duration).OnComplete(callback.Invoke).SetEase(Ease.InSine);
            }
        }
    }
}
