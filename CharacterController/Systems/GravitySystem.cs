namespace LTS_ToolKit.CharacterController
{
    using UnityEngine;
    using Unity.Entities;
    using Unity.Jobs;
    using Unity.Burst;
    using Unity.Collections;

    [ UpdateBefore( typeof( VelocitySystem ) ) ]
    public class GravitySystem : JobComponentSystem
    {
        [ BurstCompile ]
        private struct ApplyGravityJob : IJobProcessComponentData<Velocity, Gravity>
        {
            public float deltaTime;

            public void Execute( ref Velocity velocity, [ ReadOnly ] ref Gravity gravity )
            {
                velocity.Value.y -= gravity.Value * deltaTime;
            }
        }

        protected override JobHandle OnUpdate( JobHandle inputDeps )
        {
            return new ApplyGravityJob()
            {
                deltaTime = Time.deltaTime
            }.Schedule( this, inputDeps );
        }
    }
}

