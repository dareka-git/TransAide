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
    public class TransAideImportConfig
    {
        [JsonPropertyName("import_type")]
        public ImportTypeEnum ImportType { get; set; } = ImportTypeEnum.TargetOnlyClipboard; // TargetOnlyTextFile, TargetOnlyClipboard, JSONFile"

        [JsonPropertyName("import_file")]
        public string ImportFile { get; set; } = ""; // "tgt.txt", "tgt.json"

        [JsonPropertyName("import_from")]
        public ImportFromEnum ImportFrom { get; set; } = ImportFromEnum.CurrentSegment; // CurrentSegment, FirstSegment

        [JsonPropertyName("import_to")]
        public ImportToEnum ImportTo { get; set; } = ImportToEnum.Next50Segments; // Next50Segments, Next100Segments, ToEnd

        [JsonPropertyName("destination_segment_type")]
        public DestinationSegmentTypeEnum DestinationSegmentType { get; set; } = DestinationSegmentTypeEnum.UnconfirmedSegments; // UnconfirmedSegments, UnlockedSegments, AllSegments, FilteredSegments

        [JsonPropertyName("import_file_content")]
        public ImportFileContentEnum ImportFileContent { get; set; } = ImportFileContentEnum.OnlySegmentsToImport; // OnlySegmentsToImport, AllSegments


        public TransAideImportConfig()
        {
        }

        public TransAideImportConfig(ImportTypeEnum importType, string importFile, ImportFromEnum importFrom, 
            ImportToEnum importTo, DestinationSegmentTypeEnum destinationSegmentType, ImportFileContentEnum importFileContent )
        {
            ImportType = importType;
            ImportFile = importFile;
            ImportFrom = importFrom;
            ImportTo = importTo;
            DestinationSegmentType = destinationSegmentType;
            ImportFileContent = importFileContent;
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, GetDefaultSerializeOptions());
        }

        public static TransAideImportConfig LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                //Console.WriteLine("File does not exist.");
                return new TransAideImportConfig();
            }

            try
            {
                string jsonString = File.ReadAllText(filePath);
                TransAideImportConfig transAideImportConfig = JsonSerializer.Deserialize<TransAideImportConfig>(jsonString, GetDefaultSerializeOptions());
                return transAideImportConfig;
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"The file was not found: {ex.Message}");
                return default(TransAideImportConfig);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return default(TransAideImportConfig);
            }
        }

        public static void WriteToFile(TransAideImportConfig transAideImportConfig, string filePath)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(transAideImportConfig, GetDefaultSerializeOptions());
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
