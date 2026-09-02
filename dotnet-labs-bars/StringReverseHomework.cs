using System;
using System.Collections.Generic;
using System.Text;

namespace dotnet_labs_bars
{
    public static class StringReverseHomework
    {
        public static string ReverseWord(string word)
        {
            return new string(word.ToArray().Reverse().ToArray());
        }

        public static string ReverseSentence(string sentence)
        {
            string[] words = sentence.Split(' ');

            for (int i = 0; i < words.Length; i++)
            {
                words[i] = new string(words[i].ToArray().Reverse().ToArray());
            }

            return string.Join(' ', words);
        }
    }
}
