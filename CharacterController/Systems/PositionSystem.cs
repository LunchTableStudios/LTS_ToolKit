namespace LTS_ToolKit.CharacterController
{
    using UnityEngine;
    using Unity.Entities;
    using Unity.Jobs;
    using Unity.Burst;
    using Unity.Mathematics;
    using Unity.Transforms;
    using Unity.Collections;
    
    public class PositionSystem : JobComponentSystem
    {
        [ BurstCompile ]
        private struct ApplyVelocityToPositionJob : IJobProcessComponentData<Position, Velocity>
        {
            public void Execute( ref Position position, [ ReadOnly ] ref Velocity velocity )
            {
                position.Value.x += velocity.Delta.x;
                position.Value.y += velocity.Delta.y;
            }
        }

        protected override JobHandle OnUpdate( JobHandle inputDeps )
        {
            return new ApplyVelocityToPositionJob().Schedule( this, inputDeps );
        }
    }
}