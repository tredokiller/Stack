using System;
using Data.Game_Manager;
using TMPro;
using UI;
using UnityEngine;

namespace Data.UI.ScoresUI
{
    public class Scores : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoresText;
        [SerializeField] private TextMeshProUGUI bestScoresText;
        
        [SerializeField] private GameManager gameManager;

        [SerializeField] TwoSidesTransition bestScoreTransition;
        [SerializeField] TwoSidesTransition scoreTransition;


        private void Awake()
        {
            UpdateBestScoreText();
        }

        private void OnEnable()
        {
            GameManager.OnScoresRaised += UpdateScoresText;
            GameManager.OnRestart += UpdateScoresText;
            
            GameManager.OnRestart += scoreTransition.FromTransition;
            GameManager.OnStart += scoreTransition.ToTransition;
            
            GameManager.OnRestart += bestScoreTransition.ToTransition;
            GameManager.OnRestart += UpdateBestScoreText;
            
            GameManager.OnStart += bestScoreTransition.FromTransition;
        }
        
        private void UpdateScoresText()
        {
            scoresText.text = gameManager.Scores.ToString();
        }

        private void UpdateBestScoreText()
        {
            bestScoresText.text = gameManager.BestScore.ToString();
        }

        private void OnDisable()
        {
            GameManager.OnScoresRaised -= UpdateScoresText;
            GameManager.OnRestart -= UpdateScoresText;
            
            GameManager.OnRestart -= scoreTransition.FromTransition;
            GameManager.OnStart -= scoreTransition.ToTransition;
            
            GameManager.OnRestart -= bestScoreTransition.ToTransition;
            GameManager.OnRestart -= UpdateBestScoreText;
            
            GameManager.OnStart -= bestScoreTransition.FromTransition;
        }
    }
}
