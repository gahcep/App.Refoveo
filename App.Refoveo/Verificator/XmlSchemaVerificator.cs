using System.IO;
using System.Xml.Schema;
using App.Refoveo.Shared;

namespace App.Refoveo.Verificator
{
    public static class XmlSchemaVerificator
    {
        public static class InMemory
        {
            public static bool IsValid(string xsdContent, out XmlErrorRecord xsdParseStatus)
            {
                var xsdParseResult = new XmlErrorRecord();

                xsdParseStatus = xsdParseResult;

                StringVerificator.IsValid(xsdContent, validOrThrow: true);

                try
                {
                    var xsdReader = new StringReader(xsdContent);

                    XmlSchema.Read(xsdReader, (sender, args) =>
                    {
                        if (xsdParseResult.IsDirty)
                            return;

                        xsdParseResult.Set(
                            args.Exception,
                            args.Severity,
                            args.Message,
                            args.Exception.LineNumber,
                            args.Exception.LinePosition);
                    });

                    return !xsdParseResult.IsDirty;
                }
                catch (XmlSchemaException)
                {
                    return false;
                }
            }
        }

        public static class InFile
        {
            public static bool IsValid(string xsdFilePath, out XmlErrorRecord xsdParseStatus)
            {
                var xsdParseResult = new XmlErrorRecord();

                xsdParseStatus = xsdParseResult;

                StringVerificator.IsValid(xsdFilePath, validOrThrow: true);

                if (!FileVerificator.IsFilePathValid(xsdFilePath) ||
                    !FileVerificator.FileExists(xsdFilePath))
                    return false;

                try
                {
                    var xsdReader = new StringReader(new StreamReader(xsdFilePath).ReadToEnd());

                    XmlSchema.Read(xsdReader, (sender, args) =>
                    {
                        if (xsdParseResult.IsDirty)
                            return;

                        xsdParseResult.Set(
                            args.Exception,
                            args.Severity,
                            args.Message,
                            args.Exception.LineNumber,
                            args.Exception.LinePosition);
                    });

                    return !xsdParseResult.IsDirty;
                }
                catch (XmlSchemaException)
                {
                    return false;
                }
            }
        }
    }
}
