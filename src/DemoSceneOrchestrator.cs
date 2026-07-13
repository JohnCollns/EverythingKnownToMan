using Godot;
using System;
using System.IO;
using System.Threading.Tasks;
using EverythingKnownToMan.backend;

public partial class DemoSceneOrchestrator : Node
{
    [Export] private ArticleNode ArticleNode;
    [Export] public string SearchTitle = "flying fish";
    private LineEdit SearchLineEdit;
    private LineEdit FileLineEdit;
    private Button SaveButton;
    private LineEdit TagLineEdit;
    private Button ClearTagsButton;

    public override void _Ready()
    {
        base._Ready();
        //ArticleNode = GetNode<ArticleNode>("ArticleNode");
        SearchLineEdit = GetNode<LineEdit>("SearchLineEdit");
        SearchLineEdit.TextSubmitted += SearchTitleEditOnTextSubmitted;
        FileLineEdit = GetNode<LineEdit>("FileLineEdit");
        FileLineEdit.TextSubmitted += TryLoadArticleFromDisk;
        SaveButton = GetNode<Button>("SaveButton");
        SaveButton.Pressed += OnSaveButtonOnPressed;
        TagLineEdit = GetNode<LineEdit>("TagLineEdit");
        TagLineEdit.TextSubmitted += newText => ArticleNode.AddTag(newText);
        ClearTagsButton =  GetNode<Button>("ClearTagsButton");
        ClearTagsButton.Pressed += () =>  ArticleNode.ClearTags();
        //ClearTagsButton.Pressed += OnClearTagsButtonOnPressed;

        if (!string.IsNullOrEmpty(SearchTitle))
        {
            LoadArticle(SearchTitle, false);
        }
    }

    private void OnTagLineEditOnTextSubmitted(string newText)
    {
        ArticleNode.AddTag(newText);
    }

    private void OnClearTagsButtonOnPressed()
    {
        ArticleNode.ClearTags();
    }

    private void OnSaveButtonOnPressed()
    {
        string path = ArticleNode.SaveToDisk();
        GD.Print($"Saved article to {path}");
    }

    private void SearchTitleEditOnTextSubmitted(string newText)
    {
        SearchTitle = newText;
        LoadArticle(SearchTitle, false);
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

    private void TryLoadArticleFromDisk(string newText)
    {
        GD.Print("TryLoadArticleFromDisk: " + newText);
        // GD.Print($" About to read: {WikiArticle.RelativePathToFullPath($"generated\\{newText}.JSON")}");
        // string json = File.ReadAllText(WikiArticle.RelativePathToFullPath($"generated\\{newText}.JSON"));
        GD.Print($" About to read: {WikiArticle.RelativePathToFullPath($"{newText}.JSON", true)}");
        string json = File.ReadAllText(WikiArticle.RelativePathToFullPath($"{newText}.JSON", true));
        GD.Print($" read json: {json}");
        WikiArticle wikiArticle = WikiArticle.FromJSON(json);
        ArticleNode.LoadArticle(wikiArticle);
    }
}
