﻿using LostHope.Engine.Input;
using LostHope.GameCode;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostHope.Engine.UI
{
    // This class represents a base for anything that can be selected in the UI.
    // For example a button. Check the Button.cs script for an implementation example.
    // Note that every selectable inherits from DrawableGameComponent.
    public abstract class Selectable : UIElement
    {
        // Is triggered when the mouse enters the selectable's zone.
        public event Action OnEnter;
        // Is triggered when the mouse clicks on the selectable
        public event Action OnClick;
        // Is triggered when the mouse exists the selectable zone
        public event Action OnExit;
        public event Action OnSelect;
        public event Action OnDeselect;

        // Is true when the mouse in inside the selectable's zone, and false otherwise
        protected bool _isFocused;
        // Is true when the mouse was inside the selectable's zone in the previous frame, and false otherwise
        protected bool _wasFocused;

        protected bool _isSelected;
        protected bool _wasSelected;

        // The mouse position
        protected Vector2 _mousePosition;

        public Selectable(UIManager uiManager, UIAnchor anchor = UIAnchor.Center, bool selectOnRegister = true) : base(uiManager, anchor = UIAnchor.Center)
        {
            _manager.RegisterSelectable(this, selectOnRegister);

            _isFocused = false;
            _wasFocused = false;

            _isSelected = selectOnRegister;
            _wasSelected = false;
        }

        // This needs to be implemented in every selectable
        // Returns true if the mouse enters the selectable zone, false otherwise
        public abstract bool IsMouseOnSelectable();

        public virtual void InvokeOnEnter()
        {
            OnEnter?.Invoke();
        }
        public virtual void InvokeOnClick()
        {
            OnClick?.Invoke();
        }
        public virtual void InvokeOnExit()
        {
            OnExit?.Invoke();
        }
        public virtual void InvokeOnSelect()
        {
            if (_isSelected) return;

            OnSelect?.Invoke();
        }
        public virtual void InvokeOnDeselect()
        {
            OnDeselect?.Invoke();
        }

        public override void Update(GameTime gameTime)
        {
            _wasFocused = _isFocused;
            _wasSelected = _isSelected;

            // Get mouse position
            _mousePosition = _manager.UIScreenToCanvas(InputManager.MouseState.Position.ToVector2(), _anchor);

            // Set isFocused based on IsMouseOnSelectable
            if (IsMouseOnSelectable())
            {
                _isFocused = true;

                if (InputManager.MousePressed(MouseButton.Left))
                {
                    InvokeOnClick();
                    _isSelected = true;
                }
            }
            else
            {
                _isFocused = false;

                if (InputManager.MousePressed(MouseButton.Left))
                {
                    _isSelected = false;
                }
            }

            // If the selectable wasn't focused in the previous frame and is focused during this frame,
            // That means the mouse just entered the selectable's zone. So we invoke the OnEnter event
            if (!_wasFocused && _isFocused)
            {
                InvokeOnEnter();
            }
            // If the selectable was focused in the previous frame and is not focused during this frame,
            // That means the mouse just exited the selectable's zone. So we invoke the OnExit event
            else if (!_isFocused && _wasFocused)
            {
                InvokeOnExit();
            }

            // Pretty much the same logic as above, but with the selected events
            if (!_wasSelected && _isSelected)
            {
                InvokeOnSelect();
            }
            else if (!_isSelected && _wasSelected)
            {
                InvokeOnDeselect();
            }
        }
    }
}
