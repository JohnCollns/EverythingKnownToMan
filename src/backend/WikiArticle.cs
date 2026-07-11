using System.Collections.Generic;
using Godot;
using System.Net.Http.Headers;
using System.Text.Json;

namespace EverythingKnownToMan.backend;

public class WikiArticle
{
    public string Title;
    public string Description;
    public string Paragraph;
    //public byte[] Image;
    public WikiImage Image;
    public HashSet<StringName> Tags;

    public WikiArticle() {}
    public WikiArticle(string title_, string paragraph_, byte[] image_) {}
    public WikiArticle(string title_, string paragraph_, WikiImage image_) {}

    public WikiArticle(string title_, string description_, string paragraph_, WikiImage image_)
    {
        Title = title_;
        Description = description_;
        Paragraph = paragraph_;
        Image = image_;
    }
    public WikiArticle(string title_, string paragraph_, byte[] image_, HashSet<StringName> tags) {}
    public WikiArticle(string title_, string paragraph_, WikiImage image_, HashSet<StringName> tags) {}
    public WikiArticle(string title_, string description_, string paragraph_, WikiImage image_, HashSet<StringName> tags) {}

    public bool HasTag(StringName tag) => Tags.Contains(tag);
    
    // Functions
    // To JSON
    // From JSON
    // Save to Disk (JSON + Image)
    // Load from Disk (JSON + Image)
}