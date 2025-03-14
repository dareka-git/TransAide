using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;
using TransAide.Enum;

namespace TransAide.Model
{
    public class TranslationUnit
    {
        [JsonPropertyName("segment_id")]
        public string SegmentId { get; set; }
        
        [JsonPropertyName("file_id")]
        public Guid FileId { get; set; }

        [JsonPropertyName("file_name")]
        public string FileName { get; set; }

        [JsonPropertyName("match")]
        public int Match { get; set; }

        [JsonPropertyName("status")]
        public SegmentStatusEnum Status { get; set; }

        //[JsonPropertyName("previous_source")]
        //public string PreviousSource { get; set; }
        
        //[JsonPropertyName("previous_target")]
        //public string PreviousTarget { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; }

        [JsonPropertyName("target")]
        public string Target { get; set; }

        [JsonPropertyName("comment")]
        public string Comment { get; set; }


        public TranslationUnit()
        {

        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, GetDefaultSerializeOptions());
        }

        public static JsonSerializerOptions GetDefaultSerializeOptions()
        {
            return new JsonSerializerOptions
            {
                WriteIndented = true,
                //Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.LatinExtendedA),
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };
        }

    }
}
