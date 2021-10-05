using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Core.Entities.Auth
{
    public class FaceBookUserInfoResult
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("picture")]
        public FaceBookPicture Picture { get; set; }
    }

    public class FaceBookPicture
    {
        [JsonProperty("data")]
        public FaceBookPictureData Data { get; set; }
    }

    public class FaceBookPictureData
    {
        [JsonProperty("height")]
        public long Height { get; set; }

        [JsonProperty("is_silhouette")]
        public bool IsSilhouette { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("width")]
        public long Width { get; set; }
    }
}
