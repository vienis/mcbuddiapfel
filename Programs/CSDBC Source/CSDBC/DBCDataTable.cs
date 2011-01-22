using System;
using System.Collections.Generic;
using System.Text;

namespace CSDBCReader
{
    public class DBCDataTable
    {
        private DBCDataColumn[] columns;
        private DBCDataRow[] rows;

        public int LookupColumn(string name)
        {
            for (int i = 0; i < columns.Length; i++)
            {
                if (columns[i].Name == name)
                    return i;
            }
            throw new DBCException("The column '" + name + "' was not found in this DBCDataTable.");
        }

        public DBCDataTable()
        {

        }

        public DBCDataRow NewRow()
        {
            return new DBCDataRow(columns.Length);
        }

        public DBCDataRow[] Rows
        {
            get { return rows; }
            set { rows = value; }
        }
        public DBCDataColumn[] Columns
        {
            get { return columns; }
            set { columns = value; }
        }
    }
}
