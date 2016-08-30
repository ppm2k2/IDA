using System.Collections.Generic;
using System.Linq;

namespace IDADataAccess.DEMI
{
    class LookupItem
    {

        //Sample: Lookup([FormPF_Mapping].[dbo].[Gruss_Map_SP_Rtg], Quality, SP_Rtg, [S&P Bond Rating])
        private const int LOOKUP_TABLE = 0;
        private const int LOOKUP_FIELD = 1;
        private const int WHERE_NAME = 2;
        private const int WHERE_VALUE = 3;

        internal string LookupTable { get; set; }
        internal string LookupField { get; set; }
        internal string WhereName { get; set; }
        internal string WhereValue { get; set; }
        internal int Id { get; set; }

        private char[] _CharsToTrim = new char[] { ' ', '"' };

        internal LookupItem(string lookupString)
        {
            string parameterString = lookupString.Substring("Lookup(".Length).Trim(')');
            List<string> parameters = parameterString.Split(',').ToList();

            LookupTable = parameters[LOOKUP_TABLE].Trim(_CharsToTrim);
            LookupField = parameters[LOOKUP_FIELD].Trim(_CharsToTrim);
            WhereName = parameters[WHERE_NAME].Trim(_CharsToTrim);
            WhereValue = parameters[WHERE_VALUE].Trim(_CharsToTrim);
        }

    }
}
