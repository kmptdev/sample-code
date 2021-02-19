using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace challenge1
{
    public interface IIsPalendrome
    {
        bool IsPalendrome(string inputString);
    }

    public class Palendromes
    {
        public static  async IAsyncEnumerable<string> GetData()
        {
            foreach (var file in Directory.EnumerateFiles(Environment.CurrentDirectory, "*.txt"))
            {
                await foreach (var line in ReadLines(file))
                {
                    yield return line;
                }
            }
        }

        public static async IAsyncEnumerable<string> ReadLines(string file)
        {
            using StreamReader reader = File.OpenText(file);
            while (!reader.EndOfStream)
                yield return await reader.ReadLineAsync();
        }

        public async Task Test(IIsPalendrome checker)
        {
            await foreach(var line in GetData())
            {
                var check = checker.IsPalendrome(line);
            }
        }
    }

    public class TestEngine
    {
        public IEnumerable<IIsPalendrome> GetCheckers()
        {
            return (IEnumerable<IIsPalendrome>)AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                .Where(x => typeof(IIsPalendrome).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Select(x => x.Name).ToList();
        }
        
        public async Task Test1()
        {
            var p = new Palendromes();
            foreach(var checker in GetCheckers())
            {
                await p.Test(checker);
            }
        }
    }

    public class IsPalendromeChecker : IIsPalendrome
    {

        public bool IsPalendrome(string inputString)
        {
            var clean = Regex.Replace(inputString, @"[^\w]", string.Empty); 
            var reversed = (string)clean.Reverse();
            return clean == reversed;
        }
    }
}
