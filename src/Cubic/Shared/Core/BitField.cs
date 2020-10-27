using System;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Cubic.Core
{
    /// <summary>
    /// A class that represents a BitString with named fields. A field is a range
    /// of contiguous bits in a BitString.  A named field means you can use a name
    /// to address/identify a particular field. To use this class you must do the
    /// following:
    /// 1) Use AddField() to define/add your named fields in the order they
    /// appear inside the BitString
    /// 2) Call a version of InitBitField() to initialize a blank object or
    /// one that is initialized with a particular set of bits.
    /// </summary>
    public class BitField
    {
        #region Internal Classes

        /// <summary>
        /// A class that represents a field in a BitString.  A field is
        /// a range of contiguous bits in a BitString
        /// </summary>
        private class BitFieldDefinition
        {
            /// <summary>
            /// The starting location for a field in a BitString
            /// </summary>
            public int Offset { get; set; }

            /// <summary>
            /// The length of a field in a BitString
            /// </summary>
            public int Length { get; set; }
        }

        #endregion

        #region Private Members

        private BitString m_BitString;
        private OrderedDictionary m_Fields;

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns a BitString that the BitField represents
        /// </summary>
        [Description("The BitString representing the BitField")]
        public BitString BitString
        {
            get { return new BitString(m_BitString); }
        }

        /// <summary>
        /// An array of 32-bit words that stores the bits of 1's and 0's
        /// of the BitString
        /// </summary>
        [Description("The array of 32-bit words storing the BitString")]
        public uint[] Buffer
        {
            get
            {
                // Return a copy of the internal buffer that stores
                // the bits of the BitString
                return m_BitString.Buffer;
            }
        }

        /// <summary>
        /// The number of 32-bit words used to store the BitString
        /// </summary>
        [Description("Number of 32-bit words storing the BitString")]
        public int StorageSize
        {
            get
            {
                return m_BitString.StorageSize;
            }
        }

        /// <summary>
        /// The number of 1's and 0's that the BitString represents
        /// </summary>
        [Description("Number of 1's and 0's that the BitString represents")]
        public int Length
        {
            get
            {
                return m_BitString.Length;
            }
        }

        #endregion

        #region Constructors and Initializers

        /// <summary>
        /// Initiates an empty BitField
        /// </summary>
        public BitField()
        {
            m_BitString = new BitString();
            m_Fields = new OrderedDictionary();
        }

        /// <summary>
        /// Adds a named field to the BitField
        /// </summary>
        /// <param name="name">The name of the field.  Case doesn't matter.</param>
        /// <param name="length">The length of the field</param>
        public void AddField(string name, int length)
        {
          BitFieldDefinition bfd = new BitFieldDefinition()
          {
            Length = length,
            Offset = m_BitString.Length
          };
          m_Fields.Add(name.ToUpper(), bfd);
          m_BitString += new BitString(length);
        }

        /// <summary>
        /// Clear out all the named fields
        /// </summary>
        public void ClearFields()
        {
            m_Fields.Clear();
            m_BitString = new BitString();
        }

        /// <summary>
        /// After defining your named fields, you must call this
        /// method to initialize the BitString.  This initializes
        /// a BitString with all 0's that has the defined named
        /// fields.
        /// </summary>
        public void InitBitField()
        {
            int length = 0;

            // Determine the length of the BitField that is described by the fields
            for (int i = 0; i < m_Fields.Count; i++)
            {
                length += ((BitFieldDefinition)m_Fields[i]).Length;
            }

            m_BitString = new BitString(length);
        }

        /// <summary>
        /// After defining your named fields, you must call this
        /// method to initialize the BitString.  This initializes
        /// a BitString with the bits in the byte buffer.  If the
        /// byte buffer is less than the length of the total named
        /// fields then it is padded with 0's
        /// </summary>
        /// <param name="buffer">A byte array used to initialize the BitString</param>
        public void InitBitField(byte[] buffer)
        {
            int length = 0;

            // Determine the length of the BitField that is described by the fields
            for (int i = 0; i < m_Fields.Count; i++)
            {
                length += ((BitFieldDefinition)m_Fields[i]).Length;
            }

            if (length > buffer.Length * 8)
            {
                int extraBits = length - buffer.Length * 8;

                m_BitString = (new BitString(buffer)) + new string('0', extraBits);
                return;
            }

            m_BitString = new BitString(buffer, length);
        }

        /// <summary>
        /// After defining your named fields, you must call this
        /// method to initialize the BitString.  This initializes
        /// a BitString with the bits in the uint buffer.  If the
        /// uint buffer is less than the length of the total named
        /// fields then it is padded with 0's
        /// </summary>
        /// <param name="buffer">A uint array used to initialze the BitString</param>
        public void InitBitField(uint[] buffer)
        {
            int length = 0;

            // Determine the length of the BitField that is described by the fields
            for (int i = 0; i < m_Fields.Count; i++)
            {
                length += ((BitFieldDefinition)m_Fields[i]).Length;
            }

            if (length > buffer.Length * 32)
            {
                int extraBits = length - buffer.Length * 32;

                m_BitString = (new BitString(buffer)) + new string('0', extraBits);
                return;
            }

            m_BitString = new BitString(buffer, length);
        }

        /// <summary>
        /// After defining your named fields, you must call this
        /// method to initialize the BitString.  This initializes
        /// a BitString with the bits in another BitString.  If the
        /// input BitString is less than the length of the total named
        /// fields then it is padded with 0's
        /// </summary>
        /// <param name="buffer">A BitString used to initialize the BitString</param>
        public void InitBitField(BitString buffer)
        {
            int length = 0;

            // Determine the length of the BitField that is described by the fields
            for (int i = 0; i < m_Fields.Count; i++)
            {
                length += ((BitFieldDefinition)m_Fields[i]).Length;
            }

            if (length > buffer.Length)
            {
                m_BitString = new BitString(buffer + new string('0', length - buffer.Length));
                return;
            }

            m_BitString = new BitString(buffer);
        }

        /// <summary>
        /// After defining your named fields, you must call this
        /// method to initialize the BitString.  This initializes
        /// a BitString with the 1's and 0's in the input string.  If the
        /// input string is less than the length of the total named
        /// fields then it is padded with 0's
        /// </summary>
        /// <param name="buffer">A string of 1's and 0's used to initialize the BitString</param>
        public void InitBitField(String buffer)
        {
            int length = 0;

            // Determine the length of the BitField that is described by the fields
            for (int i = 0; i < m_Fields.Count; i++)
            {
                length += ((BitFieldDefinition)m_Fields[i]).Length;
            }

            if (length > buffer.Length)
            {
                m_BitString = new BitString(buffer + new string('0', length - buffer.Length));
                return;
            }

            m_BitString = new BitString(buffer);
        }

        #endregion

        #region Public API

        /// <summary>
        /// Sets the named field's value to a string of 1's and 0's.  Note that the string of 1's and 0's
        /// should be the same length as defined when you called AddField().  If it is longer then it is
        /// truncated.  If it is shorter then it will be right padded with 0's
        /// </summary>
        /// <param name="field">The name of the field to set</param>
        /// <param name="value">A string of 1's and 0's</param>
        public void SetValue(string field, string value)
        {
            string key = field.ToUpper();

            if (m_Fields.Contains(key))
            {
                BitFieldDefinition bfd = (BitFieldDefinition)m_Fields[key];

                m_BitString[bfd.Offset, bfd.Length] = new BitString(value);
            }
        }

        /// <summary>
        /// Sets the named field's value to a BitString.  Note that the BitString should be the same length
        /// as defined when you called AddField().  If it is longer then it is truncated.  If it is shorter
        /// then it will be right padded with 0's
        /// </summary>
        /// <param name="field">The name of the field to set</param>
        /// <param name="value">A BitString</param>
        public void SetValue(string field, BitString value)
        {
            string key = field.ToUpper();

            if (m_Fields.Contains(key))
            {
                BitFieldDefinition bfd = (BitFieldDefinition)m_Fields[key];

                m_BitString[bfd.Offset, bfd.Length] = value;
            }
        }

        /// <summary>
        /// Array indexing for accessing the name fields of the BitField.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public BitString this[string name]
        {
            get
            {
                string key = name.ToUpper();

                if (m_Fields.Contains(key))
                {
                    BitFieldDefinition bfd = (BitFieldDefinition)m_Fields[key];

                    return m_BitString[bfd.Offset, bfd.Length];
                }

                return new BitString();
            }

            set
            {
                string key = name.ToUpper();

                if (m_Fields.Contains(key))
                {
                    BitFieldDefinition bfd = (BitFieldDefinition)m_Fields[key];

                    m_BitString[bfd.Offset, bfd.Length] = value;
                }
            }
        }

        /// <summary>
        /// Return the buffer as an array of bytes.  This basically means we transform
        /// the buffer (which is represented by an array of uints) to an array of bytes
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            return m_BitString.GetBytes();
        }

        /// <summary>
        /// Return a string representation of the BitField
        /// </summary>
        /// <returns>A string representation of the BitField</returns>
        public override string ToString()
        {
            return m_BitString.ToString();
        }

    #endregion

      public static int GetBit(int bitField, int index)
      {
        return (bitField / (int)Math.Pow(10, index)) % 10;
      }

      public static bool GetFlag(int bitField, int index)
      {
        return GetBit(bitField, index) == 1;
      }
  }
}