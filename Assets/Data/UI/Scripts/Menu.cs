using Data.Camera;
using Data.Game_Manager;
using UI;
using UnityEngine;

namespace Data.UI.Scripts
{
    public class Menu : MonoBehaviour
    { 
        [SerializeField] private TwoSidesTransition mainMenuTransitions;
        [SerializeField] private TwoSidesTransition restartButtonTransitions;
        
        private void OnEnable()
        {
            GameManager.OnFinished += restartButtonTransitions.ToTransition;
            CameraSettings.OnCameraReset += mainMenuTransitions.ToTransition;
        }

        private void OnDisable()
        {
            GameManager.OnFinished -= restartButtonTransitions.ToTransition;
            CameraSettings.OnCameraReset -= mainMenuTransitions.ToTransition;
        }
    }
}
