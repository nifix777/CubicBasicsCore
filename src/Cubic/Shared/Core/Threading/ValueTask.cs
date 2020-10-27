using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Cubic.Core.Diagnostics;

namespace Cubic.Core.Threading
{
  [StructLayout(LayoutKind.Auto)]
  public struct ValueTask<TResult> : IEquatable<ValueTask<TResult>>
  {
    /// <summary>The task to be used if the operation completed asynchronously or if it completed synchronously but non-successfully.</summary>
    internal readonly Task<TResult> _task;
    /// <summary>The result to be used if the operation completed successfully synchronously.</summary>
    internal readonly TResult _result;

    /// <summary>Initialize the <see cref="ValueTask{TResult}"/> with the result of the successful operation.</summary>
    /// <param name="result">The result.</param>
    public ValueTask(TResult result)
    {
      _task = null;
      _result = result;
    }

    /// <summary>
    /// Initialize the <see cref="ValueTask{TResult}"/> with a <see cref="Task{TResult}"/> that represents the operation.
    /// </summary>
    /// <param name="task">The task.</param>
    public ValueTask(Task<TResult> task)
    {
      Guard.AgainstNull(task, nameof(task));

      _task = task;
      _result = default(TResult);
    }

    /// <summary>Returns the hash code for this instance.</summary>
    public override int GetHashCode() =>
        _task != null ? _task.GetHashCode() :
        _result != null ? _result.GetHashCode() :
        0;

    /// <summary>Returns a value indicating whether this value is equal to a specified <see cref="object"/>.</summary>
    public override bool Equals(object obj) =>
        obj is ValueTask<TResult> &&
        Equals((ValueTask<TResult>)obj);

    /// <summary>Returns a value indicating whether this value is equal to a specified <see cref="ValueTask{TResult}"/> value.</summary>
    public bool Equals(ValueTask<TResult> other) =>
        _task != null || other._task != null ?
            _task == other._task :
            EqualityComparer<TResult>.Default.Equals(_result, other._result);

    /// <summary>Returns a value indicating whether two <see cref="ValueTask{TResult}"/> values are equal.</summary>
    public static bool operator ==(ValueTask<TResult> left, ValueTask<TResult> right) =>
        left.Equals(right);

    /// <summary>Returns a value indicating whether two <see cref="ValueTask{TResult}"/> values are not equal.</summary>
    public static bool operator !=(ValueTask<TResult> left, ValueTask<TResult> right) =>
        !left.Equals(right);

    /// <summary>
    /// Gets a <see cref="Task{TResult}"/> object to represent this ValueTask.  It will
    /// either return the wrapped task object if one exists, or it'll manufacture a new
    /// task object to represent the result.
    /// </summary>
    public Task<TResult> AsTask() =>
        // Return the task if we were constructed from one, otherwise manufacture one.  We don't
        // cache the generated task into _task as it would end up changing both equality comparison
        // and the hash code we generate in GetHashCode.
        _task ?? Task.FromResult(_result);

    internal Task<TResult> AsTaskExpectNonNull() =>
        // Return the task if we were constructed from one, otherwise manufacture one.
        // Unlike AsTask(), this method is called only when we expect _task to be non-null,
        // and thus we don't want GetTaskForResult inlined.
        _task ?? GetTaskForResultNoInlining();

    [MethodImpl(MethodImplOptions.NoInlining)]
    private Task<TResult> GetTaskForResultNoInlining() => Task.FromResult(_result);

    /// <summary>Gets whether the <see cref="ValueTask{TResult}"/> represents a completed operation.</summary>
    public bool IsCompleted => _task == null || _task.IsCompleted;

    /// <summary>Gets whether the <see cref="ValueTask{TResult}"/> represents a successfully completed operation.</summary>
    public bool IsCompletedSuccessfully =>
        _task == null || _task.Status == TaskStatus.RanToCompletion;

    /// <summary>Gets whether the <see cref="ValueTask{TResult}"/> represents a failed operation.</summary>
    public bool IsFaulted => _task != null && _task.IsFaulted;

    /// <summary>Gets whether the <see cref="ValueTask{TResult}"/> represents a canceled operation.</summary>
    public bool IsCanceled => _task != null && _task.IsCanceled;

    /// <summary>Gets the result.</summary>
    public TResult Result => _task == null ? _result : _task.GetAwaiter().GetResult();

    /// <summary>Gets an awaiter for this value.</summary>
    public ValueTaskAwaiter<TResult> GetAwaiter() => new ValueTaskAwaiter<TResult>(this);

    /// <summary>Configures an awaiter for this value.</summary>
    /// <param name="continueOnCapturedContext">
    /// true to attempt to marshal the continuation back to the captured context; otherwise, false.
    /// </param>
    public ConfiguredValueTaskAwaitable<TResult> ConfigureAwait(bool continueOnCapturedContext) =>
        new ConfiguredValueTaskAwaitable<TResult>(this, continueOnCapturedContext);

    /// <summary>Gets a string-representation of this <see cref="ValueTask{TResult}"/>.</summary>
    public override string ToString()
    {
      if (IsCompletedSuccessfully)
      {
        TResult result = Result;
        if (result != null)
        {
          return result.ToString();
        }
      }

      return string.Empty;
    }


  }
}