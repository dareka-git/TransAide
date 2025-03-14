using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TransAide.Helper
{
    public class FileManager
    {

        public static List<string> LoadTextFileLines(string filePath)
        {
            List<string> lines = new List<string>();

            try
            {
                if (File.Exists(filePath))
                {
                    lines = new List<string>(File.ReadAllLines(filePath));
                }
                else
                {
                    MessageBox.Show("File does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show($"The file was not found: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"You do not have permission to access this file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (IOException ex)
            {
                MessageBox.Show($"An I/O error occurred while opening the file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return lines;
        }

        public static List<string> LoadTextFromClipboard()
        {
            List<string> lines = new List<string>();

            try
            {
                if (Clipboard.ContainsText())
                {
                    string clipboardText = Clipboard.GetText();
                    lines = new List<string>(clipboardText.Split(new[] { Environment.NewLine }, StringSplitOptions.None));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return lines;
        }


        public static string GetFileNamePath(string filePath, string projectFolder)
        {
            projectFolder = (projectFolder.EndsWith("\\")) ? projectFolder : projectFolder + "\\";
            // string _filePath = System.IO.Path.GetDirectoryName(filePath);
            return (String.IsNullOrEmpty(System.IO.Path.GetDirectoryName(filePath))) ? projectFolder + filePath : filePath;
        }

    }
}
