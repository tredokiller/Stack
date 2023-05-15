using System;
using System.Collections;
using System.Linq;
using Data.Camera;
using Data.Input;
using Data.Materials.Scripts;
using Data.Stack_Block.Scripts;
using UnityEngine;

namespace Data.Game_Manager
{
    public class GameManager : MonoBehaviour
    {
        public static Action OnClicked; //Mainly for stopping stack block
        public static Action OnScoresRaised;

        public static Action OnStart;
        public static Action OnFinished;
        
        public static Action OnRestart;

        private const string BestScoreName = "BestScore";

        [Header("Stack Block")] 
        [SerializeField] private MeshRenderer startStackMaterial;
        [SerializeField] private Transform placedBlocks;
        [SerializeField] private float deleteCubesDuration = 1;
        
        public int Scores { private set; get; }
        public int BestScore { private set; get; }
        public int RowScores { private set; get; }

        [SerializeField] private InputManager inputManager;
        
        private GameInput.PlayerActions _playerActions;

        [Header("Fog")]
        [SerializeField] private MeshRenderer fogMaterial;


        private Material _styledMaterial;

        private void Awake()
        {
            BestScore = PlayerPrefs.GetInt(BestScoreName);
            ChangeSceneStyle();
        }

        private void ChangeSceneStyle()
        {
            _styledMaterial = ColorGenerator.GetBasedMaterialWithRandomColor(startStackMaterial.sharedMaterial);
            startStackMaterial.material = _styledMaterial;
            fogMaterial.material.color = _styledMaterial.color;
        }

        private void Start()
        {
            SetFrameRate();
            
            _playerActions = inputManager.GetPlayerActions();
            _playerActions.Action.started += context => OnClicked.Invoke();
        }

        private void OnEnable()
        {
            StackBlock.OnBlockPlaced += RaiseScores;
            StackBlock.OnBlockPlacedPerfectly += () => RowScores += 1;
            
            OnFinished += UpdateBestScore;
            
            CameraSettings.OnCameraReset += ChangeSceneStyle;
            
            StackBlock.OnBlockSliced += () => RowScores = 0;
        }

        private void RaiseScores()
        { 
            Scores += 1;
            OnScoresRaised.Invoke();
        }

        private void UpdateBestScore()
        {
            if (Scores > BestScore)
            {
                BestScore = Scores;
                PlayerPrefs.SetInt(BestScoreName, Scores);
            }
        }
        
        private void OnDisable()
        {
            StackBlock.OnBlockPlacedPerfectly -= () => RowScores += 1;
            StackBlock.OnBlockPlaced -= RaiseScores;
            StackBlock.OnBlockSliced -= () => RowScores = 0;
            
            OnFinished -= UpdateBestScore;
            
            CameraSettings.OnCameraReset -= ChangeSceneStyle;
        }

        public void RestartGame()
        {
            Scores = 0;
            RowScores = 0;
            
            StartCoroutine(DeleteAllCubesCoroutine());

            OnRestart.Invoke();
        }
        
        private IEnumerator DeleteAllCubesCoroutine()
        {
            int childCount = placedBlocks.transform.childCount;
            
            float delay = deleteCubesDuration / childCount;
            
            for (int i = childCount - 1; i >= 0; i--)
            {
                Transform child = placedBlocks.transform.GetChild(i);

                Destroy(child.gameObject);

                yield return new WaitForSeconds(delay);
            }
        }
        
        public void StartGame()
        {
            OnStart.Invoke();
        }
        
        private void OnDestroy()
        {
            _playerActions.Action.canceled -= context => OnClicked.Invoke();
        }

        private void SetFrameRate()
        {
            QualitySettings.vSyncCount = 0;
            
            Resolution[] refreshRate = Screen.resolutions;
            Application.targetFrameRate = refreshRate.Last().refreshRate;

        }
    }
}
