using System;
using UnityEngine;

namespace Player.Data.Combat
{
    [Serializable]
    public class AttackFrame
    {
        public float startTime;
        public float endTime;

        public HItBox[] hitBoxes;
    
        public bool allowMovement = false;
    }
    
    [Serializable]
    public struct HItBox
    {
        public Vector3 hitboxOffset;
        public Vector3 hitboxSize;
    }
}