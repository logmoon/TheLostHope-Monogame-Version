﻿using TheLostHope.GameCode.Characters.FSM;
using System.Diagnostics;

namespace TheLostHope.GameCode.Characters.PlayerCharacter.States
{
    public class PlayerParryState : PlayerState
    {
        public PlayerParryState(Character character, object animKey) : base(character, animKey)
        {
        }

        public override void Enter()
        {
            base.Enter();

            _player.SetVelocityX(0);
        }

        protected override void AnimationFrameEventTriggered()
        {
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            if (_isAnimationFinished)
            {
                if (!_player.IsGrounded())
                {
                    _stateMachine.ChangeState(_player.PlayerInAirState);
                }
                else
                {
                    if (_player.IsMoving())
                    {
                        _stateMachine.ChangeState(_player.PlayerRunState);
                    }
                    else
                    {
                        _stateMachine.ChangeState(_player.PlayerIdleState);
                    }
                }
            }
        }
    }
}
