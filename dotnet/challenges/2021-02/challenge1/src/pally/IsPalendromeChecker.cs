using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
namespace challenge1
{
    public class IsPalendromeChecker : IIsPalindrome
    {
        private readonly ILogger<IsPalendromeChecker> _logger;
        public IsPalendromeChecker(ILogger<IsPalendromeChecker> logger)
        {
            _logger = logger;
        }
    
        public bool IsPalindrome(string inputString)
        {
            var clean = Regex.Replace(inputString, @"[^\w]", string.Empty).ToLower();
            var reversed = new String(clean.Reverse().ToArray()).ToString();
            var isPally = clean == reversed;
            _logger.LogInformation("Input: {input}\tCleaned input: {clean}\tReversed: {reversed}\tIsPalindrome: {isPally}", inputString, clean, reversed, isPally);
            return isPally;
        }
    }
}
