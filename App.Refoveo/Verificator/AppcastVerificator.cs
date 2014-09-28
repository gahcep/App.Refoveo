using System.IO;
using System.Xml;
using App.Refoveo.Shared;

namespace App.Refoveo.Verificator
{
    public static class AppcastVerificator
    {
        public static class WithoutSchema
        {
            public static class WithXmlInMemory
            {
                public static bool IsValid(string xmlContent)
                {
                    return XmlVerificator.InMemory.IsValid(xmlContent);
                }
            }

            public static class WithXmlInFile
            {
                public static bool IsValid(string xmlFilePath)
                {
                    return XmlVerificator.InFile.IsValid(xmlFilePath);
                }
            }
        }

        public static class WithSchemaInFile
        {
            public static class WithXmlInMemory
            {
                public static bool IsValid(string xmlContent, string xsdFilePath, out XmlErrorRecord parseStatus)
                {
                    var xmlParseResult = new XmlErrorRecord();

                    parseStatus = xmlParseResult;

                    // Check XML first
                    if (!XmlVerificator.InMemory.IsValid(xmlContent))
                        return false;

                    // Check XML Schema then
                    var funcResult = XmlSchemaVerificator.InFile.IsValid(xsdFilePath, out xmlParseResult);
                    if (!funcResult || xmlParseResult.IsDirty)
                        return false;

                    // Check XML against Schema
                    var xmlsettings = new XmlReaderSettings();
                    xmlsettings.ValidationType = ValidationType.Schema;
                    xmlsettings.Schemas.Add("http://www.w3schools.com", xsdFilePath);
                    xmlsettings.ValidationEventHandler += (sender, args) =>
                    {
                        if (xmlParseResult.IsDirty)
                            return;

                        xmlParseResult.Set(
                            args.Exception,
                            args.Severity,
                            args.Message,
                            args.Exception.LineNumber,
                            args.Exception.LinePosition);
                    };

                    var xmlreader = XmlReader.Create(new StringReader(xmlContent), xmlsettings);

                    // Check line by line
                    while (xmlreader.Read()) { }

                    return !xmlParseResult.IsDirty;
                }
            }

            public static class WithXmlInFile
            {
                public static bool IsValid(string xmlFilePath, string xsdFilePath, out XmlErrorRecord parseStatus)
                {
                    // Check XML first
                    if (!XmlVerificator.InFile.IsValid(xmlFilePath))
                    {
                        parseStatus = new XmlErrorRecord();
                        return false;
                    }

                    return WithXmlInMemory.IsValid(
                        new StreamReader(xmlFilePath).ReadToEnd(), xsdFilePath, out parseStatus);
                }
            }
        }

        public static class WithSchemaInMemory
        {
            public static class WithXmlInMemory
            {
                public static bool IsValid(string xmlContent, string xsdContent, out XmlErrorRecord parseStatus)
                {
                    var xmlParseResult = new XmlErrorRecord();

                    parseStatus = xmlParseResult;

                    // Check XML first
                    if (!XmlVerificator.InMemory.IsValid(xmlContent))
                        return false;

                    // Check XML Schema then
                    var funcResult = XmlSchemaVerificator.InMemory.IsValid(xsdContent, out xmlParseResult);
                    if (!funcResult || xmlParseResult.IsDirty)
                        return false;

                    // Check XML against Schema
                    var xmlsettings = new XmlReaderSettings();
                    xmlsettings.ValidationType = ValidationType.Schema;
                    xmlsettings.Schemas.Add("http://www.w3schools.com",
                        XmlReader.Create(new XmlTextReader(xsdContent), new XmlReaderSettings()));
                    xmlsettings.ValidationEventHandler += (sender, args) =>
                    {
                        if (xmlParseResult.IsDirty)
                            return;

                        xmlParseResult.Set(
                            args.Exception,
                            args.Severity,
                            args.Message,
                            args.Exception.LineNumber,
                            args.Exception.LinePosition);
                    };

                    var xmlreader = XmlReader.Create(new StringReader(xmlContent), xmlsettings);

                    // Check line by line
                    while (xmlreader.Read()) { }

                    return !xmlParseResult.IsDirty;
                }
            }

            public static class WithXmlInFile
            {
                public static bool IsValid(string xmlFilePath, string xsdContent, out XmlErrorRecord parseStatus)
                {
                    // Check XML first
                    if (!XmlVerificator.InFile.IsValid(xmlFilePath))
                    {
                        parseStatus = new XmlErrorRecord();
                        return false;
                    }

                    return WithXmlInMemory.IsValid(
                        new StreamReader(xmlFilePath).ReadToEnd(), xsdContent, out parseStatus);
                }
            }
        }
    }
}
