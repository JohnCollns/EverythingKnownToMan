using System;
using System.Collections.Generic;
using System.IO;
using Godot;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace EverythingKnownToMan.backend;

public class WikiArticleFields
{
    public string Title;
    public string Description;
    public string Paragraph;
    
    public WikiArticleFields() {}

    public WikiArticleFields(string title_, string description_, string paragraph_)
    {
        Title = title_;
        Description = description_;
        Paragraph = paragraph_;
    }
}

public class WikiArticle
{
    public string Title;
    public string Description;
    public string Paragraph;
    //public byte[] Image;
    [JsonIgnore]
    public WikiImage Image;
    public HashSet<StringName> Tags;
    
    public static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
        IncludeFields = true
    };

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

    public string ToJSON()
    {
        //return JsonSerializer.Serialize(this, SerializerOptions);
        JsonNode node = JsonSerializer.SerializeToNode(this, SerializerOptions);
        if (Image != null && Image.IsValid())
        {
            //node["image"] = Image.GetSavePath(Title);
            node["image"] = Path.Combine("images", (Title + "." + Image.FileFormat));
        }
        return JsonSerializer.Serialize(node, SerializerOptions);
    }

    public string SaveToDisk()
    {
        string exeDir = AppContext.BaseDirectory;
        // Returns:
        // E:\Godot\GameAWeek26\EverythingKnownToMan\Godot\everything-known-to-man\.godot\mono\temp\bin\Debug\
        // Target:
        // E:\Godot\GameAWeek26\EverythingKnownToMan\Godot\everything-known-to-man\generated\images\
        
        string projectRoot = Path.GetFullPath(Path.Combine(exeDir, "..", "..", "..", "..", ".."));
        string path = Path.Combine(projectRoot, "generated", (Title + ".JSON"));
        GD.Print(" WikiArticle saving to: " + path);
        File.WriteAllText(path, ToJSON());
        Image.SaveToDisk(Title);
        return path;
    }
}