using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
namespace challenge1
{

    
    
    public class PalindromeTestEngine : IPalindromeTestEngine
    {
        private readonly IEnumerable<IIsPalindrome> _checkers;
        private readonly IPalindromeLoader _loader;
        private readonly ILogger<PalindromeTestEngine> _logger;
        public PalindromeTestEngine(
            IEnumerable<IIsPalindrome> checkers,
            IPalindromeLoader loader,
            ILogger<PalindromeTestEngine> logger)
        {
            _checkers = checkers;
            _loader = loader;
            _logger = logger;
        }

        public async IAsyncEnumerable<PallyResult> TestAsync(IIsPalindrome checker)
        {
            var checkerName = checker.GetType().FullName;
            await foreach (var fileAndLine in _loader.GetPalindromesAsync())
            {
                _logger.LogInformation("Testing {pally} with checker {checker}", fileAndLine.Line, checkerName);
                var sw = Stopwatch.StartNew();
                var check = checker.IsPalindrome(fileAndLine.Line);
                sw.Stop();
                _logger.LogInformation("Tested {pally} with checker {checker} in {elapsedMs}ms", fileAndLine.Line, checkerName, sw.ElapsedMilliseconds);
                yield return new PallyResult(checkerName, fileAndLine.File, fileAndLine.Line, check, sw.Elapsed);
            }
        }

        public async IAsyncEnumerable<PallyResult> TestAsync()
        {
            foreach (var checker in _checkers)
            {
                 var checkerName = checker.GetType().FullName;
                _logger.LogInformation("Starting with checker {checker}", checkerName);
                var sw = Stopwatch.StartNew();
                await foreach(var result in TestAsync(checker))
                {
                    yield return result;
                }
                sw.Stop();
                _logger.LogInformation("Finished with checker {checker} in {elapsedMs}ms", checkerName, sw.ElapsedMilliseconds);
            }
        }

    }
}
