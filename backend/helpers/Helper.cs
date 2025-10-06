using System.Text.RegularExpressions;

namespace FalveyInsuranceGroup.Backend.Helpers
{
    public class Helper
    {
        // Check if state code is from enumerated list.
        public static Boolean checkStateCode(string stateCode)
        {
            Boolean stringMatches = false;

            // 50 state codes, District of Columbia, and five US territories
            string[] stateCodeLibrary = {
                "AL", "AK", "AZ", "AR", "CA", "CO", "CT", "DE", "FL", "GA",
                "HI", "ID", "IL", "IN", "IA", "KS", "KY", "LA", "ME", "MD",
                "MA", "MI", "MN", "MS", "MO", "MT", "NE", "NV", "NH", "NJ",
                "NM", "NY", "NC", "ND", "OH", "OK", "OR", "PA", "RI", "SC",
                "SD", "TN", "TX", "UT", "VT", "VA", "WA", "WV", "WI", "WY",
                "DC", "AS", "GU", "MP", "PR", "VI"
                };
            for (int i = 0; i < stateCodeLibrary.Length; ++i)
                if (stateCode == stateCodeLibrary[i])
                {
                    stringMatches = true;
                }
            return stringMatches;
        }

        // Checks if input is only whitespace. 
        public static Boolean checkWhitespace(string input)
        {
            return Regex.IsMatch(input, @"^\s*$");
        }

    }
}
