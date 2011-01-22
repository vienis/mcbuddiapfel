using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CSDBCReader
{
    public class DBCReader
    {
        BinaryReader binReader;
        int[] stringTable_Offsets;
        string[] stringTable_Strings;
        bool bEnableCompression;

        public DBCReader(bool enableCompression)
        {
            bEnableCompression = enableCompression;
        }
        public DBCReader()
        {
            bEnableCompression = false;
        }

        public void Open(string fileName)
        {
            binReader = new BinaryReader(File.Open(fileName, FileMode.Open));
        }

        /// <summary>
        /// Returns the string offset
        /// </summary>
        public long StringOffset()
        {
            int recordCount = ReadRecordCount();
            int fieldCount = ReadFieldCount();
            long stringBlockOffset = (recordCount * fieldCount * 4) + DBCFile.HEADER_SIZE + 1;
            return stringBlockOffset;
        }

        /// <summary>
        /// Reads the file signature. For a DBC file it should be "WDBC"
        /// </summary>
        /// <returns>A string containing the file signature</returns>
        public string ReadSignature()
        {
            //-- Reset the stream internal pointer
            resetPointer();

            //-- Read the first 4 bytes
            byte[] data = binReader.ReadBytes(DBCFile.SIGNATURE_LENGTH);

            //-- Convert it to a char array
            char[] sign = new char[DBCFile.SIGNATURE_LENGTH];
            for (int i = 0; i < DBCFile.SIGNATURE_LENGTH; i++)
                sign[i] = (char)data[i];

            //-- Return the char array as a string
            return new string(sign);
        }

        /// <summary>
        /// Reads the record count from the DBC header
        /// </summary>
        /// <returns>Record count in this DBC file</returns>
        public int ReadRecordCount()
        {
            //-- Read the record count (bytes 4-8) and return it
            return readInt32(4);
        }

        /// <summary>
        /// Reads the field count from the DBC header
        /// </summary>
        /// <returns>Field count in this DBC file</returns>
        public int ReadFieldCount()
        {
            //-- Read the field count (bytes 8-12) and return it
            return readInt32(8);
        }

        /// <summary>
        /// Looks up a string in the loaded string table (Don't forget to call ReadStrings first!)
        /// </summary>
        /// <param name="offset">The offset of the string in the string block</param>
        public string GetString(int offset)
        {
            for (int i = 0; i < stringTable_Offsets.Length; i++)
                if (stringTable_Offsets[i] == offset)
                    return stringTable_Strings[i];

            throw new DBCException("No string was found at address " + offset + ".");
        }

        /// <summary>
        /// Reads all strings in the memory with the corresponding string block offset
        /// </summary>
        /// <param name="fieldSize">The DBC field size</param>
        public void ReadStrings(int fieldSize)
        {
            //-- Calculate the offset of the string block
            int recordCount = ReadRecordCount();
            int fieldCount = ReadFieldCount();
            long stringBlockOffset = (recordCount * fieldCount * fieldSize) + DBCFile.HEADER_SIZE + 1;

            //-- Set the internal stream pointer at the begin of the string block
            binReader.BaseStream.Seek(stringBlockOffset, SeekOrigin.Begin);

            //-- Read all strings
            int stringDataLength = (int)(binReader.BaseStream.Length - (long)stringBlockOffset);

            // But first check if there are strings!
            if (stringDataLength == -1)
                return;

            byte[] stringData = binReader.ReadBytes(stringDataLength);
            int stringCount = countStrings(stringData);

            //-- Create a new dictionary for storing the strings in
            stringTable_Offsets = new int[stringCount];
            stringTable_Strings = new string[stringCount];

            List<char> chars = new List<char>();
            long stringOffset = stringBlockOffset;
            int currentStringNum = 0;
            for (int i = 0; i < stringData.Length; i++)
            {
                //-- Read the next byte from the stream
                byte currentByte = stringData[i];

                //-- Check if this byte is a string terminator
                if ((int)currentByte != 0)
                    chars.Add((char)currentByte);
                else
                {
                    //-- Yes, so this string has ended. Add it to the dictionary.
                    char[] stringCharArray = chars.ToArray();
                    stringTable_Offsets[currentStringNum] = i - stringCharArray.Length + 1;
                    stringTable_Strings[currentStringNum] = new string(stringCharArray);

                    //-- Create a new char collection for the new string, and set the new string offset
                    chars = new List<char>();
                    stringOffset = stringBlockOffset - i;
                    currentStringNum++;
                }
            }
        }

        public string ReadString(int offset)
        {
            for (int i = 0; i < stringTable_Offsets.Length; i++)
            {
                if (stringTable_Offsets[i] == offset)
                    return stringTable_Strings[i];
            }
            throw new DBCException("No string was found with the offset '" + offset + "'.");
        }

        int countStrings(byte[] stringBlockData)
        {
            int count = 0;
            for (int i = 0; i < stringBlockData.Length; i++)
            {
                if ((int)stringBlockData[i] == 0)
                    count++;
            }
            stringBlockData = null;
            return count;
        }

        /// <summary>
        /// Checks if a byte array consists our of null values
        /// </summary>
        /// <param name="array"></param>
        /// <returns>True if the byte array is all null</returns>
        bool byteIsNull(byte[] array)
        {
            return (array[0] == 0 && array[1] == 0 && array[2] == 0 && array[3] == 0);
        }

        public void ReadRecord(DBCDataTable table, int fieldSize, int num)
        {
            //-- Read the DBC's field count
            int fieldCount = ReadFieldCount();

            //-- Set the internal stream pointer to the record to read
            long offset = DBCFile.HEADER_SIZE + (num * fieldSize * table.Columns.Length);
            binReader.BaseStream.Seek(offset, SeekOrigin.Begin);

            //-- Create a new row for this DBCDataTable
            DBCDataRow row = table.NewRow();

            //-- Read each column and fill in its value
            for (int i = 0; i < fieldCount; i++)
            {
                byte[] fieldValue = binReader.ReadBytes(fieldSize);

                // If compression has been enabled, check if compression should be applied
                if (bEnableCompression)
                {
                    if (!byteIsNull(fieldValue))
                        row.Cells[i] = new DBCDataField(fieldValue); //no, so assign it
                }
                else
                    row.Cells[i] = new DBCDataField(fieldValue); // compression disabled
            }

            table.Rows[num] = row;
        }

        /// <summary>
        /// Closes this DBCReader
        /// </summary>
        public void Close()
        {
            binReader.Close();
        }

        int readInt32()
        {
            byte[] data = binReader.ReadBytes(4);
            return BitConverter.ToInt32(data, 0);
        }
        int readInt32(long offset)
        {
            setPointer(offset);
            byte[] data = binReader.ReadBytes(4);
            return BitConverter.ToInt32(data, 0);
        }
        void setPointer(long offset)
        {
            binReader.BaseStream.Seek(offset, SeekOrigin.Begin);
        }
        void resetPointer()
        {
            binReader.BaseStream.Seek(0, SeekOrigin.Begin);
        }

        public void GetStringTable(out int[] offsets, out string[] strings)
        {
            offsets = stringTable_Offsets;
            strings = stringTable_Strings;
        }
    }
}
