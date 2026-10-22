using System.Text.Json.Serialization;

public class RetroTVChannelData
{
    [JsonPropertyName("lastRefresh")]
    public string LastRefresh { get; set; }

    [JsonPropertyName("episodeList")]
    public List<RetroTVEpisodeData> EpisodeList { get; set; }
}

public class RetroTVEpisodeData
{
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("shortName")]
    public string ShortName { get; set; }

    [JsonPropertyName("urlLocal")]
    public string UrlLocal { get; set; }

    [JsonPropertyName("length")]
    public double Length { get; set; }

    [JsonPropertyName("isVideo")]
    public bool IsVideo { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }
}
