using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Player.Combat.MusicActions;
using UnityEngine;
using UnityEngine.UI;

namespace Player.Combat
{
    public class PlayerCombatController : MonoBehaviour, IMetronomeListener
    {
        public Image nextActionImage;
        public Sprite emptyAction;
        public KeyCode chord1 = KeyCode.Alpha1;
        public KeyCode chord2 = KeyCode.Alpha2;
        public KeyCode chord3 = KeyCode.Alpha3;
        public KeyCode chord4 = KeyCode.Alpha4;
        public KeyCode chord5 = KeyCode.Alpha5;
        public KeyCode chord6 = KeyCode.Alpha6;
        public KeyCode chord7 = KeyCode.Alpha7;

        private readonly Dictionary<KeyCode, MusicAction> _keyMappings = new();

        private PlayerStats _stats;
        private AudioSource _audioSource;
        private MusicAction _action;
        
        private void Start()
        {
            _keyMappings.Add(chord1, new Chord1());
            _keyMappings.Add(chord2, new Chord2());
            _keyMappings.Add(chord3, new Chord3());
            _keyMappings.Add(chord4, new Chord4());
            _keyMappings.Add(chord5, new Chord5());
            _keyMappings.Add(chord6, new Chord6());
            _keyMappings.Add(chord7, new Chord7());

            _stats = gameObject.GetComponent<PlayerStats>();
            _audioSource = gameObject.GetComponent<AudioSource>();
            
            gameObject.GetComponentInChildren<Metronome>().AddListener(this);
        }
        
        private void Update()
        {
            foreach ((KeyCode key, MusicAction action) in _keyMappings)
            {
                if (Input.GetKeyDown(key))
                {
                    _action = action;
                    nextActionImage.sprite = action.GetSprite();
                }
            }
        }
        

        public void MetronomeBeat()
        {
            if (_action != null)
            {
                _action.Use(_stats.scale, _audioSource);
                _action = null;

                nextActionImage.sprite = emptyAction;
            }
        }
    }
}