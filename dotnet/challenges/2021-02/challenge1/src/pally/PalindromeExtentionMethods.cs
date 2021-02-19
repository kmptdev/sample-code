using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
namespace challenge1
{
    public static class PalindromeExtentionMethods
    {
        public static IEnumerable<Type> GetCheckers(ILogger logger)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

            foreach (var assembly in assemblies)
                logger.LogInformation("Assembly found {assembly}", assembly.FullName);

            var assemblyTypes = assemblies.SelectMany(x => x.GetTypes()).ToList();

            foreach (var assemblyType in assemblyTypes)
                logger.LogInformation("Type found {type}", assemblyType.FullName);

            var isPTypes = assemblyTypes.Where(x => typeof(IIsPalindrome).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract).Select(x => x).ToList();

            foreach (var assemblyType in isPTypes)
                logger.LogInformation("IIsPalindrome Type found {type}", assemblyType.FullName);

            return isPTypes;
        }

        private static ILoggerFactory LoggerOrNull(ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
            {
                return new Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory();
            }
            else
            {
                return loggerFactory;
            }
        }

        public static IServiceCollection AddPalindrome(this IServiceCollection services, Action<PalindromeOptions> options)
        {
            return AddPalindrome(services, options, (ILoggerFactory)null);
        }

        public static IServiceCollection AddPalindrome(this IServiceCollection services, Action<PalindromeOptions> options, ILoggerFactory loggerFactory)
        {
            var nonNullLogger = LoggerOrNull(loggerFactory);
            var logger = nonNullLogger.CreateLogger("PalindromeExtentionMethods");
            return AddPalindrome(services, options, logger);
        }

        public static IServiceCollection AddPalindrome(this IServiceCollection services, Action<PalindromeOptions> options, ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            var checkers = BuildCheckers(services, options, logger);
            AddCheckers(services, checkers, logger);
            AddLoader(services, checkers, logger);
            AddTestEngine(services, checkers, logger);
            return services;
        }
        private static void AddLoader(IServiceCollection services, PalindromeOptions checkers, ILogger logger)
        {
            logger.LogInformation("Adding loader {loader}", checkers.PalindromeLoader.FullName);
            services.AddTransient(typeof(IPalindromeLoader), checkers.PalindromeLoader);
        }

        private static void AddTestEngine(IServiceCollection services, PalindromeOptions checkers, ILogger logger)
        {
            logger.LogInformation("Adding test engine {testEngine}", typeof(PalindromeTestEngine).FullName);
            services.AddTransient<IPalindromeTestEngine, PalindromeTestEngine>();
        }

        private static PalindromeOptions BuildCheckers(IServiceCollection services, Action<PalindromeOptions> options, ILogger logger)
        {
            var checkers = new PalindromeOptions(PalindromeExtentionMethods.GetCheckers(logger), logger);
            options(checkers);
            return checkers;
        }
        private static void AddCheckers(IServiceCollection services, PalindromeOptions checkers, ILogger logger)
        {
            foreach (var checker in checkers.PalindromeCheckers)
            {
                logger.LogInformation("Adding checker {checker}", checker.FullName);
                services.AddTransient(typeof(IIsPalindrome), checker);
            }
        }
    }
}
