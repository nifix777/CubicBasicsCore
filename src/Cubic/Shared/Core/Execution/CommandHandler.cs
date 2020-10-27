using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cubic.Core.Components;

namespace Cubic.Core.Execution
{

  public class CommandHandler : ICommandHandler
  {
    private readonly IServiceProvider _serviceProvider;

    public CommandHandler(IServiceProvider serviceProvider)
    {
      _serviceProvider = serviceProvider;
    }

    public Task<TResult> HandleAsync<TCommand, TResult>(TCommand command)
    {
      var handlerAsync = _serviceProvider.GetService<ICommandHandlerAsync<TCommand, TResult>>();
      return handlerAsync.HandleAsync(command);
    }

    public Task HandleAsync<TCommand>(TCommand command)
    {
      var handlerAsync = _serviceProvider.GetService<ICommandHandlerAsync<TCommand>>();
      return handlerAsync.HandleAsync(command);
    }

    //public void Handle<TCommand>(TCommand command)
    //{
    //  var handlerAsync = _serviceProvider.GetService<ICommandHandler<TCommand>>();
    //  handlerAsync.Handle(command);
    //}
  }
}
