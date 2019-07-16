using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using HypermarketList.Models;

namespace HypermarketList.Controllers
{
    public class HomeController : Controller
    {
        HypermarketEntities db = new HypermarketEntities();

        // GET: Home
        public ActionResult Index(string name)
        {
            System.Diagnostics.Debug.WriteLine("name: " + name);
            if (name != null) { ViewBag.username = name; }
            else { ViewBag.username = ""; }
            return View();
        }

        /* 註冊 */
        public ActionResult Register()
        {
            ViewBag.username = "";
            return View();
        }

        [HttpPost]
        public ActionResult Register(Member m)
        {
            if (ModelState.IsValid)
            {
                string cmd = "select mId from Member";
                Sysdb sd = new Sysdb(cmd, "Member");
                DataSet MemoryData = new DataSet();
                MemoryData = sd.SelectDB();
                DataTable Member = MemoryData.Tables["Member"];

                /* 取得最後一筆資料 */
                string LastStr = Member.Rows[Member.Rows.Count - 1]["mId"].ToString().Substring(1);
                int sum = Int32.Parse(LastStr);
                sum++;
                m.mId = sum.ToString("000");

                db.Member.Add(m);
                db.SaveChanges();
                Member.Dispose();
                TempData["message"] = "註冊成功，請使用新帳號登入！";
                return RedirectToAction("AlertRegisterFinish");
            }
            return View(m);
        }

        public ActionResult AlertRegisterFinish()
        {
            return View();
        }

        /* 登入 */
        public ActionResult Login()
        {
            ViewBag.username = "";
            return View();
        }

        [HttpPost]
        public ActionResult Login(string Name, string Password)
        {
            try
            {
                var username = db.Member.Where(m => m.name == Name).FirstOrDefault();
                var pwd = db.Member.Where(m => m.password == Password).FirstOrDefault();
                if (username != null & pwd != null)
                {
                    TempData["code"] = 1;
                    TempData["message"] = "登入成功！";
                    TempData["username"] = Name;
                    return RedirectToAction("AlertLoginInfo");
                }
                else
                {
                    TempData["code"] = 0;
                    TempData["message"] = "帳號密碼錯誤，請再次檢查！";
                    return RedirectToAction("AlertLoginInfo");
                }
            }
            catch (Exception) { }
            ViewBag.username = "";
            return View();
        }

        public ActionResult AlertLoginInfo()
        {
            return View();
        }

        /* 登出 */
        public ActionResult Logout()
        {
            ViewBag.username = "";
            return RedirectToAction("Index");
        }

        /* 公司頁面 */
        public ActionResult Company(string name, string cName)
        {
            if (name != null) { ViewBag.username = name; }
            else { ViewBag.username = ""; }
            TempData["company"] = cName;
            TempData["username"] = name;
            return View();
        }

        /* 查詢頁面 */
        public ActionResult Search(string name, string CompanyName, string City, string BranchName)
        {
            //判斷使用者是否登入
            if (name != null) { ViewBag.username = name; }
            else { ViewBag.username = ""; }

            //判斷是否於首頁進行查詢
            if (CompanyName == null)
            {
                TempData["CompanyName"] = "";
                TempData["City"] = "";
                TempData["BranchName"] = "";
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("CompanyName: " + CompanyName.Trim());

                TempData["CompanyName"] = CompanyName;
                TempData["City"] = City;
                TempData["BranchName"] = BranchName;

                var id =
                    from c in db.Company
                    join b in db.Branch on c.cId equals b.cId
                    where c.cName == CompanyName.Trim()
                    select c.cId;

                if ((City == null || City.Trim() == "")
                    & (BranchName == null || BranchName.Trim() == "")) //只有公司名稱
                {
                    var todos = db.Branch
                        .Where(b => b.cId == id.FirstOrDefault()).ToList();
                    return View(todos);
                }
                else if ((City != null || City.Trim() != "")
                    & (BranchName == null || BranchName.Trim() == "")) //公司名稱 + 縣市
                {
                    var todos = db.Branch
                        .Where(b => b.cId == id.FirstOrDefault()
                            & b.address.Substring(0, 3) == City.Trim()).ToList();
                    return View(todos);
                }
                else if ((City == null || City.Trim() == "")
                    & (BranchName != null || BranchName.Trim() != "")) //公司名稱 + 分店名稱
                {
                    var todos = db.Branch
                        .Where(b => b.cId == id.FirstOrDefault()
                            & b.bName.Contains(BranchName.Trim())).ToList();
                    return View(todos);
                }
                else //三種都有
                {
                    var todos = db.Branch
                        .Where(b => b.cId == id.FirstOrDefault()
                            & b.address.Substring(0, 3) == City.Trim()
                            & b.bName.Contains(BranchName.Trim())).ToList();
                    return View(todos);
                }
            }
            return View();
        }

        public ActionResult Edit(string name, string bNo)
        {
            if (name != null) { ViewBag.username = name; }
            else { ViewBag.username = ""; }

            var todos = db.Branch
                .Where(b => b.bNo == bNo).FirstOrDefault();
            return View(todos);
        }

        [HttpPost]
        public ActionResult Edit(string name, string CompanyName, string City, string BranchName,
            string bNo, string bName, string phone, string address, DateTime startTime, DateTime endTime)
        {
            //判斷使用者是否登入
            if (name != null) { ViewBag.username = name; }
            else { ViewBag.username = ""; }

            System.Diagnostics.Debug.WriteLine("CompanyName2: " + CompanyName.Trim());

            /* 修改資料 */
            if (name != null) { ViewBag.username = name; }
            else { ViewBag.username = ""; }
            System.Diagnostics.Debug.WriteLine("name3: " + name.Trim());
            var edit = db.Branch
                .Where(b => b.bNo == bNo).FirstOrDefault();
            edit.bName = bName;
            edit.phone = phone;
            edit.address = address;
            edit.startTime = startTime;
            edit.endTime = endTime;
            db.SaveChanges();
            return RedirectToAction("Search",
                new { name, CompanyName, City, BranchName });
        }

        public ActionResult Delete(string name, string bNo, string CompanyName, string City, string BranchName)
        {
            var todo = db.Branch.Where(b => b.bNo == bNo).FirstOrDefault();
            db.Branch.Remove(todo);
            db.SaveChanges();
            return RedirectToAction("Search",
                new { name, CompanyName, City, BranchName });
        }
    }
}