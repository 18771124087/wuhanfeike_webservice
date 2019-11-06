using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FangsInterface.Bean
{
    public class RejectionReason:BaseModel
    {
        public RejectionReason()
        {
            data = new List<RejectionReasonInfo>();
        }

        public List<RejectionReasonInfo> data;
    }

    public class RejectionReasonInfo
    {
        [JsonProperty("reasonId")]
        public string reasonId { get; set; }

        [JsonProperty("reasonName")]
        public string reasonName { get; set; } 
    }
}