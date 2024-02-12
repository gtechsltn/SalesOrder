﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using System.Data.SqlClient;
using System.Data;
using Dapper;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class InsertController : Controller
    {
        [HttpGet]
        [Route("InsertElement")]
        [Route("[controller]/[action]")]
        public IActionResult Insert()
        {
            return View();
        }
        [HttpPost]
        [OutputCache(Duration = 60)]
        [Route("InsertElement")]
        [Route("[controller]/[action]")]
        public async Task<IActionResult> Insert(Orders fc)
        {
            if (ModelState.IsValid)
            {
                int id = fc.orderid;
                DateTime date = fc.orderdate;
                string prodid = fc.ProductId;
                int uid = fc.UserId;

                string temp = Convert.ToString(date);
                string[] temp1 = new string[2];
                temp1 = temp.Split(" ");
                string dat = temp1[0];

                string conn = "Server=192.168.0.23,1427;Initial Catalog=interns;Integrated Security=False;user id=Interns;password=test;";
                using (IDbConnection sql = new SqlConnection(conn))
                {
                    string sqlstring = "insert into Orders(orderid,orderdate,ProductId,UserId) values(@id,@date,@pid,@uid)";
                    await sql.ExecuteAsync(sqlstring, new { id = id, date = dat, pid = prodid, uid = uid });
                    sqlstring = "Exec updatequantity @uid,@oid";
                    await sql.ExecuteAsync(sqlstring, new { uid = uid, oid = id });
                    sqlstring = "Exec updaterealprice @oid";
                    await sql.ExecuteAsync(sqlstring, new { oid = id });
                    Console.WriteLine("Success");
                }

                Console.WriteLine($"{id}  {dat}\t\t{prodid}  {uid}");

                return RedirectToAction("Index", "Order");
            }
            return View();

        }
    }
}
