using System;
using System.Text;
using System.Text.RegularExpressions;

namespace m.transport
{
    public class ScannerUtilities
    {

        private static char[] ignoreChar = new[] { 'I', 'O', 'Q' };

        public static void ValidateInput(string input, out string message)
        {
            Regex rg = new Regex(@"^[a-zA-Z0-9]*$");
            if (!rg.IsMatch(input))
            {
                message = "Error - Please enter letters or numbers";
                return;
            }

            rg = new Regex(@"^[ioqIOQ]*$");

            char c = input[input.Length - 1];

            if (rg.IsMatch(c.ToString()))
            {
                message = "Error - VIN can not accept '"  + c + "' character";
                return;
            }

            message = string.Empty;
        }

        public static void ValidateVIN(string inputVIN, out string cleanVIN, out string message)
        {
            string oldVIN = inputVIN;
            cleanVIN = oldVIN.ToUpper();
            message = string.Empty;

            if (oldVIN.Length < 8)
            {
                message = "Error - Please enter at least 8 characters!";
                return;
            }
            else if (oldVIN.Length < 30)
            {

                int frontIndex = oldVIN.IndexOfAny(ignoreChar);

                if (oldVIN.Length - frontIndex < 7) 
                {
                    message = "Please enter a valid VIN";
                    return;
                }
                     
                if (frontIndex > 7)
                    frontIndex = 0;
                else
                    frontIndex++;

                int endIndex = oldVIN.LastIndexOfAny(ignoreChar);

                if (oldVIN.Length - endIndex > 8)
                {
                    cleanVIN = oldVIN.Substring(frontIndex);
                }
                else
                {
                    cleanVIN = oldVIN.Substring(frontIndex, endIndex - frontIndex);
                }

                if (cleanVIN.Trim().Length > 17)
                {
                    cleanVIN = cleanVIN.Substring(cleanVIN.Length - 17);
                }
            }
            else
            {
                bool foundVIN = false;
                //handling QR code case
                string[] strs = inputVIN.Split(',');

                foreach (String s in strs)
                {
                    string tmp = s.Trim().ToUpper();

                    if (tmp.Length == 17 && tmp.LastIndexOfAny(ignoreChar) == -1)
                    {
                        cleanVIN = tmp;
                        foundVIN = true;
                        break;
                    }
                }

                if (!foundVIN)
                {
                    message = "invalid QR code";
                }

            }
        }
    }
}

