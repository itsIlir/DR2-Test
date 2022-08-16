namespace GameModels
{
    public enum NetworkMessageType : ushort
    {
        ObjectInit = 0,
        ObjectRemove = 1,
        ObjectLocation = 2,
        PlayerMovement = 3,
        PlayerInteract = 4,
        ChatMessage = 5,
    }
}
