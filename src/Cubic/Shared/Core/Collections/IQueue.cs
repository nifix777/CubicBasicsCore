namespace Cubic.Core.Collections
{
  /// <summary>
  /// Genric Interface for Queues
  /// </summary>
  /// <typeparam name="T"></typeparam>
  internal interface IQueue<T>
  {
    void Enqueue( T item );
    bool TryDequeue( out T ret );
  }
}