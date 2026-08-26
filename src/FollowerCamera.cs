using Godot;
using System;

public partial class FollowerCamera : Node2D
{
    [Export] private Vector2 CameraOffset;
    [Export] private Node2D PlayerChar;

    public override void _Process(double delta)
    {
        base._Process(delta);
        Position =  PlayerChar.Position + CameraOffset;
    }
}
