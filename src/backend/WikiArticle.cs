using System.Collections.Generic;
using Godot;
using System.Net.Http.Headers;
using System.Text.Json;

namespace EverythingKnownToMan.backend;

public class WikiArticle
{
    public string Title;
    public string Paragraph;
    public byte[] Image;
    public HashSet<StringName> Tags;

    public bool HasTag(StringName tag) => Tags.Contains(tag);
    
    // Functions
    // To JSON
    // From JSON
    // Save to Disk (JSON + Image)
    // Load from Disk (JSON + Image)
}