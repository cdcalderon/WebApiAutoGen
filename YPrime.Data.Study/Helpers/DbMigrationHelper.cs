namespace YPrime.Data.Study.Helpers
{
    public static class DbMigrationHelper
    {
        /// <summary>
        /// Helper method to temporarily disable CDC for migrations that conflict with it.
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static string GetDisableChangeDataCaptureSql(
            string schema,
            string tableName)
        {
            var script = "GO; "
                + "EXEC sys.sp_cdc_disable_table "
                + $"@source_schema = N'{schema}', " 
                + $"@source_name = N'{tableName}', "
                + $"@capture_instance = N'{schema}_{tableName}'; "
                + "GO; ";

            return script;
        }
    }
}
