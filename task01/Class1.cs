namespace task01;

public static class StringExtensions
{
    public static bool IsPalindrome(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return false;
        }

        var lowerCase = input.ToLower();
            
        var filtered = new string(lowerCase
            .Where(c => !char.IsPunctuation(c) && !char.IsWhiteSpace(c))
            .ToArray());
        
        var reversed = new string(filtered.Reverse().ToArray());
        
        return filtered == reversed;
    }
}