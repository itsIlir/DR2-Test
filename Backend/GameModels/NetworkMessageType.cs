namespace GameModels
{
    public enum NetworkMessageType : ushort
    {
        RegionJoin = 10,
        RegionLeave = 11,

        ObjectInit = 100,
        ObjectRemove = 101,
        ObjectLocation = 102,
        ObjectTransfer = 103,

        PlayerMovement = 110,
        PlayerInteract = 111,

        ChatMessage = 200,
    }
}
