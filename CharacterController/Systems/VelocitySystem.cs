namespace LTS_ToolKit.CharacterController
{
    using UnityEngine;
    using Unity.Entities;
    using Unity.Jobs;
    using Unity.Burst;
    using Unity.Mathematics;

    public class VelocitySystem : JobComponentSystem
    {
        [ BurstCompile ]
        private struct SetVelocityDeltaJob : IJobProcessComponentData<Velocity>
        {
            public float deltaTime;

            public void Execute( ref Velocity velocity )
            {
                velocity.Delta = velocity.Value * deltaTime;
            }
        }

        protected override JobHandle OnUpdate( JobHandle inputDeps )
        {
            return new SetVelocityDeltaJob()
            {
                deltaTime = Time.deltaTime
            }.Schedule( this, inputDeps );
        }
    }
}