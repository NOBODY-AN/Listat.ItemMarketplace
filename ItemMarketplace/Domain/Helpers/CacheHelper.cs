namespace Domain.Helpers
{
    public static class CacheHelper
    {
        public static string IdBuilder<T>(params object[] args)
        {
            int[] objects = new int[args.Length + 1];
            objects[0] = typeof(T).GetHashCode();

            for (int i = 0; i < args.Length; i++)
            {
                objects[i + 1] = args[i].GetHashCode();
            }

            return string.Join(',', objects);
        }
    }
}
