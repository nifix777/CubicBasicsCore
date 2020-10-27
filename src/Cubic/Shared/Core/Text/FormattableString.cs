

//namespace System
//{
//  using System.Globalization;

//  public abstract class FormattableString : IFormattable
//  {
//    protected FormattableString() { }

//    public abstract string Format { get; }

//    public abstract int ArgumentCount { get; }

//    public abstract object[] GetArguments();

//    public abstract object GetArgument(int index);

//    public abstract string ToString(IFormatProvider formatProvider);

//    string IFormattable.ToString(string ignored, IFormatProvider formatProvider) => this.ToString(formatProvider);

//    public static string Invariant(FormattableString formattable)
//    {
//      if (formattable == null)
//      {
//        throw new ArgumentNullException(nameof(formattable));
//      }

//      return formattable.ToString(CultureInfo.InvariantCulture);
//    }

//    public override string ToString() => this.ToString(CultureInfo.CurrentCulture);
//  }
//}

//namespace System.Runtime.CompilerServices
//{
//  public static class FormattableStringFactory
//  {
//    private sealed class ConcreteFormattableString : FormattableString
//    {
//      private readonly string _format;

//      private readonly object[] _arguments;

//      public override string Format => this._format;

//      public override int ArgumentCount => this._arguments.Length;

//      internal ConcreteFormattableString(string format, object[] arguments)
//      {
//        this._format = format;
//        this._arguments = arguments;
//      }

//      public override object[] GetArguments() => this._arguments;

//      public override object GetArgument(int index) => this._arguments[index];

//      public override string ToString
//          (IFormatProvider formatProvider) => string.Format(formatProvider, this._format, this._arguments);
//    }

//    public static FormattableString Create(string format, params object[] arguments)
//    {
//      if (format == null)
//      {
//        throw new ArgumentNullException(nameof(format));
//      }

//      if (arguments == null)
//      {
//        throw new ArgumentNullException(nameof(arguments));
//      }

//      return new ConcreteFormattableString(format, arguments);
//    }
//  }
//}
