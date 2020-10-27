using System;
using System.ComponentModel;
using System.Linq;

namespace Cubic.Core
{
    /// <summary>
    /// A class that represents a string of 1's and 0's such as "101011110101".
    /// Internally, the bits are stored in a uint array that is left justified.
    /// This means that the string above would be stored as
    /// uint[0] = 10101111010100000000000000000000
    /// </summary>
    public class BitString
    {
        #region Private Members

        private uint[] m_Buffer;

        #endregion

        #region Public Properties

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
                uint[] temp = new uint[m_Buffer.Length];
                m_Buffer.CopyTo(temp, 0);
                return temp;
            }
        }

        /// <summary>
        /// The number of 32-bit words used to store the BitString
        /// </summary>
        [Description("Number of 32-bit words storing the BitString")]
        public int StorageSize { get; set; }

        /// <summary>
        /// The number of 1's and 0's that the BitString represents
        /// </summary>
        [Description("Number of 1's and 0's that the BitString represents")]
        public int Length { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor.  Instantiates an empty BitString
        /// </summary>
        public BitString()
        {
            m_Buffer = new uint[0];
            Length = 0;
            StorageSize = 0;
        }

        /// <summary>
        /// Constructor to instantiate a BitString of all 0's
        /// </summary>
        /// <param name="length">The length of the BitString</param>
        public BitString(int length)
        {
            // Record length of new BitString
            Length = length;

            if (Length == 0)
            {
                StorageSize = 0;
                m_Buffer = new uint[0];
            }
            else // Otherwise, determine number of words of storage needed
            {
                StorageSize = Length/32;

                if ((Length%32) != 0)
                {
                    ++StorageSize;
                }

                // Allocate storage
                m_Buffer = new uint[StorageSize];

                // Zero out bit string
                for (int i = 0; i < StorageSize; i++)
                {
                    m_Buffer[i] = 0;
                }
            }

            return;
        }

        /// <summary>
        /// Constructor that is initialized by a string of 1's and 0's.  Only a string
        /// of 1's and 0's is considered valid.  If the string contains any other character
        /// then it will initialize the bit string up to the first invalid character
        /// </summary>
        /// <param name="value">
        /// A string of 1's and 0's that is used to initialize the BitString
        /// </param>
        public BitString(string value)
        {
            // Determine length of string (up to the first invalid character)
            Length = 0;
            for (int i = 0; (i < value.Length) && ((value[i] == '0') || (value[i] == '1')); i++)
            {
                ++Length;
            }

            if (Length == 0)
            {
                // Invalid or empty string for initializer -- construct empty BitString
                StorageSize = 0;
                m_Buffer = new uint[0];
            }
            else
            {
                // Otherwise, determine number of words of storage needed
                StorageSize = Length/32;

                if ((Length%32) != 0)
                {
                    ++StorageSize;
                }

                // Allocate storage
                m_Buffer = new uint[StorageSize];

                // Zero out BitString
                for (int i = 0; i < StorageSize; i++)
                {
                    m_Buffer[i] = 0;
                }

                // Now copy the string of 1's and 0s to the bit string
                for (int i = 0; (i < value.Length) && ((value[i] == '0') || (value[i] == '1')); i++)
                {
                    if (value[i] == '1')
                    {
                        Set(i); // Set the bit to 1, otherwise leave it 0
                    }
                }
            }
        }

        /// <summary>
        /// Constructor that is instantiated with another BitString
        /// </summary>
        /// <param name="value">A BitString used to instantiate a new BitString</param>
        public BitString(BitString value)
        {
            // Copy the length and word size of the new BitString
            Length = value.Length;
            StorageSize = value.StorageSize;

            // Check if input BitString is empty
            if (Length == 0)
            {
                m_Buffer = new uint[0];
            }
            else // Otherwise, allocate storage
            {
                m_Buffer = new uint[StorageSize];

                // Make a copy of the input BitString's character array
                for (int i = 0; i < StorageSize; i++)
                {
                    m_Buffer[i] = value.m_Buffer[i];
                }

            } // end else
        }

        /// <summary>
        /// Constructor that is initialized by a uint array.  The resulting BitString
        /// will contains 1's and 0's that match the 1's and 0's in the uint array.  So,
        /// if the uint array looks like A[0] = 0xFFFFAAAA, A[1] = 0x00002222 then the resulting
        /// BitString looks like "1111111111111111101010101010101000000000000000000010001000100010"
        /// </summary>
        /// <param name="buffer">A uint array used to instantiate a BitString</param>
        public BitString(uint[] buffer)
        {
            if (buffer.Length == 0)
            {
                StorageSize = 0;
                Length = 0;
                m_Buffer = new uint[0];
                return;
            }

            StorageSize = buffer.Length;
            Length = StorageSize*32;
            m_Buffer = new uint[buffer.Length];
            buffer.CopyTo(m_Buffer, 0);
        }

        /// <summary>
        /// Constructor that is initialized by a uint array.  The resulting BitString
        /// will contains 1's and 0's that match the 1's and 0's in the uint array.  So,
        /// if the uint array looks like A[0] = 0xFFFFAAAA, A[1] = 0x00002222 then the resulting
        /// BitString looks like "1111111111111111101010101010101000000000000000000010001000100010"
        /// </summary>
        /// <param name="buffer">A uint array used to instantiate a BitString</param>
        /// <param name="length">The number of bits to use from the buffer</param>
        public BitString(uint[] buffer, int length) : this(buffer)
        {
            // Now resize the buffer to the correct value
            if (length < buffer.Length*32)
            {
                Length = length;

                int correctStorageSize = Length/32;

                if (correctStorageSize == 0)
                {
                    ++correctStorageSize;
                }

                StorageSize = correctStorageSize;
                Array.Resize(ref m_Buffer, correctStorageSize);

                return;
            }
        }

        /// <summary>
        /// Constructor that is initialized by a byte array.  The resulting BitString
        /// will contains 1's and 0's that match the 1's and 0's in the byte array.  So,
        /// if the byte array looks like A[0] = 0xFF, A[1] = 0xAA then the resulting
        /// BitString looks like "11111111101010100000000000000000".  Note that it is padded
        /// with 0's to completely fit inside a 32-bit word boundary
        /// </summary>
        /// <param name="buffer">A byte array used to instantiate a new BitString</param>
        public BitString(byte[] buffer)
        {
            if (buffer.Length == 0)
            {
                StorageSize = 0;
                Length = 0;
                m_Buffer = new uint[0];
                return;
            }

            // Calculate storage needed to store byte array into uint array
            StorageSize = buffer.Length/4;

            int remainder = buffer.Length%4;
            if (remainder != 0)
            {
                ++StorageSize;
            }

            m_Buffer = new uint[StorageSize];
            Length = buffer.Length*8;

            // Copy contents of byte array into uint array
            int index = 0;
            for (int i = 0; i < buffer.Length/4; i++)
            {
                uint temp1 = (uint) buffer[i*4] << 24;
                uint temp2 = (uint) buffer[i*4 + 1] << 16;
                uint temp3 = (uint) buffer[i*4 + 2] << 8;
                uint temp4 = (uint) buffer[i*4 + 3];

                m_Buffer[index] = temp1 | temp2 | temp3 | temp4;
                ++index;
            }

            if (remainder == 1)
            {
                uint temp1 = (uint) buffer[index*4] << 24;

                m_Buffer[StorageSize - 1] = temp1;
            }
            else if (remainder == 2)
            {
                uint temp1 = (uint) buffer[index*4] << 24;
                uint temp2 = (uint) buffer[index*4 + 1] << 16;

                m_Buffer[StorageSize - 1] = temp1 | temp2;
            }
            else if (remainder == 3)
            {
                uint temp1 = (uint) buffer[index*4] << 24;
                uint temp2 = (uint) buffer[index*4 + 1] << 16;
                uint temp3 = (uint) buffer[index*4 + 2] << 8;

                m_Buffer[StorageSize - 1] = temp1 | temp2 | temp3;
            }
        }

        /// <summary>
        /// Constructor that is initialized by a byte array.  The resulting BitString
        /// will contains 1's and 0's that match the 1's and 0's in the byte array.  So,
        /// if the byte array looks like A[0] = 0xFF, A[1] = 0xAA then the resulting
        /// BitString looks like "11111111101010100000000000000000".  Note that it is padded
        /// with 0's to completely fit inside a 32-bit word boundary
        /// </summary>
        /// <param name="buffer">A byte array used to instantiate a new BitString</param>
        public BitString(byte[] buffer, int length) : this(buffer)
        {
            if (length < buffer.Length*8)
            {
                Length = length;
                return;
            }
        }

        #endregion

        #region Public API

        /// <summary>
        /// Clear out the BitString so it is empty
        /// </summary>
        public void Clear()
        {
            m_Buffer = new uint[0];
            Length = 0;
            StorageSize = 0;
        }

        /// <summary>
        /// Flip all the bits in the BitString
        /// </summary>
        public void Flip()
        {
            long i;

            for (i = 0; i < StorageSize; i++)
            {
                // Flip all bits in each word with complement operator
                m_Buffer[i] = ~m_Buffer[i];
            }
        }

        /// <summary>
        /// Flip the bit of the BitString located at position
        /// </summary>
        /// <param name="position">0-based bit position</param>
        public void Flip(int position)
        {
            long wordIndex;
            int bitIndex;
            uint mask;

            wordIndex = position/32; // Get the Word where the bit belongs

            bitIndex = 31 - (position%32); // Determine bit index within Word

            mask = (1U << bitIndex); // Construct mask

            m_Buffer[wordIndex] = m_Buffer[wordIndex] ^ mask; // Flip the bit
        }

        /// <summary>
        /// Clear the bit of the BitString located at position
        /// </summary>
        /// <param name="position">0-based bit position</param>
        public void Reset(int position)
        {
            long wordIndex;
            int bitIndex;
            uint mask;

            wordIndex = position/32; // Get the Word where the bit belongs

            bitIndex = 31 - (position%32); // Determine bit index within Word

            mask = ~(1U << bitIndex); // Construct mask

            m_Buffer[wordIndex] = m_Buffer[wordIndex] & mask; // Clear the bit
        }

        /// <summary>
        /// Set the bit of the BitString located at position
        /// </summary>
        /// <param name="position">0-based bit position</param>
        public void Set(int position)
        {
            long wordIndex;
            int bitIndex;
            uint mask;

            wordIndex = position/32; // Get the Word where the bit belongs

            bitIndex = 31 - (position%32); // Determine bit index within Word

            mask = (1U << bitIndex); // Construct mask

            m_Buffer[wordIndex] = m_Buffer[wordIndex] | mask; // Set the bit

            return;
        }

        /// <summary>
        /// Reverse the BitString.  So, 11001 becomes 10011
        /// </summary>
        public void Reverse()
        {
            string temp = new string(Enumerable.ToArray(ToString().Reverse()));

            BitString bs = new BitString(temp);

            Length = bs.Length;
            StorageSize = bs.StorageSize;
            m_Buffer = bs.Buffer;
        }

        /// <summary>
        /// Returns true if the BitString is all 0's, false otherwise
        /// </summary>
        /// <returns>True if the BitString is all 0's, False otherwise</returns>
        public bool IsAllZeros()
        {
            bool answer = true;
            int remainder = Length%32;

            // Create a copy of the original but padded with 0's to make it an even multiple of 32-bits
            BitString temp = new BitString(this.ToString()) + new string('0', 32 - remainder);

            for (int i = 0; i < temp.m_Buffer.Length; i++)
            {
                if (temp.m_Buffer[i] != 0)
                {
                    answer = false;
                    break;
                }
            }

            return answer;
        }

        /// <summary>
        /// Converts the BitString into a byte.  If the BitString is larger than 8-bits, it
        /// will throw an exception.  For BitStrings less than 8-bits it will pad 0's to the
        /// high order bits to make the BitString equal to 8-bits.
        /// </summary>
        /// <returns>The byte that the BitString represents</returns>
        public byte ConvertToByte()
        {
            BitString temp = new BitString(this.ToString());

            if (Length > 8)
            {
                throw new BitStringException("Unable to convert BitString with length = " + Length.ToString() +
                                             " into a 8-bit unsigned integer");
            }

            if (Length < 8)
            {
                // Pad 0's to make temp exactly 8-bits
                temp = new string('0', 8 - Length) + temp;
            }

            return (byte) (temp.Buffer[0] >> 24);
        }

        /// <summary>
        /// Converts the BitString into a 16-bit signed integer.  If the BitString is larger than 16-bits, it
        /// will throw an exception.  For BitStrings less than 16-bits it will pad either 0's or 1's to the
        /// high order bits to make the BitString equal to 16-bits.  If the high order bit is 0, then it will
        /// pad 0's.  If the high order bit is 1, then it will pad 1's to sign extend it.
        /// </summary>
        /// <returns>The 16-bit signed integer that the BitString represents</returns>
        public Int16 ConvertToInt16()
        {
            BitString temp = new BitString(this.ToString());

            if (Length > 16)
            {
                throw new BitStringException("Unable to convert BitString with length = " + Length.ToString() +
                                             " into a 16-bit integer");
            }

            if (Length < 16)
            {
                if (temp[0] == false)
                {
                    // Pad 0's to make temp exactly 32-bits
                    temp = new string('0', 32 - Length) + temp;
                }
                else
                {
                    // Pad 1's to make temp exactly 32-bits
                    temp = new string('1', 32 - Length) + temp;
                }
            }

            // Now get a byte array from the 32-bit buffer
            byte[] tempArray = BitConverter.GetBytes(temp.m_Buffer[0]);

            return BitConverter.ToInt16(tempArray, 0);
        }

        /// <summary>
        /// Converts the BitString into a 16-bit unsigned integer.  If the BitString is larger than 16-bits, it
        /// will throw an exception.  For BitStrings less than 16-bits it will pad either 0's to the high order
        /// bits to make the BitString equal to 16-bits.
        /// </summary>
        /// <returns>The 16-bit unsigned integer that the BitString represents</returns>
        public UInt16 ConvertToUInt16()
        {
            BitString temp = new BitString(this.ToString());

            if (Length > 16)
            {
                throw new BitStringException("Unable to convert BitString with length = " + Length.ToString() +
                                             " into a 16-bit unsigned integer");
            }

            if (Length < 16)
            {
                // Pad 0's to make temp exactly 32-bits
                temp = new string('0', 32 - Length) + temp;
            }

            // Now get a byte array from the 32-bit buffer
            byte[] tempArray = BitConverter.GetBytes(temp.m_Buffer[0]);

            return BitConverter.ToUInt16(tempArray, 0);
        }

        /// <summary>
        /// Converts the BitString into a 32-bit signed integer.  If the BitString is larger than 32-bits, it
        /// will throw an exception.  For BitStrings less than 32-bits it will pad either 0's or 1's to the
        /// high order bits to make the BitString equal to 32-bits.  If the high order bit is 0, then it will
        /// pad 0's.  If the high order bit is 1, then it will pad 1's to sign extend it.
        /// </summary>
        /// <returns>The 32-bit signed integer that the BitString represents</returns>
        public Int32 ConvertToInt32()
        {
            BitString temp = new BitString(this.ToString());

            if (Length > 32)
            {
                throw new BitStringException("Unable to convert BitString with length = " + Length.ToString() +
                                             " into a 32-bit integer");
            }

            if (Length < 32)
            {
                if (temp[0] == false)
                {
                    // Pad 0's to make temp exactly 32-bits
                    temp = new string('0', 32 - Length) + temp;
                }
                else
                {
                    // Pad 1's to make temp exactly 32-bits
                    temp = new string('1', 32 - Length) + temp;
                }
            }

            // Now get a byte array from the 32-bit buffer
            byte[] tempArray = BitConverter.GetBytes(temp.m_Buffer[0]);

            return BitConverter.ToInt32(tempArray, 0);
        }

        /// <summary>
        /// Converts the BitString into a 32-bit unsigned integer.  If the BitString is larger than 32-bits, it
        /// will throw an exception.  For BitStrings less than 32-bits it will pad either 0's to the high order
        /// bits to make the BitString equal to 32-bits.
        /// </summary>
        /// <returns>The 32-bit unsigned integer that the BitString represents</returns>
        public UInt32 ConvertToUInt32()
        {
            BitString temp = new BitString(this.ToString());

            if (Length > 32)
            {
                throw new BitStringException("Unable to convert BitString with length = " + Length.ToString() +
                                             " into a 32-bit unsigned integer");
            }

            if (Length < 32)
            {
                // Pad 0's to make temp exactly 32-bits
                temp = new string('0', 32 - Length) + temp;
            }

            // Now get a byte array from the 32-bit buffer
            byte[] tempArray = BitConverter.GetBytes(temp.m_Buffer[0]);

            return BitConverter.ToUInt32(tempArray, 0);
        }

        /// <summary>
        /// Converts the BitString into a 64-bit signed integer.  If the BitString is larger than 64-bits, it
        /// will throw an exception.  For BitStrings less than 64-bits it will pad either 0's or 1's to the
        /// high order bits to make the BitString equal to 64-bits.  If the high order bit is 0, then it will
        /// pad 0's.  If the high order bit is 1, then it will pad 1's to sign extend it.
        /// </summary>
        /// <returns>The 64-bit signed integer that the BitString represents</returns>
        public Int64 ConvertToInt64()
        {
            BitString temp = new BitString(this.ToString());

            if (Length > 64)
            {
                throw new BitStringException("Unable to convert BitString with length = " + Length.ToString() +
                                             " into a 64-bit integer");
            }

            if (Length < 64)
            {
                if (temp[0] == false)
                {
                    // Pad 0's to make temp exactly 64-bits
                    temp = new string('0', 64 - Length) + temp;
                }
                else
                {
                    // Pad 1's to make temp exactly 64-bits
                    temp = new string('1', 64 - Length) + temp;
                }
            }

            // Now get a byte array from the 64-bit buffer
            byte[] tempArray1 = BitConverter.GetBytes(temp.m_Buffer[1]);
            byte[] tempArray2 = BitConverter.GetBytes(temp.m_Buffer[0]);

            byte[] tempArray = new byte[8];
            Array.Copy(tempArray1, tempArray, 4);
            Array.Copy(tempArray2, 0, tempArray, 4, 4);

            return BitConverter.ToInt64(tempArray, 0);
        }

        /// <summary>
        /// Converts the BitString into a 64-bit unsigned integer.  If the BitString is larger than 64-bits, it
        /// will throw an exception.  For BitStrings less than 64-bits it will pad either 0's to the high order
        /// bits to make the BitString equal to 64-bits.
        /// </summary>
        /// <returns>The 64-bit unsigned integer that the BitString represents</returns>
        public UInt64 ConvertToUInt64()
        {
            BitString temp = new BitString(this.ToString());

            if (Length > 64)
            {
                throw new BitStringException("Unable to convert BitString with length = " + Length.ToString() +
                                             " into a 64-bit unsigned integer");
            }

            if (Length < 64)
            {
                // Pad 0's to make temp exactly 64-bits
                temp = new string('0', 64 - Length) + temp;
            }

            // Now get a byte array from the 64-bit buffer
            byte[] tempArray1 = BitConverter.GetBytes(temp.m_Buffer[1]);
            byte[] tempArray2 = BitConverter.GetBytes(temp.m_Buffer[0]);

            byte[] tempArray = new byte[8];
            Array.Copy(tempArray1, tempArray, 4);
            Array.Copy(tempArray2, 0, tempArray, 4, 4);

            return BitConverter.ToUInt64(tempArray, 0);
        }

        /// <summary>
        /// Returns a string displaying the bits of 1's and 0's in a uint
        /// </summary>
        /// <param name="myBits">A uint containing the bits you want to display</param>
        /// <param name="numDisplayBits">The number of bits in myBits to display</param>
        /// <returns>A string displaying the bits of 1's and 0's in a uint</returns>
        private string DisplayBits(uint myBits, int numDisplayBits)
        {
            uint MASK = 1U << 31;
            string answer = string.Empty;

            for (int i = 0; i < numDisplayBits; i++)
            {
                if ((myBits & MASK) != 0)
                {
                    answer += "1";
                }
                else
                {
                    answer += "0";
                }

                myBits <<= 1;
            }

            return answer;
        }

        /// <summary>
        /// Return the buffer as an array of bytes.  This basically means we transform
        /// the buffer (which is represented by an array of uints) to an array of bytes
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            // Determine size of byte[] needed to store all the bits from
            // the uint array
            int size = Length/8;

            if (Length%8 != 0)
            {
                ++size;
            }

            byte[] answer = new byte[size];

            for (int i = 0; i < StorageSize; i++)
            {
                byte[] temp = BitConverter.GetBytes(m_Buffer[i]);

                if ((i*4) < size)
                {
                    answer[i*4] = temp[3];
                }

                if ((i*4 + 1) < size)
                {
                    answer[i*4 + 1] = temp[2];
                }

                if ((i*4 + 2) < size)
                {
                    answer[i*4 + 2] = temp[1];
                }

                if ((i*4 + 3) < size)
                {
                    answer[i*4 + 3] = temp[0];
                }
            }

            return answer;
        }

        /// <summary>
        /// Return a string representation of the BitString
        /// </summary>
        /// <returns>A string representation of the BitString</returns>
        public override string ToString()
        {
            string answer = string.Empty;

            for (int i = 0; i < StorageSize; i++)
            {
                if (i < StorageSize - 1)
                {
                    answer += DisplayBits(m_Buffer[i], 32);
                }
                else
                {
                    int k = Length%32;

                    if (k == 0)
                    {
                        answer += DisplayBits(m_Buffer[i], 32);
                    }
                    else
                    {
                        answer += DisplayBits(m_Buffer[i], k);
                    }
                }
            }

            return answer;
        }

        #endregion

        #region Overloaded Operators

        /// <summary>
        /// Implicit Conversion operator to convert a string of
        /// 1's and 0's to a BitString
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator BitString(string value)
        {
            return new BitString(value);
        }

        /// <summary>
        /// Implicit Conversion operator to convert a byte to a BitString
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator BitString(byte value)
        {
            byte[] temp = new byte[1];
            temp[0] = value;

            return new BitString(temp);
        }

        /// <summary>
        /// Implicit Conversion operator to convert a Int16 to a BitString
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator BitString(Int16 value)
        {
            byte[] temp = new byte[2];
            temp = Enumerable.ToArray(BitConverter.GetBytes(value).Reverse());

            return new BitString(temp);
        }

        /// <summary>
        /// Implicit Conversion operator to convert a UInt16 to a BitString
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator BitString(UInt16 value)
        {
            byte[] temp = new byte[2];
            temp = Enumerable.ToArray(BitConverter.GetBytes(value).Reverse());

            return new BitString(temp);
        }

        /// <summary>
        /// Implicit Conversion operator to convert a Int32 to a BitString
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator BitString(Int32 value)
        {
            byte[] temp = new byte[4];
            temp = Enumerable.ToArray(BitConverter.GetBytes(value).Reverse());

            return new BitString(temp);
        }

        /// <summary>
        /// Implicit Conversion operator to convert a UInt32 to a BitString
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator BitString(UInt32 value)
        {
            byte[] temp = new byte[4];
            temp = Enumerable.ToArray(BitConverter.GetBytes(value).Reverse());

            return new BitString(temp);
        }

        /// <summary>
        /// Implicit Conversion operator to convert a Int64 to a BitString
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator BitString(Int64 value)
        {
            byte[] temp = new byte[8];
            temp = Enumerable.ToArray(BitConverter.GetBytes(value).Reverse());

            return new BitString(temp);
        }

        /// <summary>
        /// Implicit Conversion operator to convert a UInt64 to a BitString
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator BitString(UInt64 value)
        {
            byte[] temp = new byte[8];
            temp = Enumerable.ToArray(BitConverter.GetBytes(value).Reverse());

            return new BitString(temp);
        }

        /// <summary>
        /// Implicit Conversion operator to convert a BitString to a Int16
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Byte(BitString value)
        {
            return value.ConvertToByte();
        }

        /// <summary>
        /// Implicit Conversion operator to convert a BitString to a Int16
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Int16(BitString value)
        {
            return value.ConvertToInt16();
        }

        /// <summary>
        /// Implicit Conversion operator to convert a BitString to a UInt16
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator UInt16(BitString value)
        {
            return value.ConvertToUInt16();
        }

        /// <summary>
        /// Implicit Conversion operator to convert a BitString to a Int32
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Int32(BitString value)
        {
            return value.ConvertToInt32();
        }

        /// <summary>
        /// Implicit Conversion operator to convert a BitString to a UInt32
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator UInt32(BitString value)
        {
            return value.ConvertToUInt32();
        }

        /// <summary>
        /// Implicit Conversion operator to convert a BitString to a Int64
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Int64(BitString value)
        {
            return value.ConvertToInt64();
        }

        /// <summary>
        /// Implicit Conversion operator to convert a BitString to a UInt64
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator UInt64(BitString value)
        {
            return value.ConvertToUInt64();
        }

        /// <summary>
        /// Concatenation of two BitStrings
        /// </summary>
        /// <param name="b1">A BitString that will be left concatenated with b2</param>
        /// <param name="b2">A BitString that will be right concatenated with b1</param>
        /// <returns></returns>
        public static BitString operator +(BitString b1, BitString b2) // input; BitString object to concatenate
        {
            ulong tempLeft; // Temporary buffer variable
            ulong tempRight; // Temporary buffer variable
            int shiftAmount; // Amount of bit shifting required
            long leftMask; // Mask for tempLeft
            long rightMask; // Mask for tempRight
            long leftPos;
            long rightPos;
            int newLength;

            // Declare new bit string to hold result
            newLength = b1.Length + b2.Length;
            BitString NewBitString = new BitString(newLength);

            // First, check if both operands are empty strings
            if (newLength == 0)
            {
                return NewBitString; // Return an empty string
            }

            // Now check if this object is empty
            if (b1.Length == 0)
            {
                return new BitString(b2); // No need to concatenate -- just return copy of right operand
            }

            // Now check if the right operand is empty
            if (b2.Length == 0)
            {
                return new BitString(b1); // No need to concatenate -- just return copy of left operand
            }

            // Otherwise start concatenation by copying this object into new bit string
            for (int i = 0; i < b1.StorageSize; i++)
            {
                NewBitString.m_Buffer[i] = b1.m_Buffer[i];
            }

            // Now we need to concatenate the right operand onto the new bit string

            // Calculate beginning left operand index
            leftPos = b1.StorageSize - 1;

            if ((b1.Length%32) != 0) // Shifting needed since this object is not multiple of 32 bits
            {
                // Calculate the number of places to shift
                shiftAmount = 32 - (b1.Length%32);

                // Calculate the Left Mask
                leftMask = -9223372036854775808; // ‭0x8000000000000000‬
                leftMask = leftMask >> ((b1.Length%32) - 1);

                // Calculate the Right Mask
                rightMask = 0x00000000FFFFFFFF;

                // Now copy the right operand word by word to the end of the new bit string
                // along with the required shifting, masking and ORing operations
                for (rightPos = 0; rightPos < b2.StorageSize; rightPos++)
                {
                    // grab right most word from left bit string
                    tempLeft = NewBitString.m_Buffer[leftPos];
                    tempLeft = tempLeft << 32;

                    // mask out the lower trash bits
                    tempLeft = tempLeft & (ulong) leftMask;

                    // grab left most byte from right operand
                    tempRight = b2.m_Buffer[rightPos];
                    tempRight = tempRight & (ulong) rightMask;
                    tempRight = tempRight << shiftAmount;

                    // Now concatenate the words into temporary buffer by OR'ing them
                    tempLeft = tempLeft | tempRight;

                    // Now copy this into the new bit string
                    byte[] byteArray = BitConverter.GetBytes(tempLeft);

                    // Check for special end case of 1-word copy
                    if ((rightPos == (b2.StorageSize - 1)) && ((b2.Length%32) != 0) &&
                        (shiftAmount >= (b2.Length%32)))
                    {
                        uint t1 = BitConverter.ToUInt32(byteArray, 4);
                        NewBitString.m_Buffer[leftPos] = t1; // Little Endian Machine
                    }
                    else // Normal 2 word copying
                    {
                        uint t0 = BitConverter.ToUInt32(byteArray, 0);
                        uint t1 = BitConverter.ToUInt32(byteArray, 4);

                        NewBitString.m_Buffer[leftPos] = t1; // Little Endian Machine
                        NewBitString.m_Buffer[leftPos + 1] = t0; // Little Endian Machine
                    }

                    // Adjust indices
                    ++leftPos;

                } // end for
            }
            else // No bit shifting needed since this object is multiple of 32 bits
            {
                // Pad the right operand to the new bit string
                for (rightPos = 0; rightPos < b2.StorageSize; rightPos++)
                {
                    NewBitString.m_Buffer[leftPos + 1] = b2.m_Buffer[rightPos];
                    ++leftPos;
                }
            }

            // return the new bit string
            return NewBitString;
        }

        /// <summary>
        /// Concatenation of a BitString and a regular String of 1's and 0's
        /// </summary>
        /// <param name="b1">A BitString to concatenate</param>
        /// <param name="b2">A regular string of 1's and 0's to concatenate</param>
        /// <returns>A new BitString that is the concatenation of b1 and b2</returns>
        public static BitString operator +(BitString b1, string b2)
        {
            // Change the string of 1's and 0's into a BitString
            BitString tempStr = new BitString(b2);

            // Construct new BitString to be the concatenation of the string and
            // the BitString and return the result
            return (b1 + tempStr);
        }

        /// <summary>
        /// Concatenation of a BitString and a regular String of 1's and 0's
        /// </summary>
        /// <param name="b1">A regular string of 1's and 0's to concatenate</param>
        /// <param name="b2">A BitString to concatenate</param>
        /// <returns>A new BitString that is the concatenation of b1 and b2</returns>
        public static BitString operator +(string b1, BitString b2)
        {
            // Change the string of 1's and 0's into a BitString
            BitString tempStr = new BitString(b1);

            // Construct new BitString to be the concatenation of the string and
            // the BitString and return the result
            return (tempStr + b2);
        }

        /// <summary>
        /// Bitwise Exclusive-OR of two BitStrings.  If the two BitStrings have different
        /// lengths then the shorter is padded by 0's at the end so the two are
        /// equal length before the two strings are Bitwise Exclusive-OR
        /// </summary>
        /// <param name="b1">A BitString</param>
        /// <param name="b2">A BitString</param>
        /// <returns></returns>
        public static BitString operator ^(BitString b1, BitString b2)
        {
            BitString bs1; // Holds the longer BitString
            BitString bs2; // Holds the shorter BitString

            if (b1.Length == 0 && b2.Length == 0)
            {
                return new BitString();
            }

            if (b1.Length == b2.Length)
            {
                bs1 = b1;
                bs2 = b2;
            }
            else if (b1.Length < b2.Length)
            {
                bs1 = b2;

                bs2 = b1 + new string('0', b2.Length - b1.Length);
            }
            else
            {
                bs1 = b1;

                bs2 = b2 + new string('0', b1.Length - b2.Length);
            }

            BitString answer = new BitString(bs1.Length);
            for (int i = 0; i < bs1.StorageSize; i++)
            {
                answer.m_Buffer[i] = bs1.m_Buffer[i] ^ bs2.m_Buffer[i];
            }

            return answer;
        }

        /// <summary>
        /// Bitwise AND of two BitStrings.  If the two BitStrings have different
        /// lengths then the shorter is padded by 1's at the end so the two are
        /// equal length before the two strings are Bitwise AND
        /// </summary>
        /// <param name="b1">A BitString</param>
        /// <param name="b2">A BitString</param>
        /// <returns></returns>
        public static BitString operator &(BitString b1, BitString b2)
        {
            BitString bs1; // Holds the longer BitString
            BitString bs2; // Holds the shorter BitString

            if (b1.Length == 0 && b2.Length == 0)
            {
                return new BitString();
            }

            if (b1.Length == b2.Length)
            {
                bs1 = b1;
                bs2 = b2;
            }
            else if (b1.Length < b2.Length)
            {
                bs1 = b2;

                bs2 = b1 + new string('1', b2.Length - b1.Length);
            }
            else
            {
                bs1 = b1;

                bs2 = b2 + new string('1', b1.Length - b2.Length);
            }

            BitString answer = new BitString(bs1.Length);
            for (int i = 0; i < bs1.StorageSize; i++)
            {
                answer.m_Buffer[i] = bs1.m_Buffer[i] & bs2.m_Buffer[i];
            }

            return answer;
        }

        /// <summary>
        /// Bitwise OR of two BitStrings.  If the two BitStrings have different
        /// lengths then the shorter is padded by 0's at the end so the two are
        /// equal length before the two strings are Bitwise OR
        /// </summary>
        /// <param name="b1">A BitString</param>
        /// <param name="b2">A BitString</param>
        /// <returns></returns>
        public static BitString operator |(BitString b1, BitString b2)
        {
            BitString bs1; // Holds the longer BitString
            BitString bs2; // Holds the shorter BitString

            if (b1.Length == 0 && b2.Length == 0)
            {
                return new BitString();
            }

            if (b1.Length == b2.Length)
            {
                bs1 = b1;
                bs2 = b2;
            }
            else if (b1.Length < b2.Length)
            {
                bs1 = b2;

                bs2 = b1 + new string('0', b2.Length - b1.Length);
            }
            else
            {
                bs1 = b1;

                bs2 = b2 + new string('0', b1.Length - b2.Length);
            }

            BitString answer = new BitString(bs1.Length);
            for (int i = 0; i < bs1.StorageSize; i++)
            {
                answer.m_Buffer[i] = bs1.m_Buffer[i] | bs2.m_Buffer[i];
            }

            return answer;
        }

        public static BitString operator ++(BitString b1)
        {
            BitString temp = new BitString(b1.ToString());

            if (temp.Length > 64)
            {
                throw new BitStringException("Unable to increment BitString with length = " + b1.Length.ToString() +
                                             ". The maximum length for this operation is 64-bits");
            }

            temp.Flip();
            if (temp.IsAllZeros())
            {
                throw new BitStringException("Unable to increment BitString.  It is already at maximum value (all 1's)");
            }

            UInt64 value = b1.ConvertToUInt64();

            value++;
            byte[] vByte = BitConverter.GetBytes(value);
            Array.Reverse(vByte);

            return new BitString(vByte)[64 - b1.Length, b1.Length];
        }

        public static BitString operator --(BitString b1)
        {
            BitString temp = new BitString(b1.ToString());

            if (temp.Length > 64)
            {
                throw new BitStringException("Unable to decrement BitString with length = " + b1.Length.ToString() +
                                             ". The maximum length for this operation is 64-bits");
            }

            if (temp.IsAllZeros())
            {
                throw new BitStringException("Unable to decrement BitString.  It is already at minimum value (all 0's)");
            }

            UInt64 value = b1.ConvertToUInt64();

            value--;
            byte[] vByte = BitConverter.GetBytes(value);
            Array.Reverse(vByte);

            return new BitString(vByte)[64 - b1.Length, b1.Length];
        }

        #endregion

        #region Indexers

        /// <summary>
        /// Array indexing operator for accessing elements of the BitString
        /// </summary>
        /// <param name="index">The position (0-based) of the element you want to access</param>
        /// <returns>True if the bit is 1 at the index position, False otherwise</returns>
        public bool this[int index]
        {
            get
            {
                char temp = this.ToString()[index];

                if (temp == '1')
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            set
            {
                if (value == true)
                {
                    this.Set(index);
                }
                else
                {
                    this.Reset(index);
                }
            }
        }

        /// <summary>
        /// Returns a BitString that is a subset of this instance.  The new BitString
        /// begins at startIndex and has the specified length
        /// </summary>
        /// <param name="startIndex">The starting position (0-based) in the current BitString to begin subsetting</param>
        /// <param name="length">The length of BitString to </param>
        /// <returns>A BitString that is a subset of the current BitString</returns>

        public BitString this[int startIndex, int length]
        {
            get
            {
                // Get the string representation of the current instance and
                // use the string substring method to get the substring
                string temp = ToString().Substring(startIndex, length);

                // Return the BitString that represents the substring
                return new BitString(temp);
            }

            set
            {
                string temp = value.ToString();

                // Make sure the input BitString has the same length
                // as the subset range
                if (temp.Length > length)
                {
                    // The input BitString is longer than the subset
                    // range so we trim it to be the same size
                    temp = temp.Substring(temp.Length - length, length);
                }
                else if (temp.Length < length)
                {
                    // The input BitString is shorter than the subset
                    // range so we pad it with 0's to match the subset
                    // length
                    temp = new string('0', length - temp.Length) + temp;
                }

                // Get the string representation of the current instance
                // and cut-out the subset range
                string bs = this.ToString().Remove(startIndex, length);

                // Insert the input BitString into the removed subset using
                // normal string operators
                bs = bs.Insert(startIndex, temp);

                // Create a new BitString from the string.  This BitString
                // represents the original BitString with the input BitString
                // replacing the subset range
                BitString bitStr = new BitString(bs);

                // Now copy over the modified BitString into the
                // current instance
                this.StorageSize = bitStr.StorageSize;
                this.m_Buffer = new uint[bitStr.StorageSize];
                bitStr.m_Buffer.CopyTo(this.m_Buffer, 0);
                this.Length = bitStr.Length;
            }
        }



        #endregion

        #region Helper Classes

        public class BitStringException : Exception
        {
            public BitStringException(string message) : base(message)
            {
                // Do nothing
            }
        }

        #endregion
    }
}