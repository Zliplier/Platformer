using System.Collections;
using Player.Data.Combat;
using UnityEngine;

namespace Player.Script
{
    public class PlayerCombatController : PlayerScript
    {

        public PlayerMouseController mouseController;
        
        private SO_AttackData attackData;
        private Coroutine co_Attack;
        public bool IsAttacking => co_Attack != null;
        
        private void OnEnable()
        {
            playerInputMap.leftMouseDownEvent += LeftMouse;
            playerInputMap.rightMouseDownEvent += RightMouse;
        }

        private void OnDisable()
        {
            playerInputMap.leftMouseDownEvent -= LeftMouse;
            playerInputMap.rightMouseDownEvent -= RightMouse;
        }
        
        private void LeftMouse()
        {
            
        }

        private void RightMouse()
        {
            
        }



        private IEnumerator Attacking()
        {
            
            
            
            
            
            yield return null;
        }
    }
}