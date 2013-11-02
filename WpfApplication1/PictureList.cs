using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml.Linq;

namespace PicViewer
{
    class PictureList
    {
        XNamespace rss = "http://search.yahoo.com/mrss/";

        string[] _PicList = null;
        int _Current = 0;

        internal void Init(string path)
        {
            try
            {
                // todo:  exercise for the user, make the path dynamic
                _PicList = System.IO.Directory.GetFiles(path, "*.jpg");
                
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        internal string Peek()
        {
            return _PicList[_Current];
        }

        internal string Prev()
        {
            --_Current;
            if (_Current < 0)
                _Current = _PicList.Length - 1;
            return Peek();
        }

        internal string Next()
        {
            ++_Current;
            if (_Current >= _PicList.Length)
                _Current = 0;
            return Peek();
        }

        internal void LoadFromRss(string url)
        {
            try
            {
                WebClient wc = new WebClient();
                string text = wc.DownloadString(url);
                XDocument xml = XDocument.Parse(text);
                IEnumerable<string> pictures = from href in xml.Descendants(rss + "thumbnail").Attributes("url")
                                               select href.Value;
                _PicList = pictures.ToArray();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
    }
}
