#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

#endregion

namespace StrategyTester;

public static class Program
{
    public static async Task Main(string[] args)
    {
        // Win
        var runnerDir = $"{AppDomain.CurrentDomain.BaseDirectory}../../../../../app-windows";
        var runnerExe = @$"{runnerDir}\aicup22.exe";
        var workFolder = new DirectoryInfo($"{runnerDir}/../StrategyTester");
        var testResultsFile = $"{runnerDir}/../TestResults.txt";
        var testIterations = 500;
        var instances = 50;
        
        // Linux
        // var runnerDir = @"/home/yellowfive/OneDrive/MY/#CODE/#RussianAiCup/'22/app-linux";
        // var runnerExe = @"/home/yellowfive/OneDrive/MY/#CODE/#RussianAiCup/'22/app-linux/aicup22";
        // var workFolder = new DirectoryInfo(@"/home/yellowfive/OneDrive/MY/#CODE/#RussianAiCup/'22/StrategyTester");
        // var testResultsFile = @"/home/yellowfive/OneDrive/MY/#CODE/#RussianAiCup/'22/TestResults.txt";
        // var testIterations = 500;
        // var instances = 20;

        for (var i = 0; i < testIterations / instances; i++)
        {
            var result = Parallel.For(0,
                                      instances,
                                      (i, state) =>
                                      {
                                          var p = Process.Start(new ProcessStartInfo(runnerExe)
                                                                {
                                                                    // Arguments = $"--config {runnerDir}/../StrategyTester/configs/config({i}).json --batch-mode --save-results ../StrategyTester/{Guid.NewGuid()}.json",
                                                                    Arguments = $"--config {runnerDir}/../StrategyTester/configs2/config({i}).json --batch-mode --save-results ../StrategyTester/{Guid.NewGuid()}.json",
                                                                    WorkingDirectory = runnerDir
                                                                });
                                          p.WaitForExitAsync().Wait();
                                      });
        }

        var kills = new List<double>();
        var damages = new List<double>();
        var places = new List<double>();
        var scores = new List<double>();
        var crashes = 0;

        foreach (var file in workFolder.GetFiles("*.json"))
        {
            var result = JObject.Parse(File.ReadAllText(file.FullName));
            kills.Add(result["results"]["players"][0]["kills"].Value<double>());
            damages.Add(result["results"]["players"][0]["damage"].Value<double>());
            places.Add(result["results"]["players"][0]["place"].Value<double>());
            scores.Add(result["results"]["players"][0]["score"].Value<double>());
            if (result["players"][0]["crashed"].Value<bool>())
            {
                crashes++;
            }

            File.Delete(file.FullName);
        }

        var resultData = "\n" +
                         "*** *** *** \n" +
                         "\n" +
                         $"Crashes: {crashes}\n" +
                         $"Avg score {scores.Sum() / scores.Count} \n" +
                         $"Avg place {places.Sum() / places.Count} \n" +
                         $"Total score {scores.Sum()} \n" +
                         $"Best place {places.Min()} \n" +
                         $"Total damage {damages.Sum()} \n" +
                         $"Avg damage {damages.Sum() / damages.Count} \n" +
                         $"Total kills {kills.Sum()} \n" +
                         $"Avg kills {kills.Sum() / kills.Count} \n" +
                         "\n" +
                         "- - - - - -" +
                         "\n";

        Console.WriteLine(resultData);

        var currentContent = await File.ReadAllTextAsync(testResultsFile);
        await File.WriteAllTextAsync(testResultsFile, resultData + currentContent);
    }
}