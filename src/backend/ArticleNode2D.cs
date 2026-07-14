using Godot;
using System;

public partial class ArticleNode2D : Node2D
{
    [Export] public ArticleNode ArticleNode { get; set; }
    [Export] public bool LoadFromDisk = false;
    [Export] public string FileToLoad = "";

    public override void _Ready()
    {
        base._Ready();
        if (LoadFromDisk)
        {
            ArticleNode.LoadFromDisk(FileToLoad);
        }
    }
}
