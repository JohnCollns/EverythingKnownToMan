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
	private LineEdit CSVLineEdit;
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
		CSVLineEdit = GetNode<LineEdit>("CSVLineEdit");
		CSVLineEdit.TextSubmitted += OnCSVLineEditOnTextSubmitted;
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

	private void OnCSVLineEditOnTextSubmitted(string fullPath)
	{
		// example: 
		// E:\Godot\GameAWeek26\EverythingKnownToMan\ArticleList.csv
		if (!Path.Exists(fullPath))
		{
			GD.PushError("Specified CSV file does not exist.");
			return;
		}
		if (Path.GetExtension(fullPath) != ".csv")
		{
			GD.PushError("Specified file is not a CSV. Type: " + Path.GetExtension(fullPath));
			return;
		}

		SaveCSVArticlesToDisk(fullPath);
	}

	private async void SaveCSVArticlesToDisk(string path)
	{
		//string csv = File.ReadAllText(fullPath);
		string[] csv = File.ReadAllLines(path);
		foreach (string line in csv)
		{
			GD.Print("Reading line: " + line); // DEBUG DELETE
			string[] columns = line.Split(',');
			if (columns.IsEmpty() || !columns[0].StartsWith("https"))
				continue;
			
			// 0 - URL
			// 1,2,3,etc - tags
			// need to clean out any double quotes left in from export. 
			GD.Print("Requesting article: " + columns[0]); // DEBUG DELETE
			GD.Print("Requesting article: " + (columns[0].Replace("wiki/", "api/rest_v1/page/summary/"))); // DEBUG DELETE
			// GD.Print("Requesting article: " + Uri.EscapeDataString(columns[0].Replace("wiki/", "api/rest_v1/page/summary/"))); // DEBUG DELETE
			WikiArticle wikiArticle = await WikipediaScraper.FetchArticleAsync_URL(columns[0]);
			ArticleNode.LoadArticle(wikiArticle);
			// add tags
			for (int col = 1; col < columns.Length; col++)
			{
				string tag = columns[col].ToLower().Replace("\"", "");
				GD.Print("Adding tag: " + tag); // DEBUG DELETE
				ArticleNode.AddTag(tag);
			}
			ArticleNode.SaveToDisk();
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
		WikiArticle wikiArticle = await WikipediaScraper.FetchArticleAsync_Title(title);
		//GD.Print("Orch Load Article Loaded");
		ArticleNode.LoadArticle(wikiArticle);
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
