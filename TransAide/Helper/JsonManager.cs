using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;
using System.Windows;
using TransAide.Model;

namespace TransAide.Helper
{
    public class JsonManager
    {
        public static T LoadFromFile<T>(string filePath) where T : new()
        {
            if (!File.Exists(filePath))
            {
                //MessageBox.Show("File does not exist.");
                return new T();
            }

            try
            {
                string jsonString = File.ReadAllText(filePath);
                T obj = JsonSerializer.Deserialize<T>(jsonString, GetDefaultSerializeOptions());
                return obj;
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show($"The file was not found: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return default(T);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return default(T);
            }
        }

        public static void WriteToFile<T>(T obj, string filePath)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(obj, GetDefaultSerializeOptions());
                File.WriteAllText(filePath, jsonString);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving the JSON object to file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static List<T> LoadListFromFile<T>(string filePath) where T : new()
        {
            if (!File.Exists(filePath))
            {
                //Console.WriteLine("File does not exist.");
                return new List<T>();
            }

            try
            {
                string jsonString = File.ReadAllText(filePath);
                List<T> objList = JsonSerializer.Deserialize<List<T>>(jsonString, GetDefaultSerializeOptions());
                return objList;
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show($"The file was not found: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<T>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<T>();
            }
        }

        public static void WriteListToFile<T>(List<T> objList, string filePath)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(objList, GetDefaultSerializeOptions());
                File.WriteAllText(filePath, jsonString);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving the JSON object to file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
