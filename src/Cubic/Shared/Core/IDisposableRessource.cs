using System;

namespace Cubic.Core
{
    public interface IDisposableRessource : IDisposable
    {
        string Name { get; }

        object DisposableObject { get;}

        void DisposeObject(out Exception error);
    }
}