using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CSDBCReader
{
    public class DBCWriter
    {
        Stream writer;
        DBCDataTable dataTable;
        byte[] data;
        int[] stringOffsets;
        string[] stringStrings;

        int fieldSize = DBCFile.DEFAULT_FIELD_SIZE;
        int signatureSize = DBCFile.DBC_FILE_SIGNATURE.Length;
        int recordCount;
        int fieldCount;
        int recordSize;
        int recordBlock_Size;
        int stringBlock_Size;
        int stringBlock_Offset;
        int headerSize = DBCFile.HEADER_SIZE;

        public DBCWriter(Stream stream)
        {
            writer = stream;
        }        
        public DBCDataTable DataTable
        {
            get { return dataTable; }
            set { dataTable = value; }
        }
        public void Write()
        {
            //-- Setup the file properties
            recordCount = dataTable.Rows.Length;
            fieldCount = dataTable.Columns.Length;
            recordSize = fieldCount * fieldSize;
            recordBlock_Size = recordSize * recordCount;
            stringBlock_Size = calculateStringBlockSize();
            stringBlock_Offset = recordCount * recordSize + 20; //?!? Some strange behaviour! +20 is a fix (gotta sort this out!) RR

            //-- Build the data
            createDataHolder();
            writeHeader();
            writeData();
            writeStringBlock();

            //-- Write the data to the stream
            writer.Write(data, 0, data.Length);
        }

        bool isStringNullTerminated(string str)
        {
            return (str[str.Length - 1] == 0);
        }

        int calculateStringBlockSize()
        {
            if (stringStrings == null) return 0;

            //-- Get the last string and fetch it's offset, add it's length
            int lastStringIndex = stringStrings.Length - 1;
            string lastString = stringStrings[lastStringIndex];
            int lastStringLength = lastString.Length;
            int lastStringOffset = stringOffsets[lastStringIndex];

            if (!isStringNullTerminated(lastString))
                lastStringLength++;

            return lastStringOffset + lastStringLength;
        }

        /// <summary>
        /// Sets the string block. Perform before using Write()
        /// </summary>
        /// <param name="offsets">An array with the string offsets</param>
        /// <param name="strings">An array with the actual strings</param>
        public void SetStringBlock(int[] offsets, string[] strings)
        {
            stringOffsets = offsets;
            stringStrings = strings;
        }

        /// <summary>
        /// Creates the data array used to write to the new dbc file
        /// </summary>
        void createDataHolder()
        {
            //-- Create a new array containing the DBC data
            data = new byte[headerSize + recordSize * recordCount + stringBlock_Size];
        }

        /// <summary>
        /// Writes the string block to the data array
        /// </summary>
        void writeStringBlock()
        {
            if (stringOffsets == null) return;
            //-- Iterate through each string and write it to the data array
            for (int stringId = 0; stringId < stringStrings.Length; stringId++)
            {
                string cur_string = stringStrings[stringId];
                
                // Check if this string is null-terminated (0x00)
                if (isStringNullTerminated(cur_string))
                {
                    // This string isn't null-terminated, so add a null-byte (0x00)
                    cur_string += (char)0x00;
                }

                // Copy this string to a new byte array
                byte[] b_string = Encoding.ASCII.GetBytes(cur_string);

                // Copy the string data to the data array
                b_string.CopyTo(data, stringBlock_Offset + stringOffsets[stringId]);
            }
        }
        void writeHeader()
        {
            //-- Write the DBC file identifier ('WDBC')
            byte[] b_fileIdentifier = Encoding.ASCII.GetBytes(DBCFile.DBC_FILE_SIGNATURE);
            b_fileIdentifier.CopyTo(data, 0);

            //-- Write the record count (int32)
            byte[] b_recordCount = BitConverter.GetBytes(recordCount);
            b_recordCount.CopyTo(data, 4);

            //-- Write the number of fields in this DBC (int32)
            byte[] b_fieldCount = BitConverter.GetBytes(fieldCount);
            b_fieldCount.CopyTo(data, 8);

            //-- Write the record size of each record in this DBC (int32)
            byte[] b_recordSize = BitConverter.GetBytes(recordSize);
            b_recordSize.CopyTo(data, 12);

            //-- Write the string block size of this DBC (int32)
            byte[] b_stringBlock_Size = BitConverter.GetBytes(stringBlock_Size);
            b_stringBlock_Size.CopyTo(data, 16);

            // The header has now been set up
        }
        void writeData()
        {
            //-- Loop through each record and write it to the byte array
            for (int recordId = 0; recordId < dataTable.Rows.Length; recordId++)
            {
                for (int columnId = 0; columnId < dataTable.Columns.Length; columnId++)
                {
                    int cellOffset = headerSize + (recordId * recordSize) + (columnId * fieldSize);
                    dataTable.Rows[recordId].Cells[columnId].Value.CopyTo(data, cellOffset);
                }
            }
        }
    }
}
