namespace LTS_ToolKit.Controls
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Unity.Entities;
    using Rewired;

    public class InputSystem : ComponentSystem
    {
        private struct InputEntityFilter
        {
            public Input InputComponent;
        }

        private IList<InputAction> actions;

        protected override void OnStartRunning()
        {
            actions = ReInput.mapping.Actions;

            foreach( InputEntityFilter entity in GetEntities<InputEntityFilter>() )
            {
                Input input = entity.InputComponent;

                InitializeActions( input );
            }
        }

        protected override void OnUpdate()
        {
            foreach( InputEntityFilter entity in GetEntities<InputEntityFilter>() )
            {
                Input input = entity.InputComponent;
                Player rewiredPlayer = ( input.Id != 0 ) ? ReInput.players.GetPlayer( input.Id ) : ReInput.players.GetSystemPlayer();

                for ( int i = 0; i < Mathf.Max( input.Axis.Count, input.Buttons.Count ); i++ )
                {
                    if( i < input.Axis.Count )
                    {
                        KeyValuePair<string, float> item = input.Axis.ElementAt( i );
                        input.Axis[ item.Key ] = rewiredPlayer.GetAxis( item.Key );
                    }

                    if( i < input.Buttons.Count )
                    {
                        KeyValuePair<string, bool> item = input.Buttons.ElementAt( i );
                        input.Axis[ item.Key ] = rewiredPlayer.GetAxis( item.Key );
                    }
                }
            }
        }

        private void InitializeActions( Input input )
        {
            input.Axis.Clear();
            input.Buttons.Clear();

            foreach( InputAction action in actions )
            {
                switch( action.type )
                {
                    case InputActionType.Axis:
                        input.Axis.Add( action.name, 0 );
                        break;
                    
                    case InputActionType.Button:
                        input.Buttons.Add( action.name, false );
                        break;
                }
            }
        }
    }
}