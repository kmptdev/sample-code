using System.Collections.Generic;

namespace challenge1
{
    public record FileAndLine(string File, string Line);
    public interface IPalindromeLoader
    {
       IAsyncEnumerable<FileAndLine> GetPalindromesAsync();
    }
}
