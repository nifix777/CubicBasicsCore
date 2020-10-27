using System.Collections.Generic;

namespace Cubic.Core.Annotations
{
  /// <summary>
  ///     <para>
  ///         A class that exposes annotations. Annotations allow for arbitrary metadata to be stored on an object.
  ///     </para>
  ///     <para>
  ///         This interface is typically used by database providers (and other extensions). It is generally
  ///         not used in application code.
  ///     </para>
  /// </summary>
  public interface IAnnotatable
  {
    /// <summary>
    ///     Gets the value annotation with the given name, returning null if it does not exist.
    /// </summary>
    /// <param name="name"> The key of the annotation to find. </param>
    /// <returns>
    ///     The value of the existing annotation if an annotation with the specified name already exists. Otherwise, null.
    /// </returns>
    object this[[NotNull] string name] { get; }

    /// <summary>
    ///     Gets the annotation with the given name, returning null if it does not exist.
    /// </summary>
    /// <param name="name"> The key of the annotation to find. </param>
    /// <returns>
    ///     The existing annotation if an annotation with the specified name already exists. Otherwise, null.
    /// </returns>
    IAnnotation FindAnnotation( [NotNull] string name );

    /// <summary>
    /// Determines whether the specified name has annotation.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>
    ///   <c>true</c> if the specified name has annotation; otherwise, <c>false</c>.
    /// </returns>
    bool HasAnnotation( [NotNull] string name );

    /// <summary>
    ///     Gets all annotations on the current object.
    /// </summary>
    IEnumerable<IAnnotation> GetAnnotations();
  }
}