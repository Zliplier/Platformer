using Player.Data.Combat;

namespace Player.Script
{
    public class PlayerCombatController : PlayerScript
    {

        private SO_AttackData attackData;
        public bool IsAttacking => attackData != null;
        
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
        
        
        
        
    }
}