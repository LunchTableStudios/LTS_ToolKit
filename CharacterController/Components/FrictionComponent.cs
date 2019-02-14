namespace LTS_ToolKit.CharacterController
{
    using UnityEngine;
    using Unity.Entities;

    [ System.Serializable ]
    public struct Friction : IComponentData
    {
        [ Range( 0, 1 ) ]
        public float Value;
    }

    public class FrictionComponent : ComponentDataWrapper<Friction>
    {
        // Wrapper class
    }
}