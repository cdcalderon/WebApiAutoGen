using System.Web.Mvc;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Interfaces;

namespace YPrime.StudyPortal.Controllers
{
    public class SyncLogController : BaseController
    {
        private ISyncLogRepository _SyncLogRepository;

        public SyncLogController(
            ISyncLogRepository SyncLogRepository,
            ISessionService sessionService)
            : base(sessionService)
        {
            _SyncLogRepository = SyncLogRepository;
        }

        // GET: SyncLog
        public ActionResult Index()
        {
            return View();
        }

        // GET: SyncLog/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: SyncLog/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SyncLog/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: SyncLog/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: SyncLog/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: SyncLog/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: SyncLog/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}