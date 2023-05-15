using System;
using Data.Game_Manager;
using Data.Stack_Block.Scripts;
using DG.Tweening;
using UnityEngine;

namespace Data.Camera
{
    public class CameraSettings : MonoBehaviour
    {
        private UnityEngine.Camera _camera;
        private readonly float _tweenDuration = 1f;

        private const float PlayCameraSize = 2.5f;
        private const float MenuCameraSize = 4f;

        private const float HeightToRestart = 50f;

        private bool _firstStart = false;

        [SerializeField] private StackBlock stackBlock;
        [SerializeField] private Transform startCameraPosition;

        public static Action OnCameraReset;
        
        private void Awake()
        {
            _camera = UnityEngine.Camera.main;
            _camera.orthographicSize = MenuCameraSize;
        }

        private void OnEnable()
        {
            GameManager.OnRestart += CameraPositionToHeight;
            GameManager.OnStart += () => ChangeCameraSize(PlayCameraSize);
            GameManager.OnFinished += () => ChangeCameraSize(MenuCameraSize);
            
            StackBlock.OnBlockPlaced += UpdateCameraPosition;
        }

        private void ChangeCameraSize(float size)
        {
            _camera.DOOrthoSize(size, _tweenDuration);
        }

        private void CameraPositionToHeight()
        {
            transform.DOMoveY(transform.position.y + HeightToRestart, _tweenDuration).OnComplete(ResetCameraPosition);
        }

        private void ResetCameraPosition()
        {
            transform.DOMoveY(startCameraPosition.position.y, _tweenDuration);
            OnCameraReset.Invoke();
        }
        
        private void UpdateCameraPosition()
        {
            transform.DOMoveY(transform.position.y + stackBlock.transform.localScale.y, _tweenDuration);
        }
        
        private void OnDisable()
        {
            GameManager.OnRestart -= CameraPositionToHeight;
            GameManager.OnStart -= () => ChangeCameraSize(PlayCameraSize);
            GameManager.OnFinished -= () => ChangeCameraSize(MenuCameraSize);
            
            StackBlock.OnBlockPlaced -= UpdateCameraPosition;
        }
    }
}
