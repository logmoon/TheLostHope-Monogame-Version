// This file was automatically generated, any modifications will be lost!
#pragma warning disable
namespace LDtkTypes;

using LDtk;

using Microsoft.Xna.Framework;

public class LDtkPlayer : ILDtkEntity
{
    public string Identifier { get; set; }
    public System.Guid Iid { get; set; }
    public int Uid { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }
    public Vector2 Pivot { get; set; }
    public Rectangle Tile { get; set; }

    public Color SmartColor { get; set; }

    public string AsepriteFileName { get; set; }
    public float GravityScale { get; set; }
    public float MaxGravityVelocity { get; set; }
    public float GroundSpeed { get; set; }
    public float AirSpeed { get; set; }
    public float Acceleration { get; set; }
    public float Deacceleration { get; set; }
    public float JumpForce { get; set; }
    public float JumpForceCutOnJumpRelease { get; set; }
    public float JumpBufferTime { get; set; }
    public float RollBufferTime { get; set; }
    public float JumpCayoteTime { get; set; }
    public float RollVelocity { get; set; }
}
#pragma warning restore
