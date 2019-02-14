namespace LTS_ToolKit.CharacterController
{
    using Unity.Entities;

    [ System.Serializable ]
    public struct Gravity : IComponentData
    {
        public float Value;
    }

    public class GravityComponent : ComponentDataWrapper<Gravity>
    {
        // Wrapper class
    }
}