namespace FinalChallenge
{
    public class StudentHelper
    {
        public static double HitungRata(int[] nilai)
        {
            int total = 0;
            for (int i =0; i <nilai.Length; i++)
            {
                total += nilai[i];
            }
            return (double)total / nilai.Length;
        }
    }
}