using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace challenge1
{
    public class PalindromeOptions
    {
        private List<Type> _checkers = new List<Type>();
        private readonly ILogger _logger;
        public PalindromeOptions(IEnumerable<Type> checkers, ILogger logger)
        {
            _logger = logger;

            foreach(var checker in checkers)
            {
                Add(checker);
            }
            
            Loader<PalindromeLoader>();
        }

        public IEnumerable<Type> PalindromeCheckers =>_checkers;

        public Type PalindromeLoader {get;private set;}

        public PalindromeOptions Loader<T>() where T : class, IPalindromeLoader
        {
            _logger.LogInformation("Adding loader to options {loader}", typeof(T).FullName);
            PalindromeLoader = typeof(T);
            return this;
        }

        public PalindromeOptions Add<T>() where T : class, IIsPalindrome
        {
            Add(typeof(T));
            return this;
        }

        private void Add(Type t)
        {
            _logger.LogInformation("Adding IIsPalindrome type to options {logger}", t.FullName);
              _checkers.Add(t);
        }
    }
}
