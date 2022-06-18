using System;
using System.Collections.Generic;
using UnityEngine;

namespace Player.Combat
{
    public class Metronome : MonoBehaviour
    {
        public int tempo = 80;
        public bool inCombat;
        
        private readonly HashSet<IMetronomeListener> _listeners = new();
        private AudioSource _audioSource;
        private float _timeSinceLastBeat;

        void Start()
        {
            _audioSource = gameObject.GetComponent<AudioSource>();
        }

        void Update()
        {
            if (inCombat && Time.deltaTime != 0 && tempo != 0)
            {
                _timeSinceLastBeat += Time.deltaTime;
                if (_timeSinceLastBeat >= 60f / tempo)
                {
                    _timeSinceLastBeat -= 60f / tempo;
                    Beat();
                }
            }
            else
            {
                _timeSinceLastBeat = 0;
            }
        }

        protected virtual void Beat()
        {
            _audioSource.Play();
            foreach (IMetronomeListener listener in _listeners)
            {
                listener.MetronomeBeat();
            }
        }

        public void AddListener(IMetronomeListener listener)
        {
            _listeners.Add(listener);
        }
    }
}
