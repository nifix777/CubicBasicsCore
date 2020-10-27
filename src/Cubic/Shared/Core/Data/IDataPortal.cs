using System.Threading.Tasks;

namespace Cubic.Core.Data
{
  public interface IDataPortal<TData>
  {
    Task<TData> CreateAsync();
    Task<TData> CreateAsync(object options);

    Task<TData> FetchAsync();
    Task<TData> FetchAsync(object options);

    Task<TData> DeleteAsync();
    Task<TData> DeleteAsync(object options);

    Task<TData> UpdateAsync(TData data);
    Task<TCommand> Execute<TCommand>(TCommand data);
  }
}