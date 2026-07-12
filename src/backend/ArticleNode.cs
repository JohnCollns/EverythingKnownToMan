using Godot;
using System;
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

    public override void _Ready()
    {
        base._Ready();
        // ArticleTexture = GetNode<TextureRect>("ArticleSprite");
        // TitleLabel = GetNode<Label>("TitleLabel");
        // DescLabel = GetNode<Label>("DescLabel");
        // ParagraphLabel = GetNode<Label>("ParagraphLabel");
        // TagsLabel = GetNode<Label>("TagsLabel");
    }

    public void LoadArticle(WikiArticle wikiArticle)
    {
        WikiArticle = wikiArticle;
        var texture = ImageTexture.CreateFromImage(ImageFromWikiImage(wikiArticle.Image));
        if (texture != null)
            ArticleTexture.Texture = texture;
        
        TitleLabel.Text = wikiArticle.Title;
        DescLabel.Text = wikiArticle.Description;
        ParagraphLabel.Text = wikiArticle.Paragraph;
        // TODO TagsLabel.Text = wikiArticle.Title;
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
}
