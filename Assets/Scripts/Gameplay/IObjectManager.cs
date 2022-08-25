using GameModels;

namespace Gameplay
{
    public interface IObjectManager
    {
        void UpdateLocation(uint id, LocationData data);
        void Remove(uint id);
    }
}
