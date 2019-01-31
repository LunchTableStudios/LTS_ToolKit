namespace LTS_ToolKit.CharacterController
{
    using UnityEngine;
    using Unity.Entities;
    using Unity.Mathematics;
    
    public class TransformSystem : ComponentSystem
    {
        private struct TransformEntityFilter
        {
            public readonly Velocity VelocityComponent;
            public Transform TransformComponent;
        }

        protected override void OnUpdate()
        {
            foreach( TransformEntityFilter entity in GetEntities<TransformEntityFilter>() )
            {
                Velocity velocity = entity.VelocityComponent;
                Transform transform = entity.TransformComponent;

                transform.Translate( new Vector3( velocity.Delta.x, velocity.Delta.y, 0 ) );
            }
        }
    }
}