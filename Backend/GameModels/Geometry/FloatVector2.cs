using DarkRift;

namespace GameModels.Geometry
{
    public struct FloatVector2 : IDarkRiftSerializable
    {
        public FloatVector2(float x, float y)
            => (X, Y) = (x, y);

        public float X, Y;

        public void Deserialize(DeserializeEvent e)
        {
            X = e.Reader.ReadSingle();
            Y = e.Reader.ReadSingle();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(X);
            e.Writer.Write(Y);
        }
    }
}
