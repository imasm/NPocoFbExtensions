using System;
using System.Collections.Generic;
using System.Linq;
using NPoco;

namespace NPocoFbExtensions
{
    public static class DatabaseExtensions
    {

        public static void UpdateOrInsert(this IDatabase db, object poco)
        {
            UpdateOrInsert(db, poco, null, null);
        }

        public static void UpdateOrInsert(this IDatabase db, object poco, object primaryKeyValue)
        {
            UpdateOrInsert(db, poco, primaryKeyValue, null);
        }

        public static void UpdateOrInsert(this IDatabase db, object poco, object primaryKeyValue,
                                          IEnumerable<string> columns)
        {
            var pd = db.PocoDataFactory.ForType(poco.GetType());
            UpdateOrInsert(db, pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, poco);
        }

        public static void UpdateOrInsert<T>(this IDatabase db, string tableName, string primaryKeyName, T poco)
        {
            var pd = db.PocoDataFactory.ForObject(poco, primaryKeyName);
            var names = new List<string>();
            var values = new List<string>();
            var rawvalues = new List<object>();

            var index = 0;
            const string paramPrefix = "@";

            var multiplePrimaryKeysNames =
                primaryKeyName.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();

            foreach (var i in pd.Columns)
            {
                // Don't insert result columns
                if (i.Value.ResultColumn || i.Value.ComputedColumn)
                    continue;

                names.Add(db.DatabaseType.EscapeSqlIdentifier(i.Key));
                values.Add(string.Format("{0}{1}", paramPrefix, index++));

                object value = ProcessMapper(db.Mapper, i.Value, i.Value.GetValue(poco));
                rawvalues.Add(value);
            }

            if (names.Count != 0)
            {
                string sql = string.Format("UPDATE OR INSERT INTO {0} ({1}) VALUES ({2}) MATCHING ({3})",
                                           db.DatabaseType.EscapeTableName(tableName),
                                           string.Join(",", names.ToArray()),
                                           string.Join(",", values.ToArray()),
                                           string.Join(",", multiplePrimaryKeysNames));

                db.Execute(sql, rawvalues.ToArray());
            }
        }

        internal static object ProcessMapper(IMapper mapper, PocoColumn pc, object value)
        {
            if (mapper == null) return value;
            var converter = mapper.GetToDbConverter(pc.ColumnType, pc.MemberInfo);
            return converter != null ? converter(value) : value;
        }
    }
}
