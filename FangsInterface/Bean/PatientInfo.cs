using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FangsInterface.Bean
{
    public class PatientInfo :BaseModel
    {
        public PatientInfo()
        {
            pInfo = new PInfo(); 
        }
         
        [JsonProperty("data")]
        public PInfo pInfo; 
    } 

    public class PInfo 
    {
        [JsonProperty("pId")]
        public string pId { get; set; }

        [JsonProperty("pNme")]
        public string pNme { get; set; }

        [JsonProperty("bedNo")]
        public string bedNo { get; set; }

    }
}