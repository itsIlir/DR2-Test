using DarkRift;

namespace GameModels.Status
{
<<<<<<<< HEAD:Backend/GameModels/RegionJoin.cs
    public struct RegionJoin : INetworkData
    {
        public const NetworkMessageType StaticMessageType = NetworkMessageType.RegionJoin;
========
    public struct OkResponse : INetworkData
    {
        public const NetworkMessageType StaticMessageType = NetworkMessageType.Ok;
>>>>>>>> master:Backend/GameModels/Status/OkResponse.cs
        public const SendMode StaticSendMode = SendMode.Reliable;

        public NetworkMessageType MessageType => StaticMessageType;
        public SendMode SendMode => StaticSendMode;

<<<<<<<< HEAD:Backend/GameModels/RegionJoin.cs
        // The id of the room the client wants to join.
        public uint RegionId;

        public void Deserialize(DeserializeEvent e)
        {
            RegionId = e.Reader.ReadUInt32();
========
        public NetworkMessageType Request;

        public void Deserialize(DeserializeEvent e)
        {
            Request = (NetworkMessageType)e.Reader.ReadUInt16();
>>>>>>>> master:Backend/GameModels/Status/OkResponse.cs
        }

        public void Serialize(SerializeEvent e)
        {
<<<<<<<< HEAD:Backend/GameModels/RegionJoin.cs
            e.Writer.Write(RegionId);
========
            e.Writer.Write((ushort)Request);
>>>>>>>> master:Backend/GameModels/Status/OkResponse.cs
        }
    }
}
