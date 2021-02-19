using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace challenge1
{


    public class PallyHostedService : IHostedService
    {
        private readonly ILogger<PallyHostedService> _logger;
        private readonly IPalindromeTestEngine _testEngine;
        public PallyHostedService(IPalindromeTestEngine testEngine, ILogger<PallyHostedService> logger)
        {
            _testEngine = testEngine;
            _logger = logger;
        }

        private bool ContainsTrue(string fileName) => new FileInfo(fileName).Name.ToLower().Contains("true");
        private bool ContainsFalse(string fileName) => new FileInfo(fileName).Name.ToLower().Contains("false");


        private bool Correct(PallyResult pallyResult) =>
        (ContainsTrue(pallyResult.fileName), ContainsFalse(pallyResult.fileName), pallyResult.result) switch
        {
            (true, _, true) => true,
            (_, true, false) => true,
            _ => false
        };

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await TestAllLogFailed(cancellationToken);
        }

        private async Task TestAllLogFailed(CancellationToken cancellationToken)
        {
            var failedResults = await TestAllSeparateFailed(cancellationToken);
            ErrorOutput(failedResults);
        }

        private async Task<IEnumerable<PallyResult>> TestAllSeparateFailed( CancellationToken cancellationToken)
        {
            List<PallyResult> failedResults = new List<PallyResult>();
            await foreach (var result in TestAll(cancellationToken))
            {
                AddToListIfFailed(failedResults, result);
            }

            return failedResults;
        }

        private void AddToListIfFailed(List<PallyResult> failedResults, PallyResult result)
        {
            if (!Correct(result))
                    failedResults.Add(result);
        }

        private async IAsyncEnumerable<PallyResult> TestAll(CancellationToken cancellationToken)
        {
            await foreach (var result in _testEngine.TestAsync())
            {
                await Test(result);
                yield return result;
            }
        }

        private Task Test(PallyResult result)
        {
            _logger.LogInformation("Result {result}", result);
            return Task.CompletedTask;
        }

        private void ErrorOutput(IEnumerable<PallyResult> failedResults)
        {
            foreach (var fail in failedResults)
            {
                _logger.LogError("Failed Result {result}", fail);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
