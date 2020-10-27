﻿using System;
using System.Threading;

namespace Cubic.Core.Runtime
{
  public class ResetLazy<T> : IResetLazy<T>
  {
    private class Box
    {
      public Box(T value)
      {
        this.Value = value;
      }

      public readonly T Value;
    }

    public ResetLazy(Func<T> valueFactory, LazyThreadSafetyMode mode = LazyThreadSafetyMode.PublicationOnly,
      Type declaringType = null)
    {
      if (valueFactory == null)
        throw new ArgumentNullException("valueFactory");

      this.mode = mode;
      this.valueFactory = valueFactory;
      this.declaringType = declaringType ?? valueFactory.Method.DeclaringType;
    }

    private LazyThreadSafetyMode mode;
    private Func<T> valueFactory;

    private object syncLock = new object();

    private Box box;

    private Type declaringType;

    public Type DeclaringType
    {
      get { return declaringType; }
    }

    public T Value
    {
      get
      {
        var b1 = this.box;
        if (b1 != null)
          return b1.Value;

        if (mode == LazyThreadSafetyMode.ExecutionAndPublication)
        {
          lock (syncLock)
          {
            var b2 = box;
            if (b2 != null)
              return b2.Value;

            this.box = new Box(valueFactory());

            return box.Value;
          }
        }

        else if (mode == LazyThreadSafetyMode.PublicationOnly)
        {
          var newValue = valueFactory();

          lock (syncLock)
          {
            var b2 = box;
            if (b2 != null)
              return b2.Value;

            this.box = new Box(newValue);

            return box.Value;
          }
        }
        else
        {
          var b = new Box(valueFactory());
          this.box = b;
          return b.Value;
        }
      }
    }

    public bool IsValueCreated
    {
      get { return box != null; }
    }

    public void Reset()
    {
      if (mode != LazyThreadSafetyMode.None)
      {
        lock (syncLock)
        {
          this.box = null;
        }
      }
      else
      {
        this.box = null;
      }
    }
  }
}