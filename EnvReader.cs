using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public static class EnvReader
{
  public static Dictionary<string, string> Load(string path)
  {
    if (!File.Exists(path))
    {
      throw new FileNotFoundException("The specified .env file could not be found.", path);
    }

    var envVars = File.ReadAllLines(path)
        .Where(line => !string.IsNullOrWhiteSpace(line) && !line.StartsWith("#"))
        .Select(line =>
        {
          var parts = line.Split(new[] { '=' }, 2);
          // Removing double quotes from the value
          var valueWithoutQuotes = parts[1].Trim().Trim('"');
          return new { Key = parts[0].Trim(), Value = valueWithoutQuotes };
        })
        .ToDictionary(parts => parts.Key, parts => parts.Value);

    foreach (var envVar in envVars)
    {
      Environment.SetEnvironmentVariable(envVar.Key, envVar.Value);
    }

    return envVars;
  }
}

