namespace GameModels.Player
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
}
