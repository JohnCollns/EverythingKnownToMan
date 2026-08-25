using Godot;
using System;

public partial class PlayerCharacter : CharacterBody2D
{
    public static PlayerInventory Inventory {get; set;}
    public static PlayerCharacter Instance {get; set;}
    
    [Export] private Sprite2D StandardSprite;
    [Export] private Sprite2D HoldSprite;
    private Sprite2D ForegroundArmSprite;
    //[Export] private Node InventoryArticleScene;
    [Export] private Control InventoryArticle;
    //private Control InventoryArticle;
    private Vector2 InitialInvArticlePosition;
    private float InitialInvArticleRotation;

    [Export] private float GravityScale = 200f;
    [Export] private float MovementSpeed = 200f;

    public override void _Ready()
    {
        Instance = this;
        ForegroundArmSprite = HoldSprite.GetChild<Sprite2D>(0);
        //InventoryArticle = InventoryArticleScene.GetChild<Control>(0);
        InitialInvArticlePosition = InventoryArticle.Position;
        InitialInvArticleRotation = InventoryArticle.Rotation;
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
        //InventoryArticle.Position = InventoryArticle.Position * -Vector2.Right;
        //InventoryArticle.Rotation = InventoryArticle.Rotation * -1f;
        InventoryArticle.Position = InitialInvArticlePosition * (Vector2.Right * (bFacingLeft ? -1f : 1f));
        //InventoryArticle.Rotation = InitialInvArticleRotation + (bFacingLeft ? 180f : 0f);
        GD.Print($"InvArticle pos: {InventoryArticle.Position}");
    }
}
