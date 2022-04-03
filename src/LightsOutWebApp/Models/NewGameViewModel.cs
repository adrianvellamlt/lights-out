using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace LightsOut.Web
{
    public class NewGameViewModel
    {
        [JsonConstructor]
        public NewGameViewModel(string username)
        {
            Username = username;
        }

        [JsonProperty("username")]
        [MaxLength(15)]
        public string Username { get; set; }
    }
}