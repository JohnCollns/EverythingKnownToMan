using Godot;
using System;
using Godot.Collections;

public partial class WikiNPC : Node
{
    [Export] public Sprite2D Sprite;
    [Export] public Button ClickableArea;
    [Export] public string DesiredTag;
    [Export] private Texture2D SatisfiedTexture;
    [Export] private Node[] OwnedBlockers;
    [Export] private Spawner OwnedSpawner;
    public bool bIsSatisfied { get; private set; } = false;

    public override void _Ready()
    {
        base._Ready();
        ClickableArea.Pressed += OnClickableAreaOnPressed;
    }

    private void OnClickableAreaOnPressed()
    {
        GD.Print("ClickableAreaOnPressed");
        if (!bIsSatisfied && PlayerSingleton.Inventory.heldArticle.HasTag(DesiredTag.ToLower()))
        {
            GD.Print("NPC is satisfied");
            PlayerSingleton.Inventory.ConsumeArticle();
            OnSatisfied();
        }
    }

    private void OnSatisfied()
    {
        bIsSatisfied = true;
        foreach (Node blocker in OwnedBlockers)
        {
            ClearBlocker_Recursive(blocker);
        }
        if (OwnedSpawner != null)
        {
            GD.Print("OnSatisfied enabling spawner: "  + OwnedSpawner);
            OwnedSpawner.SpawnArticles();
        }
        if (SatisfiedTexture != null)
        {
            Sprite.Texture = SatisfiedTexture;
        }
    }

    private void ClearBlocker_Recursive(Node node)
    {
        if (node == null) return;
        if (node is CollisionShape2D collisionShape2D)
        {
            collisionShape2D.Disabled = true;
        }
        if (node is Sprite2D sprite)
        {
            sprite.Visible = false;
        }

        Array<Node> children = node.GetChildren();
        foreach (Node child in children)
        {
            ClearBlocker_Recursive(child);
        }
    }
}
