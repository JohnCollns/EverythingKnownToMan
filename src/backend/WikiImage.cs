using System;
using System.IO;
using Godot;
using Godot.Collections;

namespace EverythingKnownToMan.backend;

public static class WikiImageFormats
{
    public static readonly StringName jpg = "jpg";
    public static readonly StringName png = "png";
    //  public static readonly StringName gif = "gif";  // Gif is not supported by Godot.Image
    public static readonly StringName webp = "webp";
    public static readonly StringName unknown = "unknown";
    
    public enum ImageFormat
    {
        jpg,
        png,
        //gif,
        webp,
        error
    }

    private static Dictionary<StringName, ImageFormat> ImageFormats = new Dictionary<StringName, ImageFormat>()
    {
        {jpg, ImageFormat.jpg},
        {png, ImageFormat.png},
        //{gif, ImageFormat.gif},
        {webp, ImageFormat.webp},
    };

    public static ImageFormat GetImageFormatFromTag(StringName tag)
    {
        if (ImageFormats.TryGetValue(tag, out ImageFormat format))
        {
            return format;
        }
        return ImageFormat.error;
    }

    public static StringName StringToTag(string tag)
    {
        // I don't think this is computationally efficient, don't know C# well enough to improve it. 
        string formattedTag = tag.ToLower();
        formattedTag = formattedTag.Replace("image/", "");
        // if (tag.StartsWith("image/"))
        // {
        //     tag = tag.Replace("image/", "");
        // }
        if (formattedTag == "jpg" || formattedTag == "jpeg")
        {
            return jpg;
        }
        if (formattedTag == "png")
        {
            return png;
        }
        if (formattedTag == "webp")
        {
            return webp;
        }
        GD.Print("WikiImage: Could not resolve image type: " + tag);
        return unknown;
    }
}

public class WikiImage
{
    public StringName FileFormat;
    // public string URL;
    public byte[] ImageBytes;
    
    public WikiImage() {}

    public WikiImage(StringName fileFormat, byte[] imageBytes)
    {
        FileFormat = fileFormat;
        ImageBytes = imageBytes;
    }

    public WikiImage(string fileFormat, byte[] imageBytes)
    {
        FileFormat = WikiImageFormats.StringToTag(fileFormat);
        ImageBytes = imageBytes;
    }

    public bool IsValid()
    {
        return !FileFormat.IsEmpty && ImageBytes != null && ImageBytes.Length > 0;
    }

    public string SaveToDisk(string fileName)
    {
        string exeDir = AppContext.BaseDirectory;
        // Returns:
        // E:\Godot\GameAWeek26\EverythingKnownToMan\Godot\everything-known-to-man\.godot\mono\temp\bin\Debug\
        // Target:
        // E:\Godot\GameAWeek26\EverythingKnownToMan\Godot\everything-known-to-man\generated\images\
        
        string projectRoot = Path.GetFullPath(Path.Combine(exeDir, "..", "..", "..", "..", ".."));
        //GD.Print(" projectRoot: " + projectRoot);
        string path = Path.Combine(projectRoot, "generated", "images", (fileName + "." + FileFormat));
        //GD.Print(" path: " + path);
        File.WriteAllBytes(path, ImageBytes);
        return path;
    }
}