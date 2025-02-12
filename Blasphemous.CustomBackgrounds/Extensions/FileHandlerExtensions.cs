using Blasphemous.ModdingAPI.Files;
using System;

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
}
