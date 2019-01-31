namespace LTS_ToolKit.CharacterController
{
    using UnityEngine;
    using Unity.Entities;
    using Unity.Mathematics;

    [ UpdateAfter( typeof( VelocitySystem ) ) ]
    public class CollisionSystem : ComponentSystem
    {

        private struct CollisionEntityFilter
        {
            public Velocity VelocityComponent;
            public CollisionData CollisionComponent;
            public readonly BoxCollider2D ColliderComponent;
        }

        private struct RaycastData
        {
            public Vector2 TopLeft, TopRight, BottomLeft, BottomRight;
            public int HorizontalRayCount;
            public int VerticalRayCount;
            public float HorizontalRaySpacing;
            public float VerticalRaySpacing;
        }

        protected override void OnUpdate()
        {
            foreach( CollisionEntityFilter entity in GetEntities<CollisionEntityFilter>() )
            {
                Velocity velocity = entity.VelocityComponent;
                CollisionData collisionData = entity.CollisionComponent;
                BoxCollider2D collider = entity.ColliderComponent;

                collisionData.previousSlopeAngle = collisionData.slopeAngle;
                ResetCollisionState( collisionData );

                float2 delta =  velocity.Delta;

                RaycastData raycastData = CalculateRaycastData( collider, collisionData );

                if( delta.y < 0 )
                    HandleSlopeDecend( ref delta, collider.edgeRadius, collisionData, raycastData );

                if( delta.x != 0 )
                    HandleHorizontalCollisions( ref delta, collider.edgeRadius, collisionData, raycastData  );

                if( delta.y != 0 )
                    HandleVerticalCollision( ref delta, collider.edgeRadius, collisionData, raycastData  );

                if( collisionData.Below )
                    velocity.Value.y = 0;

                velocity.Delta = delta;
            }
        }

        private void HandleHorizontalCollisions( ref float2 movementDelta, float skinWidth, CollisionData collisionData, RaycastData raycastData )
        {
            float horizontalDirection = math.sign( movementDelta.x );
            float raycastDistance = math.max( math.abs( movementDelta.x ) + skinWidth, 2 * skinWidth );
            Vector2 raycastOrigin = ( horizontalDirection == 1 ) ? raycastData.BottomRight : raycastData.BottomLeft;

            for( int i = 0; i < raycastData.HorizontalRayCount; i++ )
            {
                Vector2 ray = raycastOrigin + ( Vector2.up * ( raycastData.HorizontalRaySpacing * i ) );

                Debug.DrawRay( ray, Vector2.right * horizontalDirection * raycastDistance, Color.red, 0.1f );

                RaycastHit2D hit = Physics2D.Raycast( ray, Vector2.right * horizontalDirection, raycastDistance, collisionData.Mask );

                if( hit )
                {
                    if( hit.distance == 0 )
                        continue;
                    
                    float slopeAngle = Vector2.Angle( hit.normal, Vector2.up );

                    if( i == 0 && slopeAngle <= collisionData.MaxSlopeAngle )
                    {
                        if( collisionData.decendingSlope )
                            collisionData.decendingSlope = false;

                        float distanceToSlope = 0;

                        if( slopeAngle != collisionData.previousSlopeAngle )
                        {
                            distanceToSlope = hit.distance - skinWidth;
                            movementDelta.x -= distanceToSlope * horizontalDirection;
                        }

                        HandleSlopeAscend( ref movementDelta, slopeAngle, collisionData );

                        movementDelta.x += distanceToSlope * horizontalDirection;
                    }

                    if( !collisionData.ascendingSlope || slopeAngle > collisionData.MaxSlopeAngle )
                    {
                        movementDelta.x = ( hit.distance - skinWidth ) * horizontalDirection;
                        raycastDistance = hit.distance;

                        if( collisionData.ascendingSlope )
                            movementDelta.y = math.tan( math.radians( collisionData.slopeAngle ) ) * math.abs( movementDelta.x );

                        collisionData.Right = ( horizontalDirection == 1 );
                        collisionData.Left = ( horizontalDirection == -1 );
                    }
                }
            }
        }

        private void HandleVerticalCollision( ref float2 movementDelta, float skinWidth, CollisionData collisionData, RaycastData raycastData )
        {
            float verticalDirection = math.sign( movementDelta.y );
            float raycastDistance = math.abs( movementDelta.y ) + skinWidth;
            Vector2 raycastOrigin = ( verticalDirection == 1 ) ? raycastData.TopLeft : raycastData.BottomLeft;
            RaycastHit2D hit;

            for( int i = 0; i < raycastData.VerticalRayCount; i++ )
            {
                Vector2 ray = raycastOrigin + ( Vector2.right * ( raycastData.VerticalRaySpacing * i + movementDelta.x ) );

                Debug.DrawRay( ray, Vector2.up * verticalDirection * raycastDistance, Color.red, 0.1f );

                hit = Physics2D.Raycast( ray, Vector2.up * verticalDirection, raycastDistance, collisionData.Mask );

                if( hit )
                {
                    // ToDo one way playforms / moving platforms

                    movementDelta.y = ( hit.distance - skinWidth ) * verticalDirection;
                    raycastDistance = hit.distance;

                    if( collisionData.ascendingSlope )
                        movementDelta.x = movementDelta.y / math.tan( math.radians( collisionData.slopeAngle ) ) * math.sign( movementDelta.x );

                    collisionData.Above = ( verticalDirection == 1 );
                    collisionData.Below = ( verticalDirection == -1 );
                }
            }

            if( collisionData.ascendingSlope )
            {
                float horizontalDirection = math.sign( movementDelta.x );
                raycastDistance = math.abs( movementDelta.x ) + skinWidth;
                Vector2 ray = ( ( horizontalDirection == 1 ) ? raycastData.BottomRight : raycastData.BottomLeft ) + ( Vector2.up * movementDelta.y );
                hit = Physics2D.Raycast( ray, Vector2.right * horizontalDirection, raycastDistance, collisionData.Mask );

                if( hit )
                {
                    float slopeAngle = Vector2.Angle( hit.normal, Vector2.up );

                    if( slopeAngle != collisionData.slopeAngle )
                    {
                        movementDelta.x = ( hit.distance - skinWidth ) * horizontalDirection;
                        collisionData.slopeAngle = slopeAngle;
                    }
                }
            }
        }

        private void HandleSlopeAscend( ref float2 movementDelta, float slopeAngle, CollisionData collisionData )
        {
            float radianSlopeAngle = math.radians( slopeAngle );
            float moveDistance = math.abs( movementDelta.x );
            float verticalAscendDistance = math.sin( radianSlopeAngle ) * moveDistance;

            if( movementDelta.y <= verticalAscendDistance )
            {
                movementDelta.x = math.cos( radianSlopeAngle ) * moveDistance * math.sign( movementDelta.x );
                movementDelta.y = verticalAscendDistance;
                collisionData.Below = true;
                collisionData.ascendingSlope = true;
                collisionData.slopeAngle = slopeAngle;
            }

        }

        private void HandleSlopeDecend( ref float2 movementDelta, float skinWidth, CollisionData collisionData, RaycastData raycastData  )
        {
            float horizontalDirection = math.sign( movementDelta.x );
            Vector2 raycastOrigin = ( horizontalDirection == 1 ) ? raycastData.BottomLeft : raycastData.BottomRight;

            RaycastHit2D hit = Physics2D.Raycast( raycastOrigin, Vector2.down, Mathf.Infinity, collisionData.Mask );

            if( hit )
            {
                float slopeAngle =  Vector2.Angle( hit.normal, Vector2.up );
                float radianSlopeAngle = math.radians( slopeAngle );

                if( slopeAngle != 0 && slopeAngle <= collisionData.MaxSlopeAngle )
                {
                    if( math.sign( hit.normal.x ) == horizontalDirection )
                    {
                        float moveDistance = math.abs( movementDelta.x );
                        if( hit.distance - skinWidth <= math.tan( radianSlopeAngle ) * moveDistance )
                        {
                            movementDelta.x = math.cos( radianSlopeAngle ) * moveDistance * horizontalDirection;
                            movementDelta.y -= math.sin( radianSlopeAngle ) * moveDistance;

                            collisionData.slopeAngle = slopeAngle;
                            collisionData.decendingSlope = true;
                            collisionData.Below = true;
                        }
                    }
                }
            }
        }

        private void ResetCollisionState( CollisionData collisionData )
        {
            collisionData.Above = collisionData.Below = collisionData.Left = collisionData.Right = false;
            collisionData.ascendingSlope = false;
            collisionData.decendingSlope = false;
            collisionData.slopeAngle = 0;
        }

        private RaycastData CalculateRaycastData( BoxCollider2D collider, CollisionData collisionData )
        {
            RaycastData data = new RaycastData();

            float skinWidth = collider.edgeRadius;
            Bounds bounds = collider.bounds;
            bounds.Expand( skinWidth * -2f );

            data.TopLeft = new Vector2( bounds.min.x, bounds.max.y );
            data.TopRight = bounds.max;
            data.BottomLeft = bounds.min;
            data.BottomRight = new Vector2( bounds.max.x, bounds.min.y );

            int horizontalRayCount = collisionData.HorizontalRayCount;
            int verticalRayCount = collisionData.VerticalRayCount;

            float horizontalSpacing = bounds.size.y / ( horizontalRayCount - 1 );
            float verticalSpacing = bounds.size.x / ( verticalRayCount - 1 );

            data.HorizontalRayCount = horizontalRayCount;
            data.VerticalRayCount = verticalRayCount;
            data.HorizontalRaySpacing = horizontalSpacing;
            data.VerticalRaySpacing = verticalSpacing;

            return data;
        }
    }
}