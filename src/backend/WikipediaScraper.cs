using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace EverythingKnownToMan.backend;

public class WikipediaScraper
{
    static bool bThumbnailMode = true;
    // Core Functions:
    // Article link -> WikiArticle
    // Article link + Tags -> WikiArticle
    // Generate random article link
    // Wrapper (Random link -> WikiArticle)
    
    // Using System.Net.Http,   alternatively could use Godot.Http. 
    private static readonly HttpClient Http = CreateHttpClient();
    
    private static HttpClient CreateHttpClient()
    {
        var client = new HttpClient();
        // Wikipedia requires a descriptive User-Agent for API requests.
        client.DefaultRequestHeaders.UserAgent.Add(
            new ProductInfoHeaderValue("CSharpWikiDownloader", "1.0"));
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        return client;
    }
    
    /// <summary>
    /// Fetches title, first paragraph, and main image for a Wikipedia article.
    /// </summary>
    public static async Task<WikiArticle> FetchArticleAsync(string title, string language = "en")
    {
        string encodedTitle = Uri.EscapeDataString(title.Replace(' ', '_'));
        string summaryUrl = $"https://{language}.wikipedia.org/api/rest_v1/page/summary/{encodedTitle}";

        using HttpResponseMessage response = await Http.GetAsync(summaryUrl);
        response.EnsureSuccessStatusCode();

        string json = await response.Content.ReadAsStringAsync();
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        
        // TODO: Check if this article is valid, if it has an image.

        string resolvedTitle = root.GetProperty("title").GetString() ?? title;

        // "extract" is the plain-text summary; its first sentence/paragraph
        // is effectively the article's lead paragraph.
        string extract = root.TryGetProperty("extract", out var extractEl)
            ? extractEl.GetString() ?? ""
            : "";
        //string firstParagraph = ExtractFirstParagraph(extract);
        
        string description = root.GetProperty("description").GetString() ?? "";
        
        string preferredImage = bThumbnailMode ? "thumbnail" : "originalimage";
        string fallbackImage = bThumbnailMode ? "originalimage" : "thumbnail";

        // Prefer the higher-resolution "originalimage"; fall back to "thumbnail".
        string? imageUrl = null;
        if (root.TryGetProperty(preferredImage, out var origImg) &&
            origImg.TryGetProperty("source", out var origSrc))
        {
            imageUrl = origSrc.GetString();
        }
        else if (root.TryGetProperty(fallbackImage, out var thumb) &&
                 thumb.TryGetProperty("source", out var thumbSrc))
        {
            imageUrl = thumbSrc.GetString();
        }

        byte[]? imageBytes = null;
        string? contentType = null;
        if (!string.IsNullOrEmpty(imageUrl))
        {
            using HttpResponseMessage imgResponse = await Http.GetAsync(imageUrl);
            if (imgResponse.IsSuccessStatusCode)
            {
                imageBytes = await imgResponse.Content.ReadAsByteArrayAsync();
                contentType = imgResponse.Content.Headers.ContentType?.MediaType;
            }
        }

        var wikiImage = new WikiImage(contentType, imageBytes);
        return new WikiArticle(resolvedTitle, description, extract, wikiImage);

        //return new WikiArticle(resolvedTitle, firstParagraph, imageUrl, imageBytes, contentType);
    }
}
