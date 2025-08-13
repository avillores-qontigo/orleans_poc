
namespace OrleansURLShortener.Grains
{
    public sealed class UrlShortenerGrain : Grain, IUrlShortenerGrain
    {
        IPersistentState<UrlDetails> _state;

        public UrlShortenerGrain([PersistentState(stateName: "url", storageName: "cosmosStore")] IPersistentState<UrlDetails> state)
        {
            _state = state;
        }
        public async Task<string?> GetUrl()
        {
            return _state.State.FullUrl;
        }

        public async Task SetUrl(string longUrl)
        {
            var grainId = this.GetPrimaryKeyString();
            _state.State.FullUrl = longUrl;
            _state.State.ShortenedRouteSegment = grainId;
            _state.State.PartitionKey = grainId;
            
            await _state.WriteStateAsync();
        }
    }

    [GenerateSerializer, Alias(nameof(UrlDetails))]
    public record UrlDetails
    {
        [Id(0)]
        public string PartitionKey { get; set; } = "";

        [Id(1)]
        public string FullUrl { get; set; } = "";

        [Id(2)]
        public string ShortenedRouteSegment { get; set; } = "";
    }
}
