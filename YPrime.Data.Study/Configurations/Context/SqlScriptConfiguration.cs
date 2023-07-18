using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace YPrime.Data.Study.Configurations.Context
{
    public class SqlScriptConfiguration
    {
        public static void Seed(StudyDbContext context, bool executeAllScripts)
        {
            //Init
            var sqlScriptsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SQLScripts");
            Console.WriteLine("---Begin SQLScripts Seeding--- \n");

            //Execute scripts
            var errorString = RecurseSqlSeedDirectories(context, sqlScriptsDirectory, executeAllScripts);
            Console.WriteLine("\n---End SQLScripts Seeding---");

            //Save or tell user about failures
            if (errorString == null)
            {
                context.SaveChanges();
            }
            else
            {
                throw new Exception($"Failures during SQL Execution. {errorString}");
            }
        }

        private static string RecurseSqlSeedDirectories(StudyDbContext _context, string sqlScriptsDirectory, bool selectAll)
        {
            //Get the SQL scripts (including subdirectories)
            Console.WriteLine($"Searching for scripts in: '{sqlScriptsDirectory}'");
            var wildcard = selectAll ? "*" : "CheckAuditTableColumns";
            var scriptPaths = Directory.GetFiles(sqlScriptsDirectory, $"{wildcard}.sql", SearchOption.AllDirectories);
            var scriptCount = scriptPaths.Count();
            var sql = "";

            //Log what we found
            Console.WriteLine($"Found {scriptCount} scripts: \n");
            foreach (var scriptPath in scriptPaths)
            {
                Console.WriteLine(scriptPath);
            }

            Console.WriteLine("\n");

            //Loop em
            string errorString = null;
            var errors = 0;

            foreach (var scriptPath in scriptPaths)
            {
                //Read
                Console.WriteLine($"Opening script: '{Path.GetFileName(scriptPath)}'");
                var script = File.ReadAllText(scriptPath, Encoding.UTF8);
                //Execute
                try
                {
                    var sqlStatements = SplitSqlStatements(script);
                    Console.WriteLine($"Batched script into {sqlStatements.Count()} sql statements...");
                    foreach (var sqlLoop in sqlStatements)
                    {
                        sql = sqlLoop;
                        Console.WriteLine("Executing sql...");
                        _context.Database.ExecuteSqlCommand(sql);
                    }

                    Console.WriteLine("Sucessfully executed script!");
                }
                catch (Exception ex)
                {
                    errorString = $"Exception Message: {ex.Message} Failed Script: {scriptPath}";
                    Console.WriteLine("Failed to execute script:");
                    Console.WriteLine($"Exception message: {ex.Message}");
                    Console.WriteLine("    " + sql);
                    errors++;
                }

                Console.WriteLine("\n");
            }

            Console.WriteLine($"\n---Executed {scriptCount} scripts with {errors} errors---");
            return errorString;
        }

        private static IEnumerable<string> SplitSqlStatements(string sqlScript)
        {
            //fix any commented go statements
            sqlScript = sqlScript.Replace("--GO", "--").Replace("--go", "--");
            string[] splitter = { "\r\nGO\r\n", "\r\ngo\r\n" };
            // Split by "GO" statements
            var statements = sqlScript.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
            //new[] { "GO", "go", "Go", "gO" }
            // Remove empties, trim, and return
            return statements
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim(' ', '\r', '\n'));
        }
    }
}