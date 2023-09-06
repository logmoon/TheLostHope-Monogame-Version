﻿using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using LostHope.Engine.Camera;
using LostHope.GameCode.Characters.PlayerCharacter;
using LostHope.GameCode.GameStates;

namespace LostHope.GameCode
{
    public static class Globals
    {
        // Screen width and height
        public const int ScreenWidth = 1280;
        public const int ScreenHeight = 720;

        public static SpriteBatch SpriteBatch { get; set; }
        public static GraphicsDeviceManager GraphicsDeviceManager { get; set; }
        public static GameTime GameTime { get; set; }
    }
}
