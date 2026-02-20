using UnityEngine;

namespace Player.Script
{
    public class PlayerMouseController : PlayerScript
    {
        public Vector3 mousePosition => Camera.main.ScreenToWorldPoint(new Vector3(playerInputMap.mousePosition.x,
            playerInputMap.mousePosition.y, Mathf.Abs(Camera.main.transform.position.z)));
        public Vector3 mouseDirection => CalculateMouseDirection();
        
        public Vector3 CalculateMouseDirection()
        {
            Vector3 direction = mousePosition - _player.transform.position;
            direction.Normalize();
            
            return direction;
        }
    }
}