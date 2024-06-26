﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Humper;
using Humper.Responses;
using System;
using System.Linq;
using MonoGame.Aseprite;
using TheLostHope.GameCode.Objects;
using TheLostHope.GameCode.ObjectStateMachine;
using System.Reflection.Metadata.Ecma335;
using System.Diagnostics;

namespace TheLostHope.GameCode.Characters.FSM
{
    // Base character class for all the game character aka player, enemies, npcs
    public abstract class Character : StatefullObject
    {
        // public getter for the collider
        public IBox Body { get { return _body; }}
        public int FacingDirection { get { return _facingDirection; } }
        public bool IFrame { get; set; }
        public IMovement CurrentMovement { get; private protected set; }

        // Colliders
        protected IBox _body;
        // The physics world
        protected World _physicsWorld;

        protected int _facingDirection;
        protected int _currentHealth;

        private float _iFrameTimer;
        private Vector2 velWorkspace;

        // Every child has to implement these functions
        public abstract Func<ICollision, CollisionResponses> GetCollisionFilter();
        // Required funtion that creates the collider, when inheriting from this class,
        public abstract CollisionTags GetCollisionTag();
        public abstract Vector2 GetBoxSize();
        public abstract int GetMaxHealth();
        public abstract float GetIFrameTimer();
        protected abstract void OnTakeDamage();
        protected abstract void OnHealthChanged();
        protected abstract void OnDeath();

        // Constructor
        public Character(Game game, AsepriteFile asepriteFile) : base(game, asepriteFile)
        {
            IFrame = false;

            _facingDirection = 1; // Right

            _iFrameTimer = -1f;

            _currentHealth = GetMaxHealth();
        }

        public virtual void SpawnCharacter(Vector2 position, World physicsWorld)
        {
            _physicsWorld = physicsWorld;

            _body = physicsWorld.Create(position.X + 1f, position.Y + 1f, GetBoxSize().X - 1f, GetBoxSize().Y - 1f)
                .AddTags(GetCollisionTag());
            _body.Data = this;

            //_ceilingCheckBody = physicsWorld.Create(_body.X + _body.Width/4, _body.Y - 1f, _body.Width - (_body.Width/4)*2f, 1f)
            //    .AddTags(CollisionTags.TriggerCheck);

            SpawnObject(position, Vector2.Zero);

            _currentHealth = GetMaxHealth();
        }
        public virtual void DespawnCharacter()
        {
            _physicsWorld.Remove(_body);
            Enabled = false;
            Visible = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Timers
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _iFrameTimer -= delta;

            // Update the animator
            Animator.Update(gameTime);

            // Update the state machine
            StateMachine.CurrentState.Update(delta);

            // Update Physics
            CurrentMovement = MovementStep(delta);

            // Reset y velocity whenever character touches ceiling
            if (IsTouchingCeiling())
            {
                SetVelocityY(0);
            }
        }
        
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (Animator.SpriteSheetTexture == null) return;

            // Draw using the position, scale, rotation, texture and animation
            Game1.SpriteBatch.Draw(Animator.SpriteSheetTexture, Position, Animator.GetSourceRectangle(),
                IFrame ? Color.DodgerBlue : _iFrameTimer > 0 ? Color.Black : Color.White, Rotation,
                Vector2.Zero, Scale, _facingDirection == 1 ? SpriteEffects.None :
                SpriteEffects.FlipHorizontally, 0);
        }

        #region Health
        public int GetCurrentHealth() => _currentHealth;
        public virtual void TakeDamage(int damage, Vector2 knockback = default)
        {
            if (_iFrameTimer > 0 || IFrame) return;

            _iFrameTimer = GetIFrameTimer();
            SetCurrentHealth(_currentHealth - damage);
            OnTakeDamage();

            // Knockback
            SetVelocity(knockback);

            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                OnDeath();
            }
        }
        public virtual void SetCurrentHealth(int health)
        {
            if (health > GetMaxHealth())
            {
                _currentHealth = GetMaxHealth();
            }
            else
            {
                _currentHealth = health;
            }

            OnHealthChanged();
        }
        #endregion

        #region Movement
        public bool IsMoving()
        {
            return Math.Abs(Velocity.X) > 10f;
        }
        public void SetVelocity(float velocity, Vector2 angle, int direction)
        {
            angle.Normalize();
            velWorkspace = new Vector2(angle.X * velocity * direction, angle.Y * velocity);
            SetFinalVelocity();
        }
        public void SetVelocity(float velocity, Vector2 direction)
        {
            velWorkspace = direction * velocity;
            SetFinalVelocity();
        }
        public void SetVelocity(Vector2 velocity)
        {
            velWorkspace = velocity;
            SetFinalVelocity();
        }
        public virtual void SetVelocityX(float velocity)
        {
            velWorkspace = new Vector2(velocity, Velocity.Y);
            SetFinalVelocity();
        }
        public virtual void SetVelocityY(float velocity)
        {
            velWorkspace = new Vector2(Velocity.X, velocity);
            SetFinalVelocity();
        }
        private void SetFinalVelocity()
        {
            Velocity = velWorkspace;
        }
        public void MoveX(float delta, int xInput, float speed, float accel, float deaccel, float velocityPower)
        {
            float targetSpeedX = xInput * MathHelper.Max(speed, 0f);

            float speedDifX = targetSpeedX - Velocity.X;

            float accelRateX = (Math.Abs(targetSpeedX) > 0.01f) ? accel : deaccel;

            float movementX = (float)Math.Pow(Math.Abs(speedDifX) * accelRateX, velocityPower) * Math.Sign(speedDifX);

            Velocity = new Vector2(Velocity.X + (delta * movementX),
                        Velocity.Y);

            // Set facing direction
            if (Math.Abs(Velocity.X) > 0f && Math.Sign(Velocity.X) != Math.Sign(_facingDirection))
                _facingDirection *= -1;
        }
        public void MoveY(float delta, int yInput, float speed, float accel, float deaccel, float velocityPower)
        {
            float targetSpeedY = yInput * MathHelper.Max(speed, 0f);

            float speedDifY = targetSpeedY - Velocity.Y;

            float accelRateY = (Math.Abs(targetSpeedY) > 0.01f) ? accel : deaccel;

            float movementY = (float)Math.Pow(Math.Abs(speedDifY) * accelRateY, velocityPower) * Math.Sign(speedDifY);

            Velocity = new Vector2(Velocity.X,
                        Velocity.Y + (delta * movementY));
        }

        /// <summary>
        /// This moves the character body and sets it's position, should be called after all velocity operations are done.
        /// </summary>
        public IMovement MovementStep(float delta)
        {
            var movement = _body.Move(_body.X + delta * Velocity.X,
                _body.Y + delta * Velocity.Y, GetCollisionFilter());

            //_ceilingCheckMovement = _ceilingCheckBody.Move(_body.X + _body.Width/4, _body.Y - 2f,
            //    (collision) => { return CollisionResponses.None; });

            Debug.WriteLine(Velocity.Y);

            Position = new Vector2(_body.X, _body.Y);

            return movement;
        }
        #endregion

        #region Checks
        public bool IsGrounded()
        {
            if (CurrentMovement == null) return false;

            return CurrentMovement.Hits.Any((c) => c.Box.HasTag(CollisionTags.Ground) &&
                                 (c.Normal.Y < 0));
        }
        public bool IsTouchingCeiling()
        {
            if (CurrentMovement == null) return false;

            return CurrentMovement.Hits.Any((c) => c.Box.HasTag(CollisionTags.Ground) &&
                                 (c.Normal.Y > 0));
        }
        public bool IsTouchingRightWall()
        {
            if (CurrentMovement == null) return false;

            return CurrentMovement.Hits.Any((c) => c.Box.HasTag(CollisionTags.Ground) &&
                                 (c.Normal.X > 0));
        }
        public bool IsTouchingLeftWall()
        {
            if (CurrentMovement == null) return false;

            return CurrentMovement.Hits.Any((c) => c.Box.HasTag(CollisionTags.Ground) &&
                                 (c.Normal.X < 0));
        }
        public bool IsTouchingWall()
        {
            return IsTouchingRightWall() || IsTouchingLeftWall();
        }
        #endregion
    }
}
