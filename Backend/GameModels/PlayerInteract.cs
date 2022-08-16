using DarkRift;

namespace GameModels
{
    public enum PlayerInteractType : ushort
    {
        None =           0,

        // General actions
        Jump =           1,

        // Gameplay actions
        FishingThrow =   1000,
        FishingCaught =  1001,

        FarmingPlant =   1100,
        FarmingTill =    1101,
        FarmingWater =   1102,
        FarmingHarvest = 1103,
        FarmingTalk =    1104,
    }

    public struct PlayerInteract : INetworkData
    {
        public const NetworkMessageType StaticMessageType = NetworkMessageType.PlayerInteract;
        public const SendMode StaticSendMode = SendMode.Reliable;
        public NetworkMessageType MessageType => StaticMessageType;
        public SendMode SendMode => StaticSendMode;

        /// The player object's Id.
        public ushort Id;

        /// The type of interaction performed by the player object.
        public PlayerInteractType Interaction;

        /// Interaction target, either object or location.
        public TargetData Target;

        /// Player object's transform.
        public MovementData Movement;

        public void Deserialize(DeserializeEvent e)
        {
            Id = e.Reader.ReadUInt16();
            Interaction = (PlayerInteractType) e.Reader.ReadUInt16();
            Target.Deserialize(e);
            Movement.Deserialize(e);
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(Id);
            e.Writer.Write((ushort)Interaction);
            Target.Serialize(e);
            Movement.Serialize(e);
        }
    }
}
