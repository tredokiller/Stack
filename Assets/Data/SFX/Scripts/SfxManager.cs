using Data.Game_Manager;
using Data.Stack_Block.Scripts;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace Data.SFX.Scripts
{
    public class SfxManager : MonoBehaviour
    {
        [SerializeField] private AudioClip[] slicedBlockSounds;
        [SerializeField] private AudioClip perfectBlockSound;
        
        private AudioSource[] _audioPlayers;

        [SerializeField] private GameManager gameManager;
        
        [SerializeField] private AudioMixerGroup masterGroup;
        [SerializeField] private AudioMixerGroup pitchGroup;

        private bool sfxOn = true;
        
        private void Awake()
        {
            _audioPlayers = GetComponentsInChildren<AudioSource>();
        }

        private void OnEnable()
        {
            StackBlock.OnBlockSliced += MakeBlockSlicedSound;
            StackBlock.OnBlockPlacedPerfectly += MakeBlockPlacedPerfectlySound;
        }

        private void MakeBlockSlicedSound()
        { 
            int randomIndex = Random.Range(0, slicedBlockSounds.Length);
            var clip = slicedBlockSounds[randomIndex];

            MakeSound(clip);
        }

        private void MakeBlockPlacedPerfectlySound()
        {
            if (gameManager.RowScores == 1)
            {
                pitchGroup.audioMixer.SetFloat("Pitch", 1);
            }
            else
            {
                float calculatedPitch = 1f + ((gameManager.RowScores - 1f) / 10f);
                pitchGroup.audioMixer.SetFloat("Pitch", calculatedPitch);
            }

            MakeSound(perfectBlockSound, true);
        }

        private void MakeSound(AudioClip clip, bool isPitch = false)
        {
            foreach (var audioPlayer in _audioPlayers)
            {
                if (audioPlayer.isPlaying)
                {
                    continue;
                }

                if (isPitch)
                {
                    audioPlayer.outputAudioMixerGroup = pitchGroup;
                }
                else
                {
                    audioPlayer.outputAudioMixerGroup = masterGroup;
                }
                
                audioPlayer.clip = clip;
                audioPlayer.Play();
                return;
            }
        }


        private void OnDisable()
        {
            StackBlock.OnBlockSliced -= MakeBlockSlicedSound;
            StackBlock.OnBlockPlacedPerfectly -= MakeBlockPlacedPerfectlySound;
        }

        public void SetSfx()
        {
            sfxOn = !sfxOn;

            float volume = 0;
            
            if (sfxOn == false)
            {
                volume = -80f;
            }

            pitchGroup.audioMixer.SetFloat("Volume", volume);
            masterGroup.audioMixer.SetFloat("Volume", volume);
        }
    }
}
