using System.IO;
using App.Refoveo.Shared;

namespace App.Refoveo.Abstractions
{
    public interface IFeedSource
    {
        /* Returns its FeedSource type */
        FeedSourceType IdentifyItself();

        /* Returns appcast.xml contents */
        string AppcastAsString();
        MemoryStream AppcastAsStream();

        /* Returns appversion.xml */
        string AppversionAsString();
        MemoryStream AppversionAsStream();

        /* Sets whether compression is used or not */
        void CompressionUsed(bool forAppcast, bool forAppversion);
    }
}
