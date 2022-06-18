using System;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;

namespace Player.Combat.MusicActions
{
    public class Chord1 : MusicAction
    {
        protected override AudioClip Use(Scale scale)
        {
            switch (scale)
            {
                case Scale.Major:
                {
                    // TODO: do something

                    return Resources.Load<AudioClip>("Sounds/Chords/C Major");
                }
                    
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public override Sprite GetSprite()
        {
            return Resources.Load<Sprite>("Sprites/Next Action/Chords/Major I");
        }
    }
}