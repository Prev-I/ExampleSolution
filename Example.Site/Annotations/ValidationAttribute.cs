using System.ComponentModel.DataAnnotations;


namespace Example.Site.Annotations
{
    public class EmailAttribute : RegularExpressionAttribute
    {
        public EmailAttribute()
            : base(GetRegex())
        { }

        private static string GetRegex()
        {
            return System.Configuration.ConfigurationManager.AppSettings["validation.Email"];
        }
    }

    public class CodFiscaleAttribute : RegularExpressionAttribute
    {
        public CodFiscaleAttribute()
            : base(GetRegex())
        { }

        private static string GetRegex()
        {
            return System.Configuration.ConfigurationManager.AppSettings["validation.CodFiscale"];
        }
    }

    public class NroCellulareAttribute : RegularExpressionAttribute
    {
        public NroCellulareAttribute()
            : base(GetRegex())
        { }

        private static string GetRegex()
        {
            return System.Configuration.ConfigurationManager.AppSettings["validation.NroCellulare"];
        }
    }
}
