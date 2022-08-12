namespace GameModels
{
    public enum NetworkMessageType : ushort
    {
        ObjectInit = 0,
        ObjectRemove = 1,
        ObjectLocation = 2,
        PlayerInput = 3,
        PlayerInteract = 4,
        ChatMessage = 5,
    }
}
