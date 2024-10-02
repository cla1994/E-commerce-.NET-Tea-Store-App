using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeaStoreApp.Models
{
    public class Category
    {
        // json property cambia nome di var quando viene serializzata (serve nuget, using newtonsoft in alto)
        // serve perché noi definiamo var con maiuscole ma json le processa con solo minuscole
        // si può trovare classe c# da api tramite sito json2csharp.com
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; }

        // => expression body function per risparmiare tempo nello scrivere get e return
        public string FullImageUrl => AppSettings.ApiUrl + ImageUrl;  
    }
}
