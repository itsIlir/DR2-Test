using System;

namespace GameModels
{
    public class Tags
    {
        public enum Tag : ushort
        { 
            SPAWN_PLAYER = 0,
            PLAYER_MOVE = 1,
            TEXT_MSG = 2,
            PLAYER_REMOVE = 3
        }
    }
}
