using UnityEngine;
using UnityEngine.UI;

namespace Data.UI.Scripts
{
    public class SpriteChangeButton : MonoBehaviour
    {
        [SerializeField] private Sprite firstSprite;
        [SerializeField] private Sprite secondSprite;

        private Button _button;
        private Image _image;
        
        private bool _isFirstSprite;

        private void Awake()
        {
            _isFirstSprite = true;
            
            _button = GetComponent<Button>();
            _image = GetComponent<Image>();

            _button.onClick.AddListener(ChangeSprite);
        }
        
        private void ChangeSprite()
        {
            if (_isFirstSprite)
            {
                _image.sprite = secondSprite;
                _isFirstSprite = false;
                return;
            }

            _image.sprite = firstSprite;
            _isFirstSprite = true;
        }
    }
}
