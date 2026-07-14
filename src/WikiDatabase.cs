using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using EverythingKnownToMan.backend;

public partial class WikiDatabase : Node
{
    public static WikiDatabase Instance;
    public Dictionary<string, List<WikiArticle>> WikiArticles = new Dictionary<string, List<WikiArticle>>();

    public override void _Ready()
    {
        base._Ready();
        LoadArticlesFromDisk();
        Instance = this;
    }

    public WikiArticle RequestRandomArticleOfTag(string tag)
    {
        string lowertag = tag.ToLower();
        if (!WikiArticles.ContainsKey(lowertag))
        {
            GD.PushError($"WikiDatabase contains no articles for tag: {lowertag}");
            return null;
        }
        GD.Print($"Attempting to return article for tag: {lowertag}");// DEBUG DELETE

        return WikiArticles[lowertag][new Random().Next(0, WikiArticles[lowertag].Count)];
    }

    public WikiArticle RequestRandomArticleOfTag(string[] tags)
    {
        string selectedTag = tags[new Random().Next(0, tags.Length)];
        return RequestRandomArticleOfTag(selectedTag);
    }

    private void LoadArticlesFromDisk()
    {
        string searchDir = WikiArticle.RelativePathToFullPath("", true);
        GD.Print($"LoadArticlesFromDisk searchDir: {searchDir}");
        string[] files = Directory.GetFiles(searchDir);
        GD.Print($"LoadArticlesFromDisk found number of files: {files.Length}");
        foreach (string file in files)
        {
            GD.Print($"LoadArticlesFromDisk file: {file}");
            string json = File.ReadAllText(WikiArticle.RelativePathToFullPath($"{file}", true));
            WikiArticle article = WikiArticle.FromJSON(json);
            if (article.Tags != null)
            {
                foreach (StringName tag in article.Tags)
                {
                    if (!WikiArticles.ContainsKey(tag))
                    {
                        WikiArticles.Add(tag, [article]);
                    }
                    else
                    {
                        WikiArticles[tag].Add(article);
                    }
                }
            }
            else
            {
                GD.PushWarning($"Article: {file} has no tags.");
            }
        }
        
        GD.Print($"LoadArticlesFromDisk finished successfully");
    }
}
