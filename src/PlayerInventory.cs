using Godot;
using System;
using EverythingKnownToMan.backend;

public partial class PlayerInventory : Node
{
    public WikiArticle heldArticle;
    [Export] public ArticleNode articleNode;
    public override void _Ready()
    {
        base._Ready();
        PlayerSingleton.Inventory = this;
        
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
        //GD.Print($"Inventory taking: {givenArticle.WikiArticle.Title}, losing: {heldArticle.Title}");
        WikiArticle temp = heldArticle;
        heldArticle = givenArticle.WikiArticle;
        articleNode.LoadArticle(heldArticle);
        articleNode.Show();
        
        givenArticle.LoadArticle(temp);
    }

    public void ConsumeArticle()
    {
        heldArticle = null;
        articleNode.Hide();
    }
}
