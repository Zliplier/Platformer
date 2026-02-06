using System;
using UnityEngine;

namespace Player.Data
{
    [CreateAssetMenu(fileName = "MovementStats", menuName = "Player/MovementStats")]
    public class SO_MovementStats : ScriptableObject
    {
        [Header("Gravity")]
        public Vector3 gravity;
        public float gravityAccel;
        
        [Header("Movements")]
        public float walkSpeed = 12.5f;
        public float aimMovementMultiplier = 0.2f;
        [Range(0, 1)] public float aimDotThreshold = 0.75f;
        public float groundAccel = 5f;
        public float groundDecel = 20f;
        public float airAccel = 5f;
        public float airDecel = 5f;
        [Range(0, 1)] public float turnCompensation = 0.5f;
        
        [Header("Jump")]
        public float jumpHeight = 5f;
        public float timeTillApex = 0.35f;
        public float hangTimeApex = 0.075f;
        public AnimationCurve jumpCurve;
        public int jumpAllowed = 2;
        public float jumpBufferTime = 0.125f;
        public float jumpCoyoteTime = 0.1f;
        
        [Header("Collision")]
        public LayerMask groundLayer;
        public Vector3 groundBoxSize;
        public float groundRayDistance;

        [Header("Debug")]
        public bool showVelocity;
        public bool showGravity;
        public bool showJumping;
        public bool showGroundBox;
    }
}