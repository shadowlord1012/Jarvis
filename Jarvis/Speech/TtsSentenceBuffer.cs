using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.Speech
{
    public sealed class TtsSentenceBuffer
    {
        private readonly StringBuilder _buffer = new();

        public void Append(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                _buffer.Append(text);
            }
        }

        public bool TryGetSentence(
            out string sentence)
        {
            sentence = string.Empty;

            var text = _buffer.ToString();

            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            var punctuationIndex =
                FindSentenceEnd(text);

            if (punctuationIndex < 0)
            {
                return false;
            }

            sentence =
                text[..(punctuationIndex + 1)]
                    .Trim();

            _buffer.Remove(
                0,
                punctuationIndex + 1);

            return !string.IsNullOrWhiteSpace(
                sentence);
        }

        public string Flush()
        {
            var result =
                _buffer.ToString().Trim();

            _buffer.Clear();

            return result;
        }

        private static int FindSentenceEnd(
            string text)
        {
            for (var i = 0; i < text.Length; i++)
            {
                if (text[i] is '.' or '!' or '?' &&
                    (i + 1 >= text.Length ||
                     char.IsWhiteSpace(text[i + 1])))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
