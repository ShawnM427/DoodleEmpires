using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterServer
{
    public static class Encryption
    {
        public string Encrypt(string input)
        {
            Random _random = new Random(input[0] + input[1]);

            string newString = "";
            foreach (char c in input)
            {
                int newCharValue = (int)c + _random.Next(char.MaxValue);
                newString += (char)(
                    newCharValue > char.MaxValue ? 
                        newCharValue - char.MaxValue : 
                        newCharValue < char.MinValue ? 
                            newCharValue + char.MaxValue : 
                                newCharValue);
            }

            return newString;
        }

        public string Deccrypt(string input)
        {
            Random _random = new Random(input[0] + input[1]);

            string newString = "";
            foreach (char c in input)
            {
                int newCharValue = (int)c + _random.Next(char.MaxValue);
                newString += (char)(
                    newCharValue > char.MaxValue ?
                        newCharValue - char.MaxValue :
                        newCharValue < char.MinValue ?
                            newCharValue + char.MaxValue :
                                newCharValue);
            }

            return newString;
        }
    }
}
