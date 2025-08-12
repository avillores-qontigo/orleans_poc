
namespace OrleansURLShortener.Grains
{
    public sealed class UrlShortenerGrain : Grain, IUrlShortenerGrain
    {
        IPersistentState<UrlDetails> _state;

        public UrlShortenerGrain([PersistentState(stateName: "url", storageName: "urls")] IPersistentState<UrlDetails> state)
        {
            _state = state;
        }
        public async Task<string> GetUrl()
        {
            await _state.ReadStateAsync();
            return _state.State.FullUrl;
        }

        public async Task SetUrl(string longUrl)
        {
            _state.State = new() { 
                FullUrl = longUrl,
                ShortenedRouteSegment = this.GetPrimaryKeyString()
            };

            await _state.WriteStateAsync();
        }
    }

    [GenerateSerializer, Alias(nameof(UrlDetails))]
    public record UrlDetails
    {
        [Id(0)]
        public string FullUrl { get; set; } = "";

        [Id(1)]
        public string ShortenedRouteSegment { get; set; } = "";
    }
}
