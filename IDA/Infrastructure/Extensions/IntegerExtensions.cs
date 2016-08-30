namespace ConceptONE.Infrastructure.Extensions
{
    public static class IntegerExtensions
    {
        public static string Pluralize(this int count)
        {
            if (count == 1)
                return "";
            else
                return "s";
        }
    }
}
