using Newtonsoft.Json;

namespace LightsOut.Web
{
    public class HighScoreViewModel
    {
        [JsonConstructor]
        public HighScoreViewModel
        (
            ushort rank,
            string username,
            byte complexityLevel,
            byte noOfRows,
            byte noOfColumns,
            byte remainingLights,
            long timeTakenSeconds,
            ushort noOfMoves
        )
        {
            Rank = rank;
            Username = username;
            ComplexityLevel = complexityLevel;
            NoOfRows = noOfRows;
            NoOfColumns = noOfColumns;
            RemainingLights = remainingLights;
            TimeTakenSeconds = timeTakenSeconds;
            NoOfMoves = noOfMoves;
        }
        [JsonProperty("rank")]
        public ushort Rank {get;set;}
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("complexityLevel")]
        public byte ComplexityLevel { get; set; }
        [JsonProperty("noOfRows")]
        public byte NoOfRows { get; set; }
        [JsonProperty("noOfColumns")]
        public byte NoOfColumns { get; set; }
        [JsonProperty("remainingLights")]
        public byte RemainingLights { get; set; }
        [JsonProperty("timeTakenSeconds")]
        public long TimeTakenSeconds { get; set; }
        [JsonProperty("noOfMoves")]
        public ushort NoOfMoves { get; set; }
    }
}