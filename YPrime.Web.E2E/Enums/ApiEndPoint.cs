using YPrime.Config.Enums;

namespace YPrime.Web.E2E.Enums
{
    public class ApiEndPoint : Enumeration<string>
    {

        public ApiEndPoint(
            string Id,
            string Name)
            : base(Id, Name)
        {

        }

        public static readonly ApiEndPoint SyncInitialData = new ApiEndPoint(
            "SyncInitialClientData",
            "sync initial data");

        public static readonly ApiEndPoint SyncClientData = new ApiEndPoint(
            "SyncClientData",
            "sync client data");

        public static readonly ApiEndPoint CheckForUpdates = new ApiEndPoint(
            "CheckForUpdates",
            "check for updates");
    }
}
