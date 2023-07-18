namespace YPrime.BusinessLayer.Interfaces
{
    public interface ISyncSQLBuilder
    {
        string GetSQLForTable(
            string TableName, 
            string activePatientStatusIds);
    }
}