using System;
using Newtonsoft.Json;

namespace LightsOut.Web
{
    public abstract class BaseGameSettingViewModel
    {

        [JsonConstructor]
        public BaseGameSettingViewModel
        (
            byte complexity,
            byte noOfRows,
            byte noOfColumns,
            byte noOfSwitchedOnLights,
            TimeSpan gameMaxDuration
        )
        {
            Complexity = complexity;
            NoOfRows = noOfRows;
            NoOfColumns = noOfColumns;
            NoOfSwitchedOnLights = noOfSwitchedOnLights;
            GameMaxDuration = gameMaxDuration;
        }
        [JsonProperty("complexity")]
        public byte Complexity { get; set; }
        [JsonProperty("noOfRows")]
        public byte NoOfRows { get; set; }
        [JsonProperty("noOfColumns")]
        public byte NoOfColumns { get; set; }
        [JsonProperty("noOfSwitchedOnLights")]
        public byte NoOfSwitchedOnLights { get; set; }
        [JsonProperty("gameMaxDuration")]
        public TimeSpan GameMaxDuration { get; set; }
    }

    public class GameSettingViewModel : BaseGameSettingViewModel
    {
        public GameSettingViewModel
        (
            ushort id,
            byte complexity,
            byte noOfRows, 
            byte noOfColumns, 
            byte noOfSwitchedOnLights, 
            TimeSpan gameMaxDuration
        ) : base(complexity, noOfRows, noOfColumns, noOfSwitchedOnLights, gameMaxDuration)
        {
            Id = id;
        }
        
        [JsonProperty("id")]
        public ushort Id { get; set; }
    }

    public class UpdateGameSettingViewModel : BaseGameSettingViewModel
    {
        public UpdateGameSettingViewModel
        (
            byte complexity,
            byte noOfRows, 
            byte noOfColumns, 
            byte noOfSwitchedOnLights, 
            TimeSpan gameMaxDuration
        ) : base(complexity, noOfRows, noOfColumns, noOfSwitchedOnLights, gameMaxDuration)
        { }
    }
}