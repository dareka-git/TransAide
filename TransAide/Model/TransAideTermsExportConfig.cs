using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TransAide.Enum;

namespace TransAide.Model
{
    public class TransAideTermsExportConfig
    {
        [JsonPropertyName("terms_export_file")]
        public string TermsExportFile { get; set; } = "terms.txt";
        
        [JsonPropertyName("source_segment_type")]
        public SourceSegmentTypeEnum SourceSegmentType { get; set; } = SourceSegmentTypeEnum.AllSegments; // UnconfirmedSegments, UnlockedSegments, AllSegments, FilteredSegments

        public TransAideTermsExportConfig()
        {
        }

        public TransAideTermsExportConfig(string termsExportFile, SourceSegmentTypeEnum sourceSegmentType)
        {
            TermsExportFile = termsExportFile;
            SourceSegmentType = sourceSegmentType;
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, GetDefaultSerializeOptions());
        }

        public static TransAideTermsExportConfig LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                //Console.WriteLine("File does not exist.");
                return new TransAideTermsExportConfig();
            }

            try
            {
                string jsonString = File.ReadAllText(filePath);
                TransAideTermsExportConfig transAideTermsExportConfig = JsonSerializer.Deserialize<TransAideTermsExportConfig>(jsonString, GetDefaultSerializeOptions());
                return transAideTermsExportConfig;
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"The file was not found: {ex.Message}");
                return default(TransAideTermsExportConfig);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return default(TransAideTermsExportConfig);
            }
        }

        public static void WriteToFile(TransAideTermsExportConfig transAideTermsExportConfig, string filePath)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(transAideTermsExportConfig, GetDefaultSerializeOptions());
                File.WriteAllText(filePath, jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while saving the JSON object to file: {ex.Message}");
            }
        }

        public static JsonSerializerOptions GetDefaultSerializeOptions()
        {
            return new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }

            };
        }
    }
}
