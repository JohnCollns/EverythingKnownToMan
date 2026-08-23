using Godot;
using System;

public partial class PlayerCharacter : CharacterBody2D
{
    public static PlayerInventory Inventory {get; set;}
    
    [Export] private Sprite2D StandardSprite;
    [Export] private Sprite2D HoldSprite;

    [Export] private float GravityScale = 200f;
    [Export] private float MovementSpeed = 200f;

    public override void _Ready()
    {
        SetHolding(false);
    }

    public override void _PhysicsProcess(double delta)
    {
        GD.Print(Velocity);
        base._PhysicsProcess(delta);
        var velocity = Velocity;
        velocity.Y += (float)delta * GravityScale;

        if (Input.IsActionPressed("ui_left"))
        {
            velocity.X = -MovementSpeed;
        }
        else if (Input.IsActionPressed("ui_right"))
        {
            velocity.X = MovementSpeed;
        }
        else
        {
            velocity.X = 0;
        }

        Velocity = velocity;

        // "MoveAndSlide" already takes delta time into account.
        MoveAndSlide();
        SetFacingDirection(Velocity);
    }

    private void SetHolding(bool bIsHolding)
    {
        StandardSprite.Visible = !bIsHolding;
        HoldSprite.Visible = bIsHolding;
    }

    private void SetFacingDirection(Vector2 dir)
    {
        if (Mathf.Abs(dir.X) < 0.5f)
            return;
        bool bFacingLeft = dir.X < 0f;
        StandardSprite.FlipH = bFacingLeft;
        HoldSprite.FlipH = bFacingLeft;
    }
}
