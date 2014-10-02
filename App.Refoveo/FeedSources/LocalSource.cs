using System;
using System.IO;
using System.Text;
using App.Refoveo.Abstractions;
using App.Refoveo.Shared;
using App.Refoveo.Verificator;

namespace App.Refoveo.FeedSources
{
    public class LocalSource : IFeedSource
    {
        // LOCAL path:
        // Wrong: [file://r:/...]
        // Wrong: [//r:/...]
        // Valid: [r:/...]

        public Encoding Encoding { get; set; }

        // appversion.xml
        public string AppVerFile { get; set; }
        public long AppVerFileSizeLimit { get; set; }

        // appversion.xsd
        public string AppVerSchema { get; set; }
        public long AppVerSchemaSizeLimit { get; set; }

        // appcast.xml
        public string AppCastFile { get; set; }
        public long AppCastFileSizeLimit { get; set; }

        // appcast.xsd
        public string AppCastSchema { get; set; }
        public long AppCastSchemaSizeLimit { get; set; }

        public LocalSource()
        {
            /* Default values */

            AppVerFile = String.Empty;
            AppVerSchema = String.Empty;
            AppCastFile = String.Empty;
            AppCastSchema = String.Empty;

            // 1Mb
            AppVerFileSizeLimit = 10 * 1024 * 1024;
            AppVerSchemaSizeLimit = 10 * 1024 * 1024;
            // 10Mb
            AppCastFileSizeLimit = 10 * 1024 * 1024;
            AppCastSchemaSizeLimit = 10 * 1024 * 1024;

            Encoding = Encoding.ASCII;
        }

        private bool ValidateAppCastFile()
        {
            var appcastFileExists = !String.IsNullOrEmpty(AppCastFile.Trim());
            var appcastSchemaExists = !String.IsNullOrEmpty(AppCastSchema.Trim());

            if (!appcastFileExists)
                return false;

            // Check AppCast file size
            if (FileVerificator.Size.GreaterThan(AppCastFile, AppCastFileSizeLimit))
                return false;

            if (appcastSchemaExists)
            {
                // Check AppCast schema size
                if (FileVerificator.Size.GreaterThan(AppCastSchema, AppCastSchemaSizeLimit))
                    return false;

                XmlErrorRecord error;
                var funcResult = AppcastVerificator.WithSchemaInFile.WithXmlInFile.IsValid(
                    AppCastFile, AppCastSchema, out error);

                return funcResult && !error.IsDirty;
            }

            return AppcastVerificator.WithoutSchema.WithXmlInFile.IsValid(AppCastFile);
        }

        private bool ValidateAppVerFile()
        {
            var appverFileExists = !String.IsNullOrEmpty(AppVerFile.Trim());
            var appverSchemaExists = !String.IsNullOrEmpty(AppVerSchema.Trim());

            if (!appverFileExists)
                return false;

            // Check AppVer file size
            if (FileVerificator.Size.GreaterThan(AppVerFile, AppVerFileSizeLimit))
                return false;

            if (appverSchemaExists)
            {
                // Check AppVer schema size
                if (FileVerificator.Size.GreaterThan(AppVerSchema, AppVerSchemaSizeLimit))
                    return false;

                XmlErrorRecord error;
                var funcResult = AppcastVerificator.WithSchemaInFile.WithXmlInFile.IsValid(
                    AppVerFile, AppVerSchema, out error);

                return funcResult && !error.IsDirty;
            }

            return AppcastVerificator.WithoutSchema.WithXmlInFile.IsValid(AppVerFile);
        }

        /* Returns its FeedSource type */

        public FeedSourceType IdentifyItself()
        {
            return FeedSourceType.FeedSourceLocal;
        }

        /* Returns appcast.xml contents */

        public string AppcastAsString()
        {
            if (!ValidateAppCastFile())
                return String.Empty;

            // Using StreamReader instead of File.ReadAllText allows us to set file encoding
            string content;
            using (var streamreader = new StreamReader(AppCastFile, true))
            {
                Encoding = streamreader.CurrentEncoding;
                content = streamreader.ReadToEnd();
            }

            return content;
        }

        public MemoryStream AppcastAsStream()
        {
            return new MemoryStream(Encoding.GetBytes(AppCastFile));
        }

        /* Returns appversion.xml */

        public string AppversionAsString()
        {
            if (!ValidateAppVerFile())
                return String.Empty;

            // Using StreamReader instead of File.ReadAllText allows us to set file encoding
            string content;
            using (var streamreader = new StreamReader(AppVerFile, true))
            {
                Encoding = streamreader.CurrentEncoding;
                content = streamreader.ReadToEnd();
            }

            return content;
        }

        public MemoryStream AppversionAsStream()
        {
            return new MemoryStream(Encoding.GetBytes(AppVerFile));
        }

        /* Sets whether compression is used or not */

        public void CompressionUsed(bool forAppcast, bool forAppversion)
        {
            throw new NotImplementedException();
        }
    }
}
