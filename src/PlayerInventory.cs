using Godot;
using System;
using EverythingKnownToMan.backend;

public partial class PlayerInventory : Control
{
    public WikiArticle heldArticle;
    [Export] public ArticleNode articleNode;
    public override void _Ready()
    {
        base._Ready();
        PlayerCharacter.Inventory = this;
        
        articleNode.Hide();
    }
    
    // param: article to give the inventory
    // returns the article from the inventory
    public WikiArticle SwapArticle(WikiArticle givenArticle)
    {
        //GD.Print($"Inventory taking: {givenArticle.Title}, losing: {heldArticle.Title}");
        WikiArticle temp = heldArticle;
        heldArticle = givenArticle;
        articleNode.LoadArticle(givenArticle);
        articleNode.Show();
        return temp;
    }
    
    // param: article to give the inventory
    // returns the article from the inventory
    public void SwapArticle(ArticleNode givenArticle)
    {
        string givenArticleTitle = givenArticle == null ? "None" : givenArticle.WikiArticle.Title;
        string heldArticleTitle = heldArticle == null ? "None" : heldArticle.Title;
        GD.Print($"{Name} taking: {givenArticleTitle}, losing: {heldArticleTitle}");
        WikiArticle temp = heldArticle;
        heldArticle = givenArticle.WikiArticle;
        articleNode.LoadArticle(heldArticle);
        articleNode.Show();
        PlayerCharacter.Instance.SetHolding(heldArticle != null);
        GD.Print($"Inv article has title: {articleNode.WikiArticle.Title}");
        
        givenArticle.LoadArticle(temp);
    }

    public void ConsumeArticle()
    {
        heldArticle = null;
        articleNode.Hide();
        PlayerCharacter.Instance.SetHolding(false);
    }
}
