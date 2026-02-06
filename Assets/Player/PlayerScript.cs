using System;
using Player.Data;
using UnityEngine;
using Zlipacket.CoreZlipacket.Player.Input;
using Zlipacket.CoreZlipacket.Player.Input.InputMap;

namespace Player
{
    public abstract class PlayerScript : MonoBehaviour
    {
        public Player _player;
        
        public GameObject bodyRoot => _player.bodyRoot;
        public Rigidbody rb => _player.rb;
        public Collider col => _player.col;
        public Transform feetPos => _player.feetPos;
        public SO_InputReader inputReader => _player.inputReader;
        public PlayerInputMap playerInputMap => inputReader.playerInputMap;
        public UIInputMap uiInputMap => inputReader.uiInputMap;
        
        public SO_MovementStats MoveStats => _player.moveStats;

        private void Awake()
        {
            if (_player == null)
                _player = GetComponent<Player>();
        }
        
        protected virtual void Start()
        {
            
        }
    }
}