namespace TextToSpeech.models
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    //This class represents the Json output from Rhubarb Lip Sync
    public partial class Speach
    {
        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; }

        [JsonProperty("mouthCues")]
        public List<MouthCue> MouthCues { get; set; }
    }

    public partial class Metadata
    {
        [JsonProperty("soundFile")]
        public string SoundFile { get; set; }

        [JsonProperty("duration")]
        public float Duration { get; set; }
    }

    public partial class MouthCue
    {
        [JsonProperty("start")]
        public float Start { get; set; }

        [JsonProperty("end")]
        public float End { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
