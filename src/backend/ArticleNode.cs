using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using EverythingKnownToMan.backend;
using System.Net.Http.Headers;
using System.Text.Json;

public partial class ArticleNode : Node
{
    public WikiArticle WikiArticle;
    
    //private Sprite2D articleSprite;
    [Export] private TextureRect ArticleTexture;
    [Export] private Label TitleLabel;
    [Export] private Label DescLabel;
    [Export] private Label ParagraphLabel;
    [Export] private Label TagsLabel;
    [Export] private bool bLoadFromDisk = false;
    [Export] private string DiskFileName;

    public override void _Ready()
    {
        base._Ready();
        if (bLoadFromDisk && !string.IsNullOrEmpty(DiskFileName))
        {
            LoadFromDisk(DiskFileName);
        }
    }

    public void LoadArticle(WikiArticle wikiArticle)
    {
        WikiArticle = wikiArticle;
        if (WikiArticle.Image != null)
        {
            var texture = ImageTexture.CreateFromImage(ImageFromWikiImage(wikiArticle.Image));
            if (texture != null)
                ArticleTexture.Texture = texture;
        }
        else
        {
            ArticleTexture.Texture = null;
        }
        
        TitleLabel.Text = wikiArticle.Title;
        DescLabel.Text = wikiArticle.Description;
        ParagraphLabel.Text = wikiArticle.Paragraph;
        if (WikiArticle.Tags != null)
        {
            TagsLabel.Text = $"Tags: {string.Join(", ", WikiArticle.Tags)}";
        }
        else
        {
            TagsLabel.Text = "No Tags";
        }
    }

    public static Image ImageFromWikiImage(WikiImage wikiImage)
    {
        if (wikiImage == null || !wikiImage.IsValid())
        {
            GD.PushError("Invalid WikiImage");
            return null;
        }

        Image outputImage = new Image();

        Error imageLoadError;
        switch (WikiImageFormats.GetImageFormatFromTag(wikiImage.FileFormat))
        {
            case WikiImageFormats.ImageFormat.jpg:
                imageLoadError = outputImage.LoadJpgFromBuffer(wikiImage.ImageBytes);
                break;
            case WikiImageFormats.ImageFormat.png:
                imageLoadError = outputImage.LoadPngFromBuffer(wikiImage.ImageBytes);
                break;
            case WikiImageFormats.ImageFormat.webp:
                imageLoadError = outputImage.LoadWebpFromBuffer(wikiImage.ImageBytes);
                break;
            default:
                GD.PushError("Invalid WikiImageFormat");
                imageLoadError = Error.InvalidData;
                break;
        }

        if (imageLoadError != Error.Ok)
        {
            GD.PushError("ImageFromWikiImage failed: " + imageLoadError);
            return null;
        }
        
        return outputImage;
    }

    // Returns saved file path
    public string SaveToDisk()
    {
        return WikiArticle.SaveToDisk();
    }

    public bool LoadFromDisk(string title)
    {
        try
        {
            GD.Print("TryLoadArticleFromDisk: " + title);
            // GD.Print($" About to read: {WikiArticle.RelativePathToFullPath($"generated\\{newText}.JSON")}");
            // string json = File.ReadAllText(WikiArticle.RelativePathToFullPath($"generated\\{newText}.JSON"));
            GD.Print($" About to read: {WikiArticle.RelativePathToFullPath($"{title}.JSON", true)}");
            string json = File.ReadAllText(WikiArticle.RelativePathToFullPath($"{title}.JSON", true));
            GD.Print($" read json: {json}");
            LoadArticle(WikiArticle.FromJSON(json));
            return true;
        }
        catch (Exception e)
        {
            GD.PushError(e);
        }
        return false;
    }

    public void AddTag(StringName newTagName)
    {
        GD.Print($"WikiArticle is null: {WikiArticle == null}");
        GD.Print($"WikiArticle.Tags is null: {WikiArticle.Tags == null}");
        if (WikiArticle.Tags == null)
        {
            WikiArticle.Tags = new HashSet<StringName>();
        }
        WikiArticle.Tags.Add(newTagName);
        LoadArticle(WikiArticle);
    }

    public void ClearTags()
    {
        WikiArticle.Tags.Clear();
        LoadArticle(WikiArticle);
    }
}
