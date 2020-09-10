using System;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace TGS.Tests.TestsCommon.Extensions
{
    public static class TableExtensions
    {
        public static IEnumerable<T> CreateList<T>(this Table table)
        {          
            var pivotTable = new TableWrapper(table);
            var count = table.Rows.Count;
            for (var index = 0; index < count; index++)
            {
                var instance = pivotTable.GetInstanceTable(index).CreateInstance<T>();
                yield return instance;
            }
        }

        public static IEnumerable<T> CreateList<T>(this Table table, Func<TableRow, T> methodToCreateEachInstance)
        {
            return table.Rows
                .Select(row => methodToCreateEachInstance(row));
        }
    }

    internal class TableWrapper
    {
        private readonly Table _table;

        public TableWrapper(Table table)
        {
            _table = table;
        }

        public Table GetInstanceTable(int index)
        {
            AssertItemExistsInTheSet(index);

            return CreateAnInstanceTableForThisItemInTheSet(index);
        }

        private Table CreateAnInstanceTableForThisItemInTheSet(int index)
        {
            var instanceTable = new Table("Field", "Value");
            foreach (var header in _table.Header)
            {
                instanceTable.AddRow(header, _table.Rows[index][header]);
            }

            return instanceTable;
        }

        private void AssertItemExistsInTheSet(int index)
        {
            if (_table.Rows.Count <= index)
            {
                throw new IndexOutOfRangeException();
            }
        }
    }
}