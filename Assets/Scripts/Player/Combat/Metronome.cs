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
        private int _framesInCombat;
        private int _framesPerBeat;

        void Start()
        {
            _audioSource = gameObject.GetComponent<AudioSource>();
        }

        void FixedUpdate()
        {
            if (inCombat && Time.fixedUnscaledDeltaTime != 0 && tempo != 0)
            {
                if (_framesInCombat++ % Mathf.RoundToInt(60 / Time.fixedUnscaledDeltaTime / tempo) == 0)
                {
                    Beat();
                }
            }
            else
            {
                _framesInCombat = 0;
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
