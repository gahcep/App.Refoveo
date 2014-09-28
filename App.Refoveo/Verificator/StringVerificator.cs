using System;

namespace App.Refoveo.Verificator
{
    public static class StringVerificator
    {
        public static bool IsValid(string parameter, bool validOrThrow)
        {
            if (parameter == null)
            {
                if (validOrThrow)
                    throw new ArgumentNullException("parameter");

                return false;
            }

            parameter = parameter.Trim();

            if (Equals(String.Empty, parameter))
            {
                if (validOrThrow)
                    throw new ArgumentException("Parameter is empty");

                return false;
            }

            return true;
        }
    }
}
