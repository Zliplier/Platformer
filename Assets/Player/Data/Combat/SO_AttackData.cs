using UnityEngine;

namespace Player.Data.Combat
{
    [CreateAssetMenu(menuName = "Combat/Attack Data")]
    public class SO_AttackData : ScriptableObject
    {
        public string animationTrigger;

        public float totalDuration;

        public AttackFrame[] frames;

        public float damage;
        public float hitstop;
    }
}