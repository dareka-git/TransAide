using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using TransAide.Enum;

namespace TransAide.Model
{
    public class TransAideExportConfig
    {
        [JsonPropertyName("export_type")]
        public ExportTypeEnum ExportType { get; set; } = ExportTypeEnum.SourceOnlyTextFile; // SourceOnlyTextFile, JSONFile

        [JsonPropertyName("export_file")]
        public string ExportFile { get; set; } = "src.txt"; // "src.txt", "src.json"

        [JsonPropertyName("text_format")]
        public TextFormatEnum TextFormat { get; set; } = TextFormatEnum.PlainTextWithoutTags; // PlainTextWithoutTags, TextWithTags

        [JsonPropertyName("source_segment_type")]
        public SourceSegmentTypeEnum SourceSegmentType { get; set; } = SourceSegmentTypeEnum.UnconfirmedSegments; // UnconfirmedSegments, UnlockedSegments, AllSegments, FilteredSegments

        public TransAideExportConfig()
        {
        }

        public TransAideExportConfig(ExportTypeEnum exportType, string exportFile, TextFormatEnum textFormat, SourceSegmentTypeEnum sourceSegmentType)
        {
            ExportType = exportType;
            ExportFile = exportFile;
            TextFormat = textFormat;
            SourceSegmentType = sourceSegmentType;
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, GetDefaultSerializeOptions());
        }

        public static TransAideExportConfig LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                //Console.WriteLine("File does not exist.");
                return new TransAideExportConfig();
            }

            try
            {
                string jsonString = File.ReadAllText(filePath);
                TransAideExportConfig transAideExportConfig = JsonSerializer.Deserialize<TransAideExportConfig>(jsonString, GetDefaultSerializeOptions());
                return transAideExportConfig;
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"The file was not found: {ex.Message}");
                return default(TransAideExportConfig);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return default(TransAideExportConfig);
            }
        }

        public static void WriteToFile(TransAideExportConfig transAideExportConfig, string filePath)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(transAideExportConfig, GetDefaultSerializeOptions());
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
