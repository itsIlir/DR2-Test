namespace GameModels
{
    public enum NetworkMessageType : ushort
    {
        // Messages sent from the server to clients.
        ServerMessage = 1 << 14,
        // Messages sent from a client to the server.
        ClientMessage = 1 << 15,
        // Base message type mask.
        MessageMask = 0b0011_1111_1111_1111, // 16383

        // Successful request
        Ok = 200 | ServerMessage,

        // Malformed request
        BadRequest = 400 | ServerMessage,

        // Unexpected error on server.
        ServerError = 500 | ServerMessage,

        // Join a region.
        ClientRegionJoin = 1000 | ClientMessage,

        // Leave a region.
        ClientRegionLeave = 1001 | ClientMessage,

        // Chat messages.
        ClientChatMessage = 2000 | ClientMessage,
        ServerChatMessage = 2000 | ServerMessage,

        // Initialize a player.
        ClientPlayerInit = 3000 | ClientMessage,
        ServerPlayerInit = 3000 | ServerMessage,

        // Remove a player.
        ClientPlayerRemove = 3001 | ClientMessage,
        ServerPlayerRemove = 3001 | ServerMessage,

        // Player move update.
        ClientPlayerMovement = 3002 | ClientMessage,
        ServerPlayerMovement = 3002 | ServerMessage,

        // Player jump.
        ClientPlayerJump = 3003 | ClientMessage,
        ServerPlayerJump = 3003 | ServerMessage,
    }
}
