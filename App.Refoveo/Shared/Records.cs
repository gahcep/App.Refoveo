using System;
using System.Xml.Schema;

namespace App.Refoveo.Shared
{
    public class XmlErrorRecord
    {
        public bool IsDirty { get; private set; }

        private string message;
        private XmlSeverityType severity;
        private Exception innerException;
        private int lineNumber;
        private int linePosition;

        public void Set(Exception argInnerException, XmlSeverityType argSeverity, string argMessage,
            int argLineNumber, int argLinePosition)
        {
            innerException = argInnerException;
            severity = argSeverity;
            message = argMessage;
            lineNumber = argLineNumber;
            linePosition = argLinePosition;

            IsDirty = true;
        }
    }
}
