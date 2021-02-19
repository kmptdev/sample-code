using System.Collections.Generic;
using System.Threading.Tasks;
namespace challenge1
{
    public interface IPalindromeTestEngine
    {
        IAsyncEnumerable<PallyResult> TestAsync(IIsPalindrome checker);
        IAsyncEnumerable<PallyResult> TestAsync();
    }
}
