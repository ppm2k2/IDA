namespace ConceptONE.Infrastructure.Extensions
{
    public static class BoolExtensions
    {

        public static string ToYNString(this bool value)
        {
            if (value)
                return "Y";
            else
                return "N";
        }

        public static string ToYesNoString(this bool value)
        {
            if (value)
                return "Yes";
            else
                return "No";
        }

        public static int ToInt(this bool value)
        {
            if (value)
                return 1;
            else
                return 0;
        }

    }
}
