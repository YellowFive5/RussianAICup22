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

        var parallelPool = 23;

        var testIterations = 0;
        for (var i = 500; i < 550; i++)
        {
            if (i % parallelPool == 0)
            {
                testIterations = i;
                break;
            }
        }

        var cb = new Dictionary<int, string>();
        for (var i = 0; i < parallelPool; i++)
        {
            cb.TryAdd(i, "");
        }

        var counter = 1;
        var locker = new object();

        // Linux
        // var runnerDir = @"/home/yellowfive/OneDrive/MY/#CODE/#RussianAiCup/'22/app-linux";
        // var runnerExe = @"/home/yellowfive/OneDrive/MY/#CODE/#RussianAiCup/'22/app-linux/aicup22";
        // var workFolder = new DirectoryInfo(@"/home/yellowfive/OneDrive/MY/#CODE/#RussianAiCup/'22/StrategyTester");
        // var testResultsFile = @"/home/yellowfive/OneDrive/MY/#CODE/#RussianAiCup/'22/TestResults.txt";
        // var testIterations = 500;
        // var instances = 20;

        Parallel.For(0,
                     testIterations / parallelPool,
                     (i1, state) =>
                     {
                         Parallel.For(0,
                                      parallelPool,
                                      (i2, state) =>
                                      {
                                          while (true)
                                          {
                                              bool contains;
                                              lock (locker)
                                              {
                                                  contains = cb.ContainsKey(i2);
                                              }

                                              if (contains)
                                              {
                                                  lock (locker)
                                                  {
                                                      cb.Remove(i2, out _);
                                                  }

                                                  var p = Process.Start(new ProcessStartInfo(runnerExe)
                                                                        {
                                                                            WorkingDirectory = runnerDir,
                                                                            
                                                                            // Arguments = $"--config {runnerDir}/../StrategyTester/configs/config({i}).json --batch-mode --save-results ../StrategyTester/{Guid.NewGuid()}.json",
                                                                            Arguments = $"--config {runnerDir}/../StrategyTester/configs2/config({i2}).json --batch-mode --save-results ../StrategyTester/{Guid.NewGuid()}.json",
                                                                            // Arguments = $"--config {runnerDir}/../StrategyTester/configs3/config({i2}).json --batch-mode --save-results ../StrategyTester/{Guid.NewGuid()}.json",
                                                                        });
                                                  p.WaitForExitAsync().Wait();
                                                  Console.WriteLine($"- - - - ~{counter++} / {testIterations} - - - - ");
                                                  lock (locker)
                                                  {
                                                      cb.TryAdd(i2, "");
                                                  }

                                                  return;
                                              }

                                              Task.Delay(TimeSpan.FromSeconds(1)).Wait();
                                          }
                                      });
                     });

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

            try
            {
                result["players"][0]["crash"].Value<string>();
            }
            catch (Exception)
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