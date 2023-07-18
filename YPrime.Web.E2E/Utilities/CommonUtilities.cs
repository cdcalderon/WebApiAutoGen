using System.Linq;

namespace YPrime.Web.E2E.Utilities
{
    public class CommonUtilities
    {
        public static string getDateFromDatetimeUI(string input)
        {
            string output = "";
            if (input.Contains('-'))
            {
                output = input.Split(' ')[0];
            }
            else if (input.Contains('/'))
            {
                var array = input.Split(' ')[0];
                output = array.Replace('/', '-');
            }
            else
            {
                var array =input.Split(' ');
                output = string.Join("-", array.Take(3));
            }
            return output;
        }

    }
}
