using Blasphemous.ModdingAPI.Files;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Blasphemous.CustomBackgrounds.Extensions;

internal static class FileHandlerExtensions
{
    internal static T LoadDataAsJson<T>(this FileHandler fileHandler, string fileName)
    {
        if (!fileHandler.LoadDataAsJson(fileName, out T result))
        {
            throw new ArgumentException($"Failed to load {fileName} to JSON of type {typeof(T)}!");
        }
        return result;
    }

    internal static bool LoadContentAsJson<T>(this FileHandler fileHandler, string fileName, out T output)
    {
        if (ReadFileContents(fileHandler, fileHandler.ContentFolder + fileName, out var output2))
        {
            output = JsonConvert.DeserializeObject<T>(output2);
            return true;
        }

        output = default(T);
        return false;
    }

    private static bool ReadFileContents(this FileHandler fileHandler, string path, out string output)
    {
        if (File.Exists(path))
        {
            output = File.ReadAllText(path);
            return true;
        }

        output = null;
        return false;
    }
}
