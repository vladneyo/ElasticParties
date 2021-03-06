﻿using ElasticParties.Data.Constants;
using ElasticParties.Data.Converters;
using ElasticParties.Data.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ElasticParties.Services
{
    public class GooglePlacesService
    {
        public async Task<List<Place>> GetDataAsync()
        {
            using (var httpClient = new HttpClient())
            {
                List<Place> places = new List<Place>();
                foreach (var place in PlaceTypes.Places)
                {
                    var content = await httpClient.GetStringAsync(string.Format(GoogleConstants.SearchPlacesLinkPattern, 50.0268781, 36.2205936, 200000, place.Key, place.Value, GoogleConstants.GooglePlacesAPIKey));
                    var json = await Task.Run(() => JObject.Parse(content));
                    var jSettings = new JsonSerializerSettings
                    {
                        ContractResolver = new DefaultContractResolver
                        {
                            NamingStrategy = new SnakeCaseNamingStrategy(),
                        }
                    };
                    jSettings.Converters.Add(new LocationConverter());

                    places.AddRange(JsonConvert.DeserializeObject<List<Place>>(json.Property("results").Value.ToString(), jSettings));
                }
                return places;
            }
        }
    }
}
