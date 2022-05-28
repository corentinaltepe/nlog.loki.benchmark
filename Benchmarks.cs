using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using NLog;

namespace nlog.loki.benchmark
{
    // Run the benchmark with the following command line:
    // dotnet run --configuration Release --framework net48 --runtimes net48 netcoreapp31 net60 --filter * --join
    [MemoryDiagnoser]
    [MarkdownExporterAttribute.GitHub]
    public class Program
    {
        // Prepare logger and logging statements
        private Logger Logger;
        private readonly string[] Users = new[] { "Corentin Altepe", "A", "B", "C", "D", "E", "F", "G", "H" };
        private readonly string[] Destinations = new[] { "127.0.0.1:8475", "A", "B", "C", "D", "E", "F", "G", "H" };
        private readonly Exception Exception = new("Ooops. Something went wrong!");
        private readonly List<string> LoggingMessages = new() {
            "4 error CS8400: Feature 'target-typed object creation' is not available in C# 8.0. Please use language version 9.0 or greater.",
            "5 error CS8370: Feature 'target-typed object creation' is not available in C# 7.3. Please use language version 9.0 or greater.",
            "6 error CS0246: The type or namespace name 'Logger' could not be found (are you missing a using directive or an assembly reference?)",
        };
        
        static void Main(string[] args) => BenchmarkSwitcher.FromAssemblies(new[] { typeof(Program).Assembly }).Run(args);

        [Params(4, 12, 100, 1000)]
        public int NumberLogs;

        private List<Action> LoggingStatements;
        [GlobalSetup]
        public void GlobalSetup()
        {
            LoggingStatements = new();

            var i = 0;
            while(i < NumberLogs) {
                LoggingStatements.Add(() => Logger.Info("1 Receive message from {User} with destination {Destination}.", Users[i % Users.Length], Destinations[i % Destinations.Length]));
                i++;
                LoggingStatements.Add(() => Logger.Info("2 Receive message from {User} with destination {Destination}.", Users[i % Users.Length], Destinations[i % Destinations.Length]));
                i++;
                LoggingStatements.Add(() => Logger.Info("3 Receive message from {User} with destination {Destination}.", Users[i % Users.Length], Destinations[i % Destinations.Length]));
                i++;
                LoggingStatements.Add(() => Logger.Info(LoggingMessages[i % 3]));
                i++;
            }
        }

        [IterationSetup(Targets = new[] { nameof(Grpc) })]
        public void IterationSetupGrpc() 
        {
            Logger = LogManager.GetCurrentClassLogger();
            LogManager.Configuration.LoggingRules.Clear();
            LogManager.Configuration.LoggingRules.Add(new("*", LogLevel.Info, LogLevel.Fatal, LogManager.Configuration.FindTargetByName("lokigrpc")));
            LogManager.ReconfigExistingLoggers();
        }

        [Benchmark]
        public void Grpc()
        {
            foreach(var action in LoggingStatements)
                action();
            LogManager.Flush();
        }

        [IterationSetup(Targets = new[] { nameof(Http) })]
        public void IterationSetupHttp() 
        {
            Logger = LogManager.GetCurrentClassLogger();
            LogManager.Configuration.LoggingRules.Clear();
            LogManager.Configuration.LoggingRules.Add(new("*", LogLevel.Info, LogLevel.Fatal, LogManager.Configuration.FindTargetByName("lokihttp")));
            LogManager.ReconfigExistingLoggers();
        }

        [Benchmark]
        public void Http()
        {
            foreach(var action in LoggingStatements)
                action();
            LogManager.Flush();
        }
    }
}
