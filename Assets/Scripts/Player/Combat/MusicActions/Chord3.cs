using System;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;

namespace Player.Combat.MusicActions
{
    public class Chord3 : MusicAction
    {
        protected override AudioClip Use(Scale scale)
        {
            switch (scale)
            {
                case Scale.Major:
                {
                    // TODO: do something

                    return Resources.Load<AudioClip>("Sounds/Chords/E Minor");
                }
                    
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public override Sprite GetSprite()
        {
            return Resources.Load<Sprite>("Sprites/Next Action/Chords/Minor iii");
        }
    }
}