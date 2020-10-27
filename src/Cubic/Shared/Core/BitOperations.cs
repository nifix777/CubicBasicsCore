namespace Cubic.Core
{
    public static class BitOperations
    {
        public static bool IsTrue(this int bitArray, int singleBit)
        {
            return (bitArray & singleBit) == singleBit;
        }

        public static int SetBit(this int bitArray, int setValue, bool isTrue)
        {
            int num;
            num = (!isTrue ? bitArray & ~setValue : bitArray | setValue);
            return num;
        }

        public static int ToggleBit(this int bitArray, int setValue)
        {
            return bitArray ^ setValue;
        }
    }
}