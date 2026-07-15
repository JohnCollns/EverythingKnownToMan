using Godot;
using System;
using EverythingKnownToMan.backend;

public partial class Spawner : Node
{
    [Export] public string[] tags;
    [Export] public int numArticles;
    [Export] public string articleScenePath;
    private PackedScene articleSceneTemplate;
    [Export] public float maxRadius;
    [Export] public float minRadius;

    public override void _Ready()
    {
        base._Ready();
        articleSceneTemplate = GD.Load<PackedScene>(articleScenePath);
        //SpawnArticles(numArticles);
    }

    public void SpawnArticles(int numArticles_)
    {
        for (int i=0; i<numArticles_; i++)
            SpawnArticle();
    }
    
    public void SpawnArticles() { SpawnArticles(numArticles); }

    public void SpawnArticle()
    {
        WikiArticle article = WikiDatabase.Instance.RequestRandomArticleOfTag(tags);
        if (article == null)
        {
            return;
        }
        ArticleNode2D articleNode2D = articleSceneTemplate.Instantiate<ArticleNode2D>();
        AddChild(articleNode2D);
        articleNode2D.ArticleNode.LoadArticle(article);
        articleNode2D.Position = GetRandomOffset();
    }

    public Vector2 GetRandomOffset()
    {
        float distance = new Random().NextSingle() * (maxRadius - minRadius) + minRadius;
        float angle = new Random().NextSingle() * MathF.PI * 2;
        return new Vector2(MathF.Cos(angle) * distance, MathF.Sin(angle) * distance);
    }
}
