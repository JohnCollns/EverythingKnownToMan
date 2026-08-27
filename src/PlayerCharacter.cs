using Godot;
using System;

public partial class PlayerCharacter : CharacterBody2D
{
    public static PlayerInventory Inventory {get; set;}
    public static PlayerCharacter Instance {get; set;}
    
    [Export] private Sprite2D StandardSprite;
    [Export] private Sprite2D HoldSprite;
    private Sprite2D ForegroundArmSprite;
    [Export] private Control InventoryArticle;
    private Vector2 InvArticlePositionLeft;
    private Vector2 InvArticlePositionRight;
    private float InitialInvArticleRotation;

    [Export] private float GravityScale = 200f;
    [Export] private float MovementSpeed = 200f;

    [Export] private AudioStreamPlayer2D SwapAudio;
    [Export] private AudioStream[] SwapSounds;
    [Export] private AudioStreamPlayer2D FailAudio;

    public override void _Ready()
    {
        Instance = this;
        ForegroundArmSprite = HoldSprite.GetChild<Sprite2D>(0);
        InitialInvArticleRotation = InventoryArticle.Rotation;
        InvArticlePositionRight = new Vector2(InventoryArticle.Position.X, InventoryArticle.Position.Y);
        InvArticlePositionLeft = new Vector2(-InventoryArticle.Position.X, InventoryArticle.Position.Y);
        SetHolding(false);
    }

    public override void _PhysicsProcess(double delta)
    {
        //GD.Print(Velocity);
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

    public void SetHolding(bool bIsHolding)
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
        ForegroundArmSprite.FlipH = bFacingLeft;
        
        InventoryArticle.Position = (bFacingLeft ? InvArticlePositionLeft : InvArticlePositionRight);
        InventoryArticle.Rotation = InitialInvArticleRotation * (bFacingLeft ? -1f : 1f);
    }

    public void PlaySwapAudio()
    {
        SwapAudio.Stream = SwapSounds[GD.RandRange(0, SwapSounds.Length - 1)];
        SwapAudio.Play();
    }

    public void PlayFailAudio()
    {
        FailAudio.Play();
    }
}
