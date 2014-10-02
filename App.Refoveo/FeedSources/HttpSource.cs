using System;
using System.IO;
using App.Refoveo.Abstractions;
using App.Refoveo.Shared;

namespace App.Refoveo.FeedSources
{
    public class HttpSource : IFeedSource
    {
        /* Returns its FeedSource type */

        public FeedSourceType IdentifyItself()
        {
            return FeedSourceType.FeedSourceHttp;
        }

        /* Returns appcast.xml contents */

        public string AppcastAsString()
        {
            throw new NotImplementedException();
        }

        public MemoryStream AppcastAsStream()
        {
            throw new NotImplementedException();
        }

        /* Returns appversion.xml */

        public string AppversionAsString()
        {
            throw new NotImplementedException();
        }

        public MemoryStream AppversionAsStream()
        {
            throw new NotImplementedException();
        }

        /* Sets whether compression is used or not */

        public void CompressionUsed(bool forAppcast, bool forAppversion)
        {
            throw new NotImplementedException();
        }
    }
}
