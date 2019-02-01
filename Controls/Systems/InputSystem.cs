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

                for( int a = 0; a < input.Axis.Count; a++ )
                {
                    input.Axis[a].Value = rewiredPlayer.GetAxis( input.Axis[a].Name );
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
                        input.Axis.Add( new Input.AxisAction( action.name ) );
                        break;
                    
                    case InputActionType.Button:
                        input.Buttons.Add( new Input.ButtonAction( action.name ) );
                        break;
                }
            }
        }
    }
}