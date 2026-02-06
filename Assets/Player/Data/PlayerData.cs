using System;

namespace Player.Data
{
    [Serializable]
    public class PlayerData
    {
        public SO_MovementStats movementStats;
        
        public PlayerData(PlayerData playerData)
        {
            movementStats = playerData.movementStats;
        }
    }
}