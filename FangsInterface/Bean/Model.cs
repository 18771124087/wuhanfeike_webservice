using FangsInterface.Bean;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace FangsInterface
{
    public class Model : BaseModel
    { 
        public Model()
        {
            infos = new List<Info>();
        }

        [JsonProperty("infos")]
        public List<Info> infos;
    }

    public class Info
    {
        [JsonProperty("orderType")]
        public string OrderType { get; set; }

        [JsonProperty("orderNo")]
        public string OrderNo { get; set; }

        [JsonProperty("orderDate")]
        public string OrderDate { get; set; }

        [JsonProperty("orderAmount")]
        public string OrderAmount { get; set; }

        [JsonProperty("orderStatus")]
        public string OrderStatus { get; set; }
    }
}