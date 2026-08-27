using Godot;
using System;

public partial class ArticleNode2D : Node2D
{
    [Export] public ArticleNode ArticleNode { get; set; }
    [Export] public bool LoadFromDisk = false;
    [Export] public string FileToLoad = "";

    [Export] private float BobPeriod;
    [Export] private float BobAmplitude;
    [Export] private Vector2 PeriodRandomRange;
    private float PeriodSeed;
    private float PeriodOffset;
    private Vector2 CurrentBob;
    public Vector2 InitPosition;

    public override void _Ready()
    {
        base._Ready();
        if (LoadFromDisk)
        {
            ArticleNode.LoadFromDisk(FileToLoad);
        }
        
        InitPosition = Position;
        //InitPosition = GlobalPosition;
        PeriodSeed = (float)GD.RandRange(PeriodRandomRange.X, PeriodRandomRange.Y);
        PeriodOffset = (float)GD.RandRange(0f, Mathf.Pi);
        CurrentBob = new Vector2();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        CurrentBob.Y = BobAmplitude * Mathf.Cos(Mathf.Pi * 2f * PeriodSeed / BobPeriod * ((float)Time.GetTicksMsec()/1000f + PeriodOffset));
        Position = InitPosition + CurrentBob;
    }
}
