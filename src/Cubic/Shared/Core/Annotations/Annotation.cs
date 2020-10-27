using Cubic.Core.Diagnostics;

namespace Cubic.Core.Annotations
{
  /// <summary>
  ///     <para>
  ///         An arbitrary piece of metadata that can be stored on an object that implements <see cref="IAnnotatable" />.
  ///     </para>
  ///     <para>
  ///         This type is typically used by database providers (and other extensions). It is generally
  ///         not used in application code.
  ///     </para>
  /// </summary>
  public class Annotation : IAnnotation
  {
    /// <summary>
    ///     Initializes a new instance of the <see cref="Annotation" /> class.
    /// </summary>
    /// <param name="name"> The key of this annotation. </param>
    /// <param name="value"> The value assigned to this annotation. </param>
    public Annotation( [NotNull] string name , [NotNull] object value )
    {
      Guard.AgainstNullOrEmpty( name, nameof( name )  );
      Guard.ArgumentNull( value, nameof( value )  );

      Name = name;
      Value = value;
    }

    /// <summary>
    ///     Gets the key of this annotation.
    /// </summary>
    public virtual string Name { get; }

    /// <summary>
    ///     Gets the value assigned to this annotation.
    /// </summary>
    public virtual object Value { get; }
  }
}