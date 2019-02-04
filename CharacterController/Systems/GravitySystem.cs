namespace LTS_ToolKit.CharacterController
{
    using UnityEngine;
    using Unity.Entities;

    [ UpdateBefore( typeof( VelocitySystem ) ) ]
    public class GravitySystem : ComponentSystem
    {
        private struct GravityEntityFilter
        {
            public readonly Gravity GravityComponent;
            public Velocity VelocityComponent;
        }

        protected override void OnUpdate()
        {
            float deltaTime = Time.deltaTime;
            foreach( GravityEntityFilter entity in GetEntities<GravityEntityFilter>() )
            {
                entity.VelocityComponent.Value.y -= entity.GravityComponent.Value * deltaTime;
            }
        }
    }
}