using System;
using System.Collections.Generic;
using System.Text;

namespace CSDBCReader
{
    public struct DBCBitMask
    {

    }
    public struct DBCFileInfo
    {
        private int records;
        private int fields;
        private int fieldSize;
        private int recordSize;


        public DBCFileInfo(int records, int fields)
        {
            this.records = records;
            this.fields = fields;
            this.fieldSize = 4;
            this.recordSize = fieldSize * fields;
        }

        public DBCFileInfo(int records, int fields, int fieldsize)
        {
            this.records = records;
            this.fields = fields;
            this.fieldSize = fieldsize;
            this.recordSize = fields * fieldSize;
        }

        public int Records
        {
            get { return records; }
            set { records = value; }
        }
        public int Fields
        {
            get { return fields; }
            set { fields = value; }
        }
        public int FieldSize
        {
            get { return fieldSize; }
            set { fieldSize = value; }
        }
        public int RecordSize
        {
            get { return recordSize; }
            set { recordSize = value; }
        }
    }


    public struct DBCDataColumn : System.Data.SqlTypes.INullable
    {
        private string name;
        private object dataType;
        private bool isString;

        public DBCDataColumn(string name, bool isString)
        {
            this.name = name;
            dataType = Type.Missing;
            this.isString = isString;
        }
        public DBCDataColumn(string name, Type type, bool isString)
        {
            this.name = name;
            this.dataType = type;
            this.isString = isString;
        }


        public bool IsString
        {
            get { return isString; }
        }
        public object DataType
        {
            get { return dataType; }
            set { dataType = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public bool IsNull
        {
            get { return name == null; }
        }
    }
    public struct DBCDataRow
    {
        private DBCDataField[] cells;

        public DBCDataRow(int columns)
        {
            cells = new DBCDataField[columns];
        }

        public DBCDataField[] Cells
        {
            get { return cells; }
            set { cells = value; }
        }
    }
    public struct DBCDataField : System.Data.SqlTypes.INullable
    {
        private byte[] byteValue;

        #region Static Null Value
        //-------------------------//
        // To be able to support compression of byteValue, but still being
        // able to use it with ease, we need a global static null value for
        // the DBCDataField's value. It's defined here.
        //-------------------------//
        static byte[] NullValue;
        static DBCDataField()
        {
            NullValue = new byte[4];
        }
        #endregion

        public DBCDataField(byte[] value)
        {
            this.byteValue = value;
        }

        public byte[] Value
        {
            get
            {
                if (byteValue == null)
                    return DBCDataField.NullValue;
                else
                    return byteValue;
            }
            set { byteValue = value; }
        }

        public bool IsNull
        {
            get { return byteValue == null; }
        }
    }
}
