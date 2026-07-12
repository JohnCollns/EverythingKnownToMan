using Godot;
using System;
using System.Threading.Tasks;
using EverythingKnownToMan.backend;

public partial class DemoSceneOrchestrator : Node
{
    [Export] private ArticleNode ArticleNode;
    [Export] public string SearchTitle = "flying fish";
    private LineEdit SearchLineEdit;

    public override void _Ready()
    {
        base._Ready();
        //ArticleNode = GetNode<ArticleNode>("ArticleNode");
        SearchLineEdit = GetNode<LineEdit>("SearchLineEdit");
        SearchLineEdit.TextSubmitted += SearchTitleEditOnTextSubmitted;

        LoadArticle(SearchTitle, false);
    }

    private void SearchTitleEditOnTextSubmitted(string newText)
    {
        SearchTitle = newText;
        LoadArticle(SearchTitle, true);
    }

    protected async void LoadArticle(string title, bool saveToDisk)
    {
        GD.Print("Orch Load Article Start");
        WikiArticle wikiArticle = await WikipediaScraper.FetchArticleAsync(title);
        //GD.Print("Orch Load Article Loaded");
        GD.Print($"Orch Load Article Loaded, Image format: {wikiArticle.Image.FileFormat}, array len: {wikiArticle.Image.ImageBytes.Length}");
        GD.Print("Orch Load Article Loaded, about to load article");
        ArticleNode.LoadArticle(wikiArticle);
        GD.Print("Orch Load Article Finish");
        if (saveToDisk)
        {
            string path = ArticleNode.SaveToDisk();
            GD.Print($"Saved article to {path}");
        }
    }
}
