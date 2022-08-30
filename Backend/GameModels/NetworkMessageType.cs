namespace GameModels
{
    public enum NetworkMessageType : ushort
    {
        // Server messages.
        ServerMessage = 1 << 14,
        // Client messages.
        ClientMessage = 1 << 15,
        // Base message type mask.
        MessageMask = 0b0011_1111_1111_1111, // 16383

        /// Successful request
        Ok = 200 | ServerMessage,

        /// Malformed request
        BadRequest = 400 | ServerMessage,

        /// Unexpected error on server.
        ServerError = 500 | ServerMessage,

        /// Join a region.
        RegionJoin = 1000,
        ClientRegionJoin = RegionJoin | ClientMessage,

        /// Leave a region.
        RegionLeave = 1001,
        ClientRegionLeave = RegionLeave | ClientMessage,

        /// Chat messages.
        ChatMessage = 2000,
        ClientChatMessage = ChatMessage | ClientMessage,
        ServerChatMessage = ChatMessage | ServerMessage,

        /// Initialize a player.
        PlayerInit = 3000,
        ClientPlayerInit = PlayerInit | ClientMessage,
        ServerPlayerInit = PlayerInit | ServerMessage,

        /// Remove a player.
        PlayerRemove = 3001,
        ClientPlayerRemove = PlayerRemove | ClientMessage,
        ServerPlayerRemove = PlayerRemove | ServerMessage,

        /// Player move update.
        PlayerMovement = 3002,
        ClientPlayerMovement = PlayerMovement | ClientMessage,
        ServerPlayerMovement = PlayerMovement | ServerMessage,

        /// Non-targeted player interaction.
        PlayerInteract = 3003,
        ClientPlayerInteract = PlayerInteract | ClientMessage,
        ServerPlayerInteract = PlayerInteract | ServerMessage,

        /// Player interaction with target position.
        PlayerInteractPosition = 3004,
        ClientPlayerInteractPosition = PlayerInteractPosition | ClientMessage,
        ServerPlayerInteractPosition = PlayerInteractPosition | ServerMessage,
    }
}
