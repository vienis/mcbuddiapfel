using System;
using System.Collections.Generic;
using System.Text;

namespace CSDBCReader
{
    public class DBCFile
    {
        /* --DBC FILE STRUCTURE--
        [Header]
        Column 	 Field 	    Type 	    Notes
        1 	     Signature 	String 	    (4-bytes) string, always 'WDBC'
        2 	     Records 	Integer 	(4-bytes) number of records in the file
        3 	     Fields 	Integer 	(4-bytes) number of fields per record
        4 	     Record     Size 	    Integer (4-bytes) Fields*FieldSize (FieldSize is usually 4, but not always)
        5 	     String     Block Size 	Integer Size of the string block
         
         
        */
        public const string DBC_FILE_SIGNATURE = "WDBC";
        public const int SIGNATURE_LENGTH = 4;
        public const int HEADER_SIZE = 20;
        public const int DEFAULT_FIELD_SIZE = 4;

        private DBCFileInfo fileInfo;
        private DBCReader reader;
        private DBCDataTable dataTable;
        private string fileName;
        private DBCDataColumn[] columns;

        protected DBCFile()
        {

        }
        protected void LoadDBCFile(string fileName, DBCDataColumn[] columns)
        {
            this.fileName = fileName;
            this.columns = columns;
        }

        public DBCFile(string fileName, DBCDataColumn[] columns)
        {
            LoadDBCFile(fileName, columns);
        }
        public DBCFile(string fileName)
        {
            this.fileName = fileName;
        }

        /// <summary>
        /// Reads the contents of the DBC file
        /// </summary>
        public void Read(bool enableCompression)
        {
            Read(4, enableCompression);
        }
        /// <summary>
        /// Reads the contens of the DBC file
        /// </summary>
        /// <param name="fieldSize">Size of a field</param>
        public void Read(int fieldSize, bool enableCompression)
        {
            //-- Create a new DBCReader object and open our .DBC
            reader = new DBCReader(enableCompression);
            reader.Open(fileName);

            //-- Read the header info
            readHeader(fieldSize);

            //-- Read the DBC data
            readData();

            //-- Close the DBCFileReader
            reader.Close();
        }

        /// <summary>
        /// Returns the read DBCDataTable
        /// </summary>
        /// <returns>A DBCDataTable</returns>
        public virtual DBCDataTable GetDataTable()
        {
            return dataTable;
        }

        /// <summary>
        /// Retrieves the DBCReader this DBCFile is using.
        /// </summary>
        public DBCReader Reader
        {
            get { return reader; }
        }

        void writeInfoData(string text)
        {
            //byte[] data = Encoding.ASCII.GetBytes(text);
            //infoStream.Write(data, 0, data.Length);
            //infoStream.Flush();
            Console.WriteLine(text);
        }

        void readData()
        {
            //-- Create a new data table and setup its columns
            dataTable = new DBCDataTable();
            dataTable.Columns = new DBCDataColumn[fileInfo.Fields];

            if (columns == null)
            {
                for (int i = 0; i < fileInfo.Fields; i++)
                    dataTable.Columns[i] = new DBCDataColumn("Column" + i, false);
            }
            else
            {
                for (int i = 0; i < fileInfo.Fields; i++)
                    dataTable.Columns[i] = columns[i];
            }
            
            //-- Output some reflection data
            writeInfoData("Records to read: " + fileInfo.Records);

            //-- Read all strings
            reader.ReadStrings(FileInfo.FieldSize);

            //-- Read each row
            dataTable.Rows = new DBCDataRow[fileInfo.Records];
            for (int i = 0; i < fileInfo.Records; i++)
                reader.ReadRecord(dataTable, FileInfo.FieldSize, i);
        }
        void readHeader(int fieldSize)
        {
            //-- Check it's signature
            string file_signature = reader.ReadSignature();
            if (!file_signature.Equals(DBC_FILE_SIGNATURE))
            {
                // Incorrect signature!
                reader.Close();
                throw new DBCException("Incorrect DBC signature. File signature should be '" + DBC_FILE_SIGNATURE + "' but is '" + file_signature + "'.");
            }

            //-- Read it's record count
            int recordCount = reader.ReadRecordCount();

            //-- Read it's field count
            int fieldCount = reader.ReadFieldCount();

            //-- Create a new DBCFileInfo struct
            fileInfo = new DBCFileInfo(recordCount, fieldCount, fieldSize);
        }
        
        /// <summary>
        /// Returns the filename of this DBCFile.
        /// </summary>
        public string FileName
        {
            get { return fileName; }
        }

        /// <summary>
        /// Returns the file info of this DBC file.
        /// </summary>
        public DBCFileInfo FileInfo
        {
            get { return fileInfo; }
        }
    }
}
