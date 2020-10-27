using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Execution
{
  public interface ICommandHandler
  {
    Task<TResult> HandleAsync<TCommand, TResult>(TCommand command);
    Task HandleAsync<TCommand>(TCommand command);
    //void Handle<TCommand>(TCommand command);
  }

  public interface ICommandHandlerAsync<in TCommand, TResult>
  {
    Task<TResult> HandleAsync(TCommand command);
  }

  public interface ICommandHandlerAsync<in TCommand>
  {
    Task HandleAsync(TCommand command);
  }

  //public interface ICommandHandler<in TCommand>
  //{
  //  void Handle(TCommand command);
  //}
}
