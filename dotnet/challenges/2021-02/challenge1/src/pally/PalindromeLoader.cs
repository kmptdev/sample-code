using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace challenge1
{
    public class PalindromeLoader : IPalindromeLoader
    {
        private readonly ILogger<PalindromeLoader> _logger;
        public  PalindromeLoader(ILogger<PalindromeLoader> logger)
        {
            _logger = logger;
        }

        public  async IAsyncEnumerable<FileAndLine> GetPalindromesAsync()
        {
            var fileCount = 0;
            var sw = Stopwatch.StartNew();
            foreach (var file in Directory.EnumerateFiles(Environment.CurrentDirectory, "*.txt"))
            {
                fileCount++;
                await foreach (var line in GetPalindromesAsync(file))
                {
                    yield return new FileAndLine(file, line);
                }
            }
             sw.Stop();
            _logger.LogInformation("Finished with {fileCount} files in {elaspedMs}ms", fileCount, sw.ElapsedMilliseconds);
        }

        public async IAsyncEnumerable<string> GetPalindromesAsync(string file)
        {
            _logger.LogInformation("Starting with file {file}", file);
            var sw = Stopwatch.StartNew();
            using StreamReader reader = File.OpenText(file);
            while (!reader.EndOfStream)
                yield return await reader.ReadLineAsync();
            sw.Stop();
            _logger.LogInformation("Finished with file {file} in {elaspedMs}ms", file, sw.ElapsedMilliseconds);
        }

   
    }
}
