using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FangsInterface.Bean
{
    public class PdaUser : BaseModel
    {
        public PdaUser()
        {
            row = new RowInfo();
        }

        [JsonProperty("data")]
        public RowInfo row;
    }

    public class RowInfo
    {
        [JsonProperty("oprId")]
        public string oprId { get; set; }

        [JsonProperty("oprName")]
        public string oprName { get; set; }

        [JsonProperty("role_no")]
        public string role_no { get; set; }

        [JsonProperty("role_name")]
        public string role_name { get; set; }

        [JsonProperty("department")]
        public string department { get; set; } 
    }
}