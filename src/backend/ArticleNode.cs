using Godot;
using System;
using EverythingKnownToMan.backend;
using System.Net.Http.Headers;
using System.Text.Json;

public partial class ArticleNode : Node
{
    public WikiArticle WikiArticle;

    public override void _Ready()
    {
        base._Ready();
    }
}
