namespace AOCHelpers
{
    public static class DupdobMath
    {
        public static long RoundedUpDiv(long toDiv, long divisor)
        {
            if (toDiv == 0)
            {
                return 0;
            }
            if (toDiv < divisor)
            {
                return 1;
            }
            return toDiv / divisor + (((toDiv % divisor) == 0) ?  0 : 1);
        }
    }
}