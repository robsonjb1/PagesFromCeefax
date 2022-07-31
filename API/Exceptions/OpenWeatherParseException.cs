class OpenWeatherParseException : Exception
{
    public OpenWeatherParseException() { }

    public OpenWeatherParseException(string response, Exception innerException)
        : base($"Unable to parse API response from OpenWeatherMap. Please check your API key in appsettings.json. {response}", innerException)
    {

    }
}