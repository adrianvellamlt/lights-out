using Newtonsoft.Json;

namespace LightsOut.Web
{
    public class ToggleCellViewModel
    {
        [JsonConstructor]
        public ToggleCellViewModel(byte rowNumber, byte columnNumber)
        {
            RowNumber = rowNumber;
            ColumnNumber = columnNumber;
        }

        [JsonProperty("rowNumber")]
        public byte RowNumber { get; set; }

        [JsonProperty("columnNumber")]
        public byte ColumnNumber { get; set; }
    }
}