// namespace EverythingKnownToMan.backend;
//
// // WikipediaDownloader.cs
// //
// // A simple C# script/console app that downloads a Wikipedia article's:
// //   - Title
// //   - First paragraph (summary/extract)
// //   - Main (thumbnail/lead) image
// //
// // It uses Wikipedia's REST "page summary" endpoint, which conveniently
// // returns all three in one call:
// //   https://en.wikipedia.org/api/rest_v1/page/summary/{title}
// //
// // Requirements: .NET 6+ SDK
// // Run with:     dotnet run WikipediaDownloader.cs "Albert Einstein"
// //   (or just `dotnet run` and it will prompt / use a default title)
// //
// // Output: saves a JSON-friendly text file and the image into ./output/<Title>/
//
// using System.Net.Http.Headers;
// using System.Text.Json;
//
// // ---------- Entry point ----------
//
// string articleTitle = args.Length > 0 ? args[0] : PromptForTitle();
//
// try
// {
//     var article = await WikipediaClient.FetchArticleAsync(articleTitle);
//     string savedFolder = await WikipediaClient.SaveToDiskAsync(article);
//
//     Console.WriteLine();
//     Console.WriteLine($"Title:      {article.Title}");
//     Console.WriteLine($"Paragraph:  {Truncate(article.FirstParagraph, 200)}");
//     Console.WriteLine($"Image URL:  {article.ImageUrl ?? "(no image found)"}");
//     Console.WriteLine($"Saved to:   {savedFolder}");
// }
// catch (Exception ex)
// {
//     Console.Error.WriteLine($"Failed to fetch article '{articleTitle}': {ex.Message}");
// }
//
// static string PromptForTitle()
// {
//     Console.Write("Wikipedia article title: ");
//     return Console.ReadLine() ?? "Wikipedia";
// }
//
// static string Truncate(string s, int max) =>
//     string.IsNullOrEmpty(s) || s.Length <= max ? s : s[..max] + "...";
//
// // ---------- Data + client ----------
//
// public record WikipediaArticle(
//     string Title,
//     string FirstParagraph,
//     string? ImageUrl,
//     byte[]? ImageBytes,
//     string? ImageContentType
// );
//
// public static class WikipediaClient
// {
//     private static readonly HttpClient Http = CreateHttpClient();
//
//     private static HttpClient CreateHttpClient()
//     {
//         var client = new HttpClient();
//         // Wikipedia requires a descriptive User-Agent for API requests.
//         client.DefaultRequestHeaders.UserAgent.Add(
//             new ProductInfoHeaderValue("CSharpWikiDownloader", "1.0"));
//         client.DefaultRequestHeaders.Accept.Add(
//             new MediaTypeWithQualityHeaderValue("application/json"));
//         return client;
//     }
//
//     /// <summary>
//     /// Fetches title, first paragraph, and main image for a Wikipedia article.
//     /// </summary>
//     public static async Task<WikipediaArticle> FetchArticleAsync(string title, string language = "en")
//     {
//         string encodedTitle = Uri.EscapeDataString(title.Replace(' ', '_'));
//         string summaryUrl = $"https://{language}.wikipedia.org/api/rest_v1/page/summary/{encodedTitle}";
//
//         using HttpResponseMessage response = await Http.GetAsync(summaryUrl);
//         response.EnsureSuccessStatusCode();
//
//         string json = await response.Content.ReadAsStringAsync();
//         using JsonDocument doc = JsonDocument.Parse(json);
//         JsonElement root = doc.RootElement;
//
//         string resolvedTitle = root.GetProperty("title").GetString() ?? title;
//
//         // "extract" is the plain-text summary; its first sentence/paragraph
//         // is effectively the article's lead paragraph.
//         string extract = root.TryGetProperty("extract", out var extractEl)
//             ? extractEl.GetString() ?? ""
//             : "";
//         string firstParagraph = ExtractFirstParagraph(extract);
//
//         // Prefer the higher-resolution "originalimage"; fall back to "thumbnail".
//         string? imageUrl = null;
//         if (root.TryGetProperty("originalimage", out var origImg) &&
//             origImg.TryGetProperty("source", out var origSrc))
//         {
//             imageUrl = origSrc.GetString();
//         }
//         else if (root.TryGetProperty("thumbnail", out var thumb) &&
//                  thumb.TryGetProperty("source", out var thumbSrc))
//         {
//             imageUrl = thumbSrc.GetString();
//         }
//
//         byte[]? imageBytes = null;
//         string? contentType = null;
//         if (!string.IsNullOrEmpty(imageUrl))
//         {
//             using HttpResponseMessage imgResponse = await Http.GetAsync(imageUrl);
//             if (imgResponse.IsSuccessStatusCode)
//             {
//                 imageBytes = await imgResponse.Content.ReadAsByteArrayAsync();
//                 contentType = imgResponse.Content.Headers.ContentType?.MediaType;
//             }
//         }
//
//         return new WikipediaArticle(resolvedTitle, firstParagraph, imageUrl, imageBytes, contentType);
//     }
//
//     private static string ExtractFirstParagraph(string extract)
//     {
//         if (string.IsNullOrWhiteSpace(extract)) return "";
//         // The REST summary extract is usually already just the lead paragraph,
//         // but split on double newlines in case multiple paragraphs are present.
//         var parts = extract.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);
//         return parts.Length > 0 ? parts[0].Trim() : extract.Trim();
//     }
//
//     /// <summary>
//     /// Saves the article's text (title + paragraph) and image to disk under
//     /// ./output/{Title}/. Returns the folder path used.
//     /// </summary>
//     public static async Task<string> SaveToDiskAsync(WikipediaArticle article, string baseDir = "output")
//     {
//         string safeFolderName = string.Concat(article.Title.Split(Path.GetInvalidFileNameChars()));
//         string folder = Path.Combine(baseDir, safeFolderName);
//         Directory.CreateDirectory(folder);
//
//         string textPath = Path.Combine(folder, "article.txt");
//         await File.WriteAllTextAsync(textPath,
//             $"Title: {article.Title}\n\nFirst paragraph:\n{article.FirstParagraph}\n\nImage URL: {article.ImageUrl}\n");
//
//         if (article.ImageBytes is not null)
//         {
//             string extension = article.ImageContentType switch
//             {
//                 "image/png" => ".png",
//                 "image/jpeg" => ".jpg",
//                 "image/gif" => ".gif",
//                 "image/svg+xml" => ".svg",
//                 "image/webp" => ".webp",
//                 _ => ".img"
//             };
//             string imagePath = Path.Combine(folder, "main_image" + extension);
//             await File.WriteAllBytesAsync(imagePath, article.ImageBytes);
//         }
//
//         return folder;
//     }
// }