using System;
using Player.Data;
using Player.Script.Animation;
using Unity.Cinemachine;
using UnityEditor.Animations;
using UnityEngine;
using Zlipacket.CoreZlipacket.Player.Input;
using Zlipacket.CoreZlipacket.Player.Input.InputMap;

namespace Player
{
    public class Player : MonoBehaviour
    {
        public SO_Player player;
        [HideInInspector] public PlayerData playerData;
        public SO_InputReader inputReader => player.inputReader;
        public PlayerInputMap playerInputMap => inputReader.playerInputMap;
        public UIInputMap uiInputMap => inputReader.uiInputMap;
        public Vector3 mousePosition => Camera.main.ScreenToWorldPoint(new Vector3(playerInputMap.mousePosition.x, playerInputMap.mousePosition.y, Mathf.Abs(Camera.main.transform.position.z)));

        public GameObject bodyRoot;
        public Rigidbody rb;
        public Collider col;
        public Animator animator;
        public PlayerAnimationController playerAnimationController;
        public Transform feetPos;
        public CinemachineCamera cam;
        
        public SO_MovementStats moveStats => playerData.movementStats;
        
        private void Awake()
        {
            playerData = new PlayerData(player.playerData);
            rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            
        }
    }
}