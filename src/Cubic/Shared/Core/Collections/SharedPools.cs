namespace Cubic.Core.Collections
{
  public static class SharedPools
  {
    /// <summary>
    /// pool that uses default constructor with 100 elements pooled
    /// </summary>
    public static ObjectPool<T> BigDefault<T>() where T : class, new()
    {
      return DefaultBigPool<T>.Instance;
    }

    /// <summary>
    /// pool that uses default constructor with 20 elements pooled
    /// </summary>
    public static ObjectPool<T> Default<T>() where T : class, new()
    {
      return DefaultNormalPool<T>.Instance;
    }

    private static class DefaultBigPool<T> where T : class, new()
    {
      public static readonly ObjectPool<T> Instance = new ObjectPool<T>( () => new T() , 100 );
    }

    private static class DefaultNormalPool<T> where T : class, new()
    {
      public static readonly ObjectPool<T> Instance = new ObjectPool<T>( () => new T() , 20 );
    }

    /// <summary>
    /// Used to reduce the # of temporary byte[]s created to satisfy serialization and
    /// other I/O requests
    /// </summary>
    //public static readonly ObjectPool<byte[]> ByteArray = new ObjectPool<byte[]>( () => new byte[ByteBufferSize] , ByteBufferCount );
    //public static readonly ObjectPool<char[]> CharArray = new ObjectPool<char[]>( () => new char[ByteBufferSize] , CharBufferCount );

    public static readonly ArrayPool<byte> ByteArray = new ArrayPool<byte>( ( lenght ) => new byte[lenght] , true, ByteBufferSize );
    public static readonly ArrayPool<char> CharArray = new ArrayPool<char>( (lenght) => new char[lenght] , true , CharBufferCount );

    // byte pooled memory : 4K * 512 = 4MB
    public const int ByteBufferSize = 4 * 1024;
    private const int ByteBufferCount = 512;

    // char pooled memory : 8K * 5 = 40K
    private const int CharBufferCount = 5;
  }
}