namespace Domain.Helpers
{
    public static class PageHelper
    {
        public static int CalculateTotalPagesCount(int elementsCount, int countPerPage)
            => (int)Math.Round((double)elementsCount / countPerPage, 0, MidpointRounding.ToPositiveInfinity);
    }
}
