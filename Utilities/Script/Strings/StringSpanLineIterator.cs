using System.Collections;
using System.Collections.Generic;

namespace SZUtilities
{
    public struct StringSpanlineIterator : IEnumerator<StringSpan>
    {
        public readonly StringSpan Span;

        private int m_start;
        private StringSpan m_current;

        public StringSpanlineIterator(StringSpan span)
            : this()
        {
            Span = span;
        }

        public void Reset()
        {
            m_start = 0;
            m_current = new StringSpan();
        }

        public bool MoveNext()
        {
            if (m_current.Valid)
            {
                var end = m_start + m_current.Length;
                if (end >= Span.Length)
                    return false;

                if (Span[end] == '\r')
                    ++m_start;

                m_start += m_current.Length + 1;
            }
            else
            {
                m_start = 0;
            }

            if (Span.Find('\n', m_start) is { } endl && endl >= 0)
            {
                if (endl > 0 && Span[endl - 1] == '\r')
                    --endl;

                m_current = new StringSpan(Span, m_start, endl - m_start);
                return true;
            }
            else if (!m_current.Valid)
            {
                m_current = new StringSpan(Span);
                return true;
            }

            return false;
        }

        public StringSpan Current => m_current;

        object IEnumerator.Current => m_current;

        public void Dispose()
        { }
    }
}
