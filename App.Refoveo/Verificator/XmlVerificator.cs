using System.Xml;

namespace App.Refoveo.Verificator
{
    public static class XmlVerificator
    {
        public static class InMemory
        {
            public static bool IsValid(string xmlContent)
            {
                StringVerificator.IsValid(xmlContent, validOrThrow: true);

                try
                {
                    // Sanity check on XML loading
                    (new XmlDocument()).LoadXml(xmlContent);
                }
                catch (XmlException)
                {
                    return false;
                }

                return true;
            }
        }

        public static class InFile
        {
            public static bool IsValid(string xmlFilePath)
            {
                StringVerificator.IsValid(xmlFilePath, validOrThrow: true);

                if (!FileVerificator.IsFilePathValid(xmlFilePath) ||
                    !FileVerificator.FileExists(xmlFilePath))
                    return false;

                try
                {
                    // Sanity check on XML loading
                    (new XmlDocument()).Load(xmlFilePath);
                }
                catch (XmlException)
                {
                    return false;
                }

                return true;
            }
        }
    }

    
}
