//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Cubic.Core.Tools
//{
//  public static class IdTools
//  {
//    private static readonly char[] s_encode32Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUV".ToCharArray();

//    /// <summary>
//    ///   <para>Generates a string from a given long way faster than long.ToString();</para>
//    ///   <para>Source: <a href="https://www.meziantou.net/2019/03/04/some-performance-tricks-with-net-strings">https://www.meziantou.net/2019/03/04/some-performance-tricks-with-net-strings</a></para>
//    /// </summary>
//    /// <param name="id"></param>
//    public static unsafe string GenerateId(long id)
//    {
//      // The following routine is ~310% faster than calling long.ToString() on x64
//      // and ~600% faster than calling long.ToString() on x86 in tight loops of 1 million+ iterations
//      // See: https://github.com/aspnet/Hosting/pull/385

//      // stackalloc to allocate array on stack rather than heap
//      char* buffer = stackalloc char[13];
//      buffer[0] = s_encode32Chars[(int)(id >> 60) & 31];
//      buffer[1] = s_encode32Chars[(int)(id >> 55) & 31];
//      buffer[2] = s_encode32Chars[(int)(id >> 50) & 31];
//      buffer[3] = s_encode32Chars[(int)(id >> 45) & 31];
//      buffer[4] = s_encode32Chars[(int)(id >> 40) & 31];
//      buffer[5] = s_encode32Chars[(int)(id >> 35) & 31];
//      buffer[6] = s_encode32Chars[(int)(id >> 30) & 31];
//      buffer[7] = s_encode32Chars[(int)(id >> 25) & 31];
//      buffer[8] = s_encode32Chars[(int)(id >> 20) & 31];
//      buffer[9] = s_encode32Chars[(int)(id >> 15) & 31];
//      buffer[10] = s_encode32Chars[(int)(id >> 10) & 31];
//      buffer[11] = s_encode32Chars[(int)(id >> 5) & 31];
//      buffer[12] = s_encode32Chars[(int)id & 31];
//      return new string(buffer);

//    }
//  }
//}
