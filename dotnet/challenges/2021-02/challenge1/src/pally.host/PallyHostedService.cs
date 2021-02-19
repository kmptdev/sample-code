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

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var fails = new List<PallyResult>();
            await foreach(var result in _testEngine.TestAsync())
            {
                _logger.LogInformation("Result {result}", result);
                var fileInfo = new FileInfo(result.fileName);
                var fileName = fileInfo.Name.ToLower();
                if(fileName.Contains("true") && !result.result)
                {
                    fails.Add(result);
                }
                else if(fileName.Contains("false") && result.result)
                {
                    fails.Add(result);
                }
            }

            foreach(var fail in fails)
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
