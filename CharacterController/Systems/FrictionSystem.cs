namespace LTS_ToolKit.CharacterController
{
    using Unity.Entities;
    using Unity.Jobs;
    using Unity.Burst;
    using Unity.Collections;

    [ UpdateAfter( typeof( VelocitySystem ) ) ]
    public class FrictionSystem : JobComponentSystem
    {
        [ BurstCompile ]
        private struct ApplyFrictionToVelocityJob : IJobProcessComponentData<Velocity, Friction>
        {
            public void Execute( ref Velocity velocity, [ ReadOnly ] ref Friction friction )
            {
                velocity.Value *= 1 - friction.Value;
            }
        }

        protected override JobHandle OnUpdate( JobHandle inputDeps )
        {
            return new ApplyFrictionToVelocityJob().Schedule( this, inputDeps );
        }
    }
}