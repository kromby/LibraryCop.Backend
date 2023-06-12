using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Utils
{
    internal class ISBNHelper
    {
        public static bool IsValid(string isbn)
        {
            if (isbn.Length == 10)
                return VerifyISBN10(isbn);
            else if (isbn.Length == 13)
                return VerifyISBN13(isbn);
            else return false;
        }

        private static bool VerifyISBN10(string isbn)
        {
            // Remove any non-numeric characters from the ISBN
            isbn = new string(isbn.Where(char.IsDigit).ToArray());

            // Calculate the check digit
            int sum = 0;
            for (int i = 0; i < 9; i++)
            {
                int digit = isbn[i] - '0';
                sum += (i + 1) * digit;
            }
            int checkDigit = sum % 11;

            // Compare the calculated check digit to the actual check digit
            if (checkDigit == 10)
            {
                return (isbn[9] == 'X');
            }
            else
            {
                return (isbn[9] - '0') == checkDigit;
            }
        }

        private static bool VerifyISBN13(string isbn)
        {
            // Remove any non-numeric characters from the ISBN
            isbn = new string(isbn.Where(char.IsDigit).ToArray());

            // Calculate the check digit
            int sum = 0;
            for (int i = 0; i < 12; i++)
            {
                int digit = isbn[i] - '0';
                sum += (i % 2 == 0) ? digit : 3 * digit;
            }
            int checkDigit = (10 - (sum % 10)) % 10;

            // Compare the calculated check digit to the actual check digit
            return (isbn[12] - '0') == checkDigit;
        }
    }
}
