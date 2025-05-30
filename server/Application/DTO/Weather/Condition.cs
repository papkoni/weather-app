using System.Text.Json.Serialization;

namespace Application.DTO.Weather;

public record Condition(
    string Text, 
    [property: JsonPropertyName("icon")]
    string Icon);
