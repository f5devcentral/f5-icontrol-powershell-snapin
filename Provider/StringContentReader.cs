using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation.Provider;
using System.Collections;
using System.Collections.ObjectModel;

namespace iControlSnapIn.Provider
{
    public class StringContentReader : IContentReader
    {
        private string _value;
        private long _curOffset = 0;
        Collection<string> _lines = new Collection<string>();

        public StringContentReader()
        {
        }

        public StringContentReader(string str)
        {
            _value = str;
            System.IO.StringReader sr = new System.IO.StringReader(str);
            string s = null;
            while (null != (s = sr.ReadLine()))
            {
                _lines.Add(s);
            }
        }

        #region IContentReader Members

        public void Close()
        {
            //throw new Exception("The method or operation is not implemented.");
        }

        public System.Collections.IList Read(long readCount)
        {
            IList result = null;

            if ((_curOffset < 0) || (_curOffset >= _value.Length))
            {
                // do nothing, and return null;
            }
            else
            {
                Collection<string> list = new Collection<string>();

                for (int i=(int)_curOffset, c=0; (i<_lines.Count) && (c<readCount); i++, c++)
                {
                    list.Add(_lines[i]); 
                }

                result = list;

                _curOffset += readCount;
            }
            return result;
       }

        public void Seek(long offset, System.IO.SeekOrigin origin)
        {
            switch (origin)
            {
                case System.IO.SeekOrigin.Begin:
                    _curOffset = offset;
                    break;
                case System.IO.SeekOrigin.Current:
                    _curOffset += offset;
                    break;
                case System.IO.SeekOrigin.End:
                    _curOffset = _value.Length - offset;
                    if (_curOffset < 0) { _curOffset = 0; }
                    break;
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
