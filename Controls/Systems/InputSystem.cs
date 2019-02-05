namespace LTS_ToolKit.Controls
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Unity.Entities;
    using Rewired;

    public class InputSystem : ComponentSystem
    {
        private struct AxisInputEntityFilter
        {
            public AxisInput InputComponent;
        }

        private struct ButtonInputEntityFilter
        {
            public ButtonInput InputComponent;
        }

        private IList<InputAction> actions;

        protected override void OnStartRunning()
        {
            actions = ReInput.mapping.Actions;
        }

        protected override void OnUpdate()
        {
            foreach( InputAction action in actions )
            {
                switch( action.type )
                {
                    case InputActionType.Axis:
                        foreach( AxisInputEntityFilter entity in GetEntities<AxisInputEntityFilter>() )
                        {
                            AxisInput input = entity.InputComponent;
                            Player rewiredPlayer = ( input.Id != 0 ) ? ReInput.players.GetPlayer( input.Id ) : ReInput.players.GetSystemPlayer();
                            if( input.Names.Contains( action.name ) )
                            {
                                if( input.Values == null )
                                    input.Values = new Dictionary<string, float>();

                                float axis = rewiredPlayer.GetAxis( action.name );

                                if( input.Values.ContainsKey( action.name ) )
                                    input.Values[ action.name ] = axis;
                                else
                                    input.Values.Add( action.name, axis );
                            }
                        }
                        break;
                    
                    case InputActionType.Button:
                        foreach( ButtonInputEntityFilter entity in GetEntities<ButtonInputEntityFilter>() )
                        {
                            ButtonInput input = entity.InputComponent;
                            Player rewiredPlayer = ( input.Id != 0 ) ? ReInput.players.GetPlayer( input.Id ) : ReInput.players.GetSystemPlayer();

                            if( input.Values == null )
                                    input.Values = new Dictionary<string, bool>();

                            if( input.Names.Contains( action.name ) )
                            {
                                bool button = rewiredPlayer.GetButton( action.name );
                                if( input.Values.ContainsKey( action.name ) )
                                    input.Values[ action.name ] = button;
                                else
                                    input.Values.Add( action.name, button );
                            }
                        }
                        break;
                }
            }
        }
    }
}