namespace EverythingKnownToMan.backend;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;

public class StringNameJsonConverter : JsonConverter<StringName>
{
    public override StringName Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => new StringName(reader.GetString());

    public override void Write(Utf8JsonWriter writer, StringName value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString());
}