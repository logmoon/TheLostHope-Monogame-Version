﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheLostHope.GameCode.GameStates;
using TheLostHope.Engine.ContentManagement;
using TheLostHopeEngine.EngineCode.StateManagement;
using TheLostHopeEngine.EngineCode.UI;
using TheLostHopeEngine.EngineCode.Localization;
using TheLostHope.GameCode.GameStates.SubStates;
using TheLostHope.GameCode.ContentLoading;
using TheLostHopeEngine.EngineCode.Inputs;

namespace TheLostHope
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private static SpriteBatch _spriteBatch;
        private StateManager _stateManager;
        private UIManager _uiManager;

        #region Properties
        public bool IsPaused;
        public GraphicsDeviceManager GraphicsManager { get { return _graphics; } }
        public static SpriteBatch SpriteBatch { get { return _spriteBatch; } }
        public UIManager UIManager { get { return _uiManager; } }
        #endregion

        #region Game States
        public MainMenuState MainMenuState { get; private set; }
        #endregion

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void UnloadContent()
        {
            //if (_settings.IsFullscreen)
            //{
            //    SaveWindow();
            //}

            //Settings.SaveJson<Settings>("Settings.json", _settings, SettingsContext.Default.Settings);

            base.UnloadContent();
        }

        protected override void Initialize()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // --- Load all global things here ---
            // Initialize content loader
            ContentLoader.Initialize(Content);
            // Add a font
            ContentLoader.AddFont("PeaberryMono.ttf");
            // LDtk File
            ContentLoader.LoadLDtkFile("World");
            // Game Assets
            GameAssetsLoader.Initialize();
            // Init the localization system
            LocalizationSystem.Initialize();
            // Init the input system
            InputSystem.Instance.Initialize(GameAssetsLoader.InputAsset);

            IsPaused = false;

            // Window properties
            Window.AllowUserResizing = true;
            IsMouseVisible = true;

            // Time step
            //IsFixedTimeStep = _settings.IsFixedTimeStep;
            IsFixedTimeStep = false;

            //_settings.IsFullscreen = _settings.IsFullscreen || _settings    .IsBorderless;
            _graphics.SynchronizeWithVerticalRetrace = false;

            Window.Title = "The Lost Hope";

            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();

            // RestoreWindow();
            //if (_settings.IsFullscreen)
            //{
            //    ApplyFullscreenChange(false);
            //}

            // Initialize Game State Manager
            _stateManager = new StateManager(this);
            // Initialize UI Manager
            _uiManager = new UIManager();

            // Add Components
            Components.Add(_stateManager);

            // Initilize game states here
            MainMenuState = new MainMenuState(this, _stateManager);

            // Set start game state
            _stateManager.SetState(MainMenuState);

            base.Initialize();
        }

        //public void ToggleFullscreen()
        //{
        //    bool oldIsFullscreen = _settings.IsFullscreen;

        //    if (_settings.IsBorderless)
        //    {
        //        _settings.IsBorderless = false;
        //    }
        //    else
        //    {
        //        _settings.IsFullscreen = !_settings.IsFullscreen;
        //    }

        //    ApplyFullscreenChange(oldIsFullscreen);
        //}
        //public void ToggleBorderless()
        //{
        //    bool oldIsFullscreen =  _settings.IsFullscreen;

        //    _settings.IsBorderless = !_settings.IsBorderless;
        //    _settings.IsFullscreen = _settings.IsBorderless;

        //    ApplyFullscreenChange(oldIsFullscreen);
        //}
        //private void ApplyFullscreenChange(bool oldIsFullscreen)
        //{
        //    if (_settings.IsFullscreen)
        //    {
        //        if (oldIsFullscreen)
        //        {
        //            ApplyHardwareMode();
        //        }
        //        else
        //        {
        //            SetFullscreen();
        //        }
        //    }
        //    else
        //    {
        //        UnsetFullscreen();
        //    }
        //}
        //private void ApplyHardwareMode()
        //{
        //    _graphics.HardwareModeSwitch = !_settings.IsBorderless;
        //    _graphics.ApplyChanges();
        //}
        //private void SetFullscreen()
        //{
        //    SaveWindow();

        //    _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        //    _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        //    _graphics.HardwareModeSwitch = !_settings.IsBorderless;

        //    _graphics.IsFullScreen = true;
        //    _graphics.ApplyChanges();
        //}
        //private void UnsetFullscreen()
        //{
        //    _graphics.IsFullScreen = false;
        //    RestoreWindow();
        //}
        //private void SaveWindow()
        //{
        //    _settings.X = Window.ClientBounds.X;
        //    _settings.Y = Window.ClientBounds.Y;
        //    _settings.Width = Window.ClientBounds.Width;
        //    _settings.Height = Window.ClientBounds.Height;
        //}
        //private void RestoreWindow()
        //{
        //    Window.Position = new Point(_settings.X, _settings.Y);
        //    _graphics.PreferredBackBufferWidth = _settings.Width;
        //    _graphics.PreferredBackBufferHeight = _settings.Height;
        //    _graphics.ApplyChanges();
        //}

        protected override void Update(GameTime gameTime)
        {
            // Update the input system
            InputSystem.Instance.Update();

            GameplayManager.Instance.Update(gameTime);
            _uiManager.Update(gameTime);

            if (!IsPaused)
            {
                base.Update(gameTime);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            // Choose any other color you want to clear the screen to
            GraphicsDevice.Clear(Color.Black);

            // Draw Game
            base.Draw(gameTime);

            // Draw UI
            _uiManager.Draw(_spriteBatch);
        }
    }
}