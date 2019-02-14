namespace LTS_ToolKit.CharacterController
{
    using Unity.Entities;
    using Unity.Mathematics;

    [ System.Serializable ]
    public struct Velocity : IComponentData
    {
        public float2 Value;
        public float2 Delta;
    }

    public class VelocityComponent : ComponentDataWrapper<Velocity>
    {
        // Wrapper class
    }
}