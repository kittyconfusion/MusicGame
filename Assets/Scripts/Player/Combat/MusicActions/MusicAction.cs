using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace Player.Combat.MusicActions
{
    public abstract class MusicAction
    {
        public void Use(Scale scale, AudioSource source)
        {
            source.PlayOneShot(Use(scale));
        }

        protected abstract AudioClip Use(Scale scale);

        public abstract Sprite GetSprite();
    }
}