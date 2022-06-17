using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    public PlayerStats playerStats;
    public Sprite fullSprite;
    public Sprite previewSprite;
    public Sprite emptySprite;
    public float spriteSize = 25;
    public float spacing = 4;
    public float spriteStartX = 417;
    public float barY = 176;
    public int healthPerSprite = 5;
    public float changeSmoothing = 0.5f;

    private readonly List<Image> _fullSprites = new();
    private readonly List<Image> _previewSprites = new();
    private int _numSprites;
    private float _changeSpeed;
    private int _currHealth;
    private float _currHealthF;
    
    // Update is called once per frame
    void Update()
    {
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
                
                sprites[i].fillAmount = Mathf.Clamp01((float) health/healthPerSprite - i);
            }
        }

        if (setCurrHealth)
        {
            _currHealth = health;
        }
    }

    private void AddSprite(int idx)
    {
        /*GameObject emptyObj = new GameObject();
        Image emptyImage = emptyObj.AddComponent<Image>();
        RectTransform emptyTransform = emptyObj.GetComponent<RectTransform>();

        emptyTransform.SetParent(transform, true);
        emptyObj.name = "Empty Health";
        emptyImage.sprite = emptySprite;
        emptyTransform.sizeDelta = new Vector2(spriteSize, spriteSize);
        emptyTransform.localScale = Vector3.one;
        emptyTransform.localPosition = new Vector3(spriteStartX - (spacing + spriteSize) * idx, barY, 0);*/

        Image empty = CreateSprite(emptySprite, transform, 1,
            new Vector3(spriteStartX - (spacing + spriteSize) * idx, barY, 0));

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
