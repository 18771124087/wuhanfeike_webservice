using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FangsInterface.Bean
{
    public class MedicalAdvice : BaseModel
    {
        public MedicalAdvice()
        {
            data = new List<MedicalAdviceInfo>();
        }

        public List<MedicalAdviceInfo> data { get; set; }
    }

    public class MedicalAdviceInfo
    { 
        [JsonProperty("barCode")]
        public string barCode { get; set; }

        [JsonProperty("pNme")]
        public string name { get; set; }  

        [JsonProperty("adviceId")]
        public string adviceId { get; set; }
         
        [JsonProperty("adviceName")]
        public string adviceName { get; set; }

    }
}