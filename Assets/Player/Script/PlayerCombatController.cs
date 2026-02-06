namespace Player.Script
{
    public class PlayerCombatController : PlayerScript
    {
        
        
        
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