namespace LTS_ToolKit.CharacterController
{
    using UnityEngine;
    using Unity.Entities;
    using Unity.Mathematics;

    [ UpdateBefore( typeof( TransformSystem ) ) ]
    public class VelocitySystem : ComponentSystem
    {
        private struct VelocityEntityFilter
        {
            public Velocity VelocityComponent;
        }

        protected override void OnUpdate()
        {
            float deltaTime = Time.deltaTime;
            foreach( VelocityEntityFilter entity in GetEntities<VelocityEntityFilter>() )
            {
                Velocity velocity = entity.VelocityComponent;
                velocity.Delta = velocity.Value * deltaTime;
            }
        }
    }
}