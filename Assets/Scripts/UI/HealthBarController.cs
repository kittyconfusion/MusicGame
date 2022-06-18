using System;
using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HealthBarController : MonoBehaviour
    {
        public PlayerStats playerStats;
        public Sprite fullSprite;
        public Sprite previewSprite;
        public Sprite emptySprite;
        public float spriteSize = 25;
        public float spacing = 3;
        public float spacingFromRight = 4;
        public float spacingFromTop = 4;
        public int healthPerSprite = 5;
        public float changeSmoothing = 0.5f;

        private readonly List<Image> _fullSprites = new();
        private readonly List<Image> _previewSprites = new();
        private int _numSprites;
        private float _changeSpeed;
        private int _currHealth;
        private float _currHealthF;

        private Vector2 _resolution;

        private void Start()
        {
            UnityEngine.Camera camera = UnityEngine.Camera.main;
            _resolution = new Vector2(camera.scaledPixelWidth, camera.scaledPixelHeight);
        }

        // Update is called once per frame
        void Update()
        {
            UnityEngine.Camera camera = UnityEngine.Camera.main;
            Vector2 resolution = new Vector2(camera.scaledPixelWidth, camera.scaledPixelHeight);
            if (!resolution.Equals(_resolution))
            {
                _resolution = resolution;
                Refresh();
            }
            
            int correctSprites = Mathf.CeilToInt(playerStats.GetMaxHealth() / (float) healthPerSprite);

            while (_numSprites < correctSprites)
            {
                AddSprite(_numSprites++);
            }

            while (correctSprites < _numSprites)
            {
                Destroy(transform.GetChild(--_numSprites).gameObject);
            }

            if (_currHealth != playerStats.GetHealth())
            {
                _currHealthF = Mathf.SmoothDamp(_currHealthF, playerStats.GetHealth(), ref _changeSpeed, changeSmoothing);
                SetHealth(Mathf.RoundToInt(_currHealthF), _currHealth > playerStats.GetHealth(), true);
                SetHealth(playerStats.GetHealth(), _currHealth < playerStats.GetHealth(), false);
            }
            else
            {
                _currHealthF = _currHealth;
            }
        }

        private void Refresh()
        {
            for (int i = 0; i < _previewSprites.Count; i++)
            {
                _previewSprites[i].gameObject.transform.parent.localPosition = GetPosition(i);
            }
        }

        // TODO: make this not use while loops
        private void SetHealth(int health, bool setPreviewSprites, bool setCurrHealth)
        {
            List<Image> sprites = setPreviewSprites ? _previewSprites : _fullSprites;
        
            if (_currHealth != health)
            {
                for (int i = 0; i < sprites.Count; i++)
                {
                    if (sprites[i].IsDestroyed())
                    {
                        sprites.RemoveAt(i);
                        continue;
                    }
                
                    sprites[i].fillAmount = Mathf.Clamp01((float) health / healthPerSprite - i);
                }
            }

            if (setCurrHealth)
            {
                _currHealth = health;
            }
        }

        private Vector3 GetPosition(int idx)
        {
            return new Vector3(_resolution.x / 2 - (spacingFromRight + spriteSize / 2) - (spacing + spriteSize) * idx,
                _resolution.y / 2 - (spacingFromTop + spriteSize / 2), 0);
        }

        private void AddSprite(int idx)
        {
            Image empty = CreateSprite(emptySprite, transform, 1, GetPosition(idx));

            Image preview = CreateSprite(previewSprite, empty.transform, 0, Vector3.zero);
            _previewSprites.Add(preview);
        
            Image full = CreateSprite(fullSprite, preview.transform, 0, Vector3.zero);
            _fullSprites.Add(full);
        }

        private Image CreateSprite(Sprite sprite, Transform parent, int fillAmount, Vector3 position)
        {
            GameObject obj = new GameObject();
            Image image = obj.AddComponent<Image>();
            RectTransform objTransform = obj.GetComponent<RectTransform>();

            objTransform.SetParent(parent);
            obj.name = sprite.name;
            image.sprite = sprite;
            image.type = Image.Type.Filled;
            image.fillMethod = Image.FillMethod.Horizontal; 
            image.fillOrigin = (int) Image.OriginHorizontal.Right;
            image.fillAmount = fillAmount;
            objTransform.sizeDelta = new Vector2(spriteSize, spriteSize);
            objTransform.localScale = Vector3.one;
            objTransform.localPosition = position;

            return image;
        }
    }
}
