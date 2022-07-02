using System;
using System.Collections.Generic;
using System.Text;

namespace SZUtilities
{
    public struct StringSpan 
        : IEquatable<StringSpan>
        , IComparable<StringSpan>
        , IComparable<string>
        , IEquatable<string>
        , IComparable
    {
        #region Fields

        private char[] m_characters;
        private int m_start;
        private int m_length;
        private int? m_hashCode;

        public StringSpan(string source)
            : this(source.ToCharArray(), 0, source.Length, null)
        { }

        public StringSpan(StringSpan span)
            : this(span.m_characters, span.m_start, span.m_length, span)
        { }

        public StringSpan(StringSpan span, int start, int length)
            : this(span.m_characters, span.m_start + start, length, span)
        { }

        public StringSpan(char[] characters)
            : this(characters, 0, characters.Length)
        { }

        public StringSpan(char[] characters, int start, int length)
            : this(characters, start, length, null)
        { }

        private StringSpan(char[] characters, int start, int length, StringSpan? from)
        {
            m_characters = characters;
            m_start = start;
            m_length = length;
            m_hashCode = null;

            if (from.HasValue && from.Value is { } fromSpan)
            {
                if (fromSpan.m_characters == m_characters && fromSpan.m_start == m_start && fromSpan.m_length == m_length)
                    m_hashCode = fromSpan.m_hashCode;
            }
        }

        #endregion

        #region Operators

        public bool Valid => null != m_characters;
        public int Length => m_length;

        public char this[int index] => m_characters[m_start + index];

        public override bool Equals(object obj)
        {
            if (obj is StringSpan span)
                return Equals(span);
            else if (obj is string str)
                return Equals(str);
            
            return false;
        }

        public override int GetHashCode()
        {
            if (m_hashCode.HasValue)
                return m_hashCode.Value;

            m_hashCode = CalculateHashCode(this);
            return m_hashCode.Value;
        }

        private static int CalculateHashCode(StringSpan span)
        {
            // Win32 default C# string hash implementation
            // https://stackoverflow.com/questions/15174477/how-is-gethashcode-of-c-sharp-string-implemented

            unsafe
            {
                fixed (char* src = span.m_characters)
                {
                    int hash1 = (0x1505 << 16) + 0x1505; 
                    int hash2 = hash1;

                    char* buffer = src + span.m_start;
                    int len = span.m_length;

                    while (len > 8)
                    {
                        int* pint = (int*)buffer;
                        hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ pint[0]; 
                        hash2 = ((hash2 << 5) + hash2 + (hash2 >> 27)) ^ pint[1];

                        buffer += 8;
                        len  -= 8;
                    }

                    bool hashPingPong = false;
                    while(len > 0)
                    {
                        char c = *buffer;
                        if (hashPingPong)
                            hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ c;
                        else
                            hash2 = ((hash2 << 5) + hash2 + (hash2 >> 27)) ^ c;

                        ++buffer;
                        --len;
                        hashPingPong = !hashPingPong;
                    }

                    return hash1 + (hash2 * 1566083941);
                }
            }
        }

        public bool Equals(StringSpan other)
        {
            if (m_length != other.Length)
                return false;

            for (var i = 0; i < other.Length; ++i)
            {
                if (this[i] != other[i])
                    return false;
            }

            return true;
        }

        public bool Equals(string other)
        {
            if (m_length != other.Length)
                return false;

            for(var i = 0; i < other.Length; ++i)
            {
                if (this[i] != other[i])
                    return false;
            }

            return true;
        }

        public int CompareTo(StringSpan other)
        {
            // ToDo: Make this comparision alphabetical and non allocating
            var debug = new DebugEx(nameof(StringSpan));
            debug.LogWarning($"Using inefficient method {nameof(CompareTo)}");

            var left = new string(m_characters, m_start, m_length);
            var right = new string(other.m_characters, other.m_start, other.m_length);
            return left.CompareTo(right);
        }

        public int CompareTo(string other)
        {
            // ToDo: Make this comparision alphabetical
            var debug = new DebugEx(nameof(StringSpan));
            debug.LogWarning($"Using inefficient method {nameof(CompareTo)}");

            var left = new string(m_characters, m_start, m_length);
            return left.CompareTo(other);
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;

            if (obj is StringSpan span)
                return CompareTo(span);
            else if (obj is string str)
                return CompareTo(str);

            return 1;
        }

        public override string ToString()
        {
            var builder = new StringBuilder(capacity: m_length);
            
            for(var i = 0; i < m_length; ++i)
                builder.Append(this[i]);
            
            return builder.ToString();
        }

        #endregion

        #region Methods

        public StringSpan Substring(int start, int length)
        {
            return new StringSpan(this, start, length);
        }

        public int Find(StringSpan text, int startIndex = 0)
        {
            if (text.m_length <= 0)
                return 0;

            var thisEnd = m_length - text.m_length + 1;
            for (var thisIt = startIndex; thisIt < thisEnd; ++thisIt)
            {
                var correct = true;
                var thisItCopy = thisIt;
                for(var textIt = 0; textIt < text.Length; ++textIt, ++thisItCopy)
                {
                    if (this[thisItCopy] == text[textIt])
                        continue;
                    
                    correct = false;
                    break;
                }

                if (correct)
                    return thisIt;
            }

            return -1;
        }

        public int Find(string str, int startIndex = 0)
        {
            if (str.Length <= 0)
                return 0;

            var thisEnd = m_length - str.Length + 1;
            for (var thisIt = startIndex; thisIt < thisEnd; ++thisIt)
            {
                var correct = true;
                var thisItCopy = thisIt;
                for (var textIt = 0; textIt < str.Length; ++textIt, ++thisItCopy)
                {
                    if (this[thisItCopy] == str[textIt])
                        continue;

                    correct = false;
                    break;
                }

                if (correct)
                    return thisIt;
            }

            return -1;
        }

        public int Find(char c, int startIndex = 0)
        {
            for (var index = startIndex; index < m_length; ++index)
            {
                if (c == this[index])
                    return index;
            }

            return -1;
        }

        public void Split(char delimiter, ref List<StringSpan> result)
        {
            result ??= new List<StringSpan>();
            result.Clear();

            for(var it = 0;;)
            {
                var end = Find(delimiter, it);

                var len = end >= 0 
                    ? end - it 
                    : m_length - it;

                result.Add(new StringSpan(this, it, len));

                if (end < 0)
                    return;

                it = end + 1;
            }
        }

        public void Trim(char delimiter)
        {
            TrimStart(delimiter);
            TrimEnd(delimiter);
        }

        public void TrimStart(char delimiter)
        {
            while (m_length > 0 && this[0] == delimiter)
            {
                ++m_start;
                --m_length;
                m_hashCode = null;
            }
        }

        public void TrimEnd(char delimiter)
        {
            while (m_length > 0 && this[m_length - 1] == delimiter)
            {
                --m_length;
                m_hashCode = null;
            }
        }

        #endregion
    }
}
