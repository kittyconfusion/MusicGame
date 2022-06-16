using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    public PlayerStats playerStats;
    public Sprite fullSprite;
    public Sprite emptySprite;
    public float spriteSize = 25;
    public float spacing = 4;
    public float spriteStartX = 417;
    public float barY = 176;
    public int healthPerSprite = 5;
    public float changeSmoothing = 0.5f;

    private readonly List<Image> _sprites = new();
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
            _sprites.RemoveAt(_numSprites);
        }

        if (_currHealth != playerStats.GetHealth())
        {
            _currHealthF = Mathf.SmoothDamp(_currHealthF, playerStats.GetHealth(), ref _changeSpeed, changeSmoothing);
            SetHealth(Mathf.RoundToInt(_currHealthF));
        }
        else
        {
            _currHealthF = _currHealth;
        }
    }

    // TODO: make this not use while loops
    private void SetHealth(int health)
    {
        if (_currHealth != health)
        {
            for (int i = 0; i < _sprites.Count; i++)
            {
                _sprites[i].fillAmount = Mathf.Clamp01((float) health/healthPerSprite - i);
            }
        }

        _currHealth = health;

        /*if (_currHealth < health)
        {
            for (int i = 0; i < _sprites.Count; i++)
            {
                _sprites[i].fillAmount = Mathf.Clamp01((float) health/healthPerSprite - i);
            }
            // foreach (Image sprite in _sprites)
            // {
            //     sprite.fillAmount = Mathf.Clamp01(health - )
            //     // while (_currHealth < health && sprite.fillAmount < 1)
            //     // {
            //     //     _currHealth += 1;
            //     //     sprite.fillAmount += 1f/healthPerSprite;
            //     // }
            // }
        }

        if (_currHealth > health)
        {
            for (int i = _sprites.Count - 1; i >= 0; i--)
            {
                Image sprite = _sprites[i];
                while (_currHealth > health && sprite.fillAmount > 0)
                {
                    _currHealth -= 1;
                    sprite.fillAmount -= 1f/healthPerSprite;
                }
            }
        }*/
    }

    private void AddSprite(int idx)
    {
        GameObject emptyObj = new GameObject();
        Image emptyImage = emptyObj.AddComponent<Image>();
        RectTransform emptyTransform = emptyObj.GetComponent<RectTransform>();

        emptyTransform.SetParent(transform, true);
        emptyObj.name = "Empty Health";
        emptyImage.sprite = emptySprite;
        emptyTransform.sizeDelta = new Vector2(spriteSize, spriteSize);
        emptyTransform.localScale = Vector3.one;
        emptyTransform.localPosition = new Vector3(spriteStartX - (spacing + spriteSize) * idx, barY, 0);
        
        
        GameObject fullObj = new GameObject();
        Image fullImage = fullObj.AddComponent<Image>();
        RectTransform fullTransform = fullObj.GetComponent<RectTransform>();

        fullTransform.SetParent(emptyTransform);
        fullObj.name = "Full Health";
        fullImage.sprite = fullSprite;
        fullImage.type = Image.Type.Filled;
        fullImage.fillMethod = Image.FillMethod.Horizontal; 
        fullImage.fillOrigin = (int) Image.OriginHorizontal.Right;
        fullImage.fillAmount = 0;
        fullTransform.sizeDelta = new Vector2(spriteSize, spriteSize);
        fullTransform.localScale = Vector3.one;
        fullTransform.localPosition = Vector3.zero;
        
        _sprites.Add(fullImage);
    }
}
