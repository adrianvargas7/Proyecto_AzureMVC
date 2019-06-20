using jQueryAjaxInAsp.NETMVC.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace jQueryAjaxInAsp.NETMVC.Controllers
{
    public class RegistrosController : Controller
    {
        // GET: Registro
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewAll()
        {
            return View(GetAllRegistro());
        }

        IEnumerable<Registro> GetAllRegistro()
        {
            using (DBModel5 db = new DBModel5())
            {
                return db.Registroes.ToList<Registro>();
            }
        }

        public ActionResult AddOrEdit(int id = 0)
        {
            Registro reg = new Registro();
            if (id != 0)
            {
                using (DBModel5 db = new DBModel5())
                {
                    reg = db.Registroes.Where(x => x.Id == id).FirstOrDefault<Registro>();
                }
            }
            return View(reg);
        }

        [HttpPost]
        public ActionResult AddOrEdit(Registro reg)
        {
            try
            {
                if (reg.CargarImagen != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(reg.CargarImagen.FileName);
                    string extension = Path.GetExtension(reg.CargarImagen.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    reg.Foto = "~/AppFiles/Imagenes/" + fileName;
                    reg.CargarImagen.SaveAs(Path.Combine(Server.MapPath("~/AppFiles/Imagenes/"), fileName));
                }
                using (DBModel5 db = new DBModel5())
                {
                    if (reg.Id == 0)
                    {
                        db.Registroes.Add(reg);
                        db.SaveChanges();
                    }
                    else
                    {
                        db.Entry(reg).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                }
                return Json(new { success = true, html = GlobalClass.RenderRazorViewToString(this, "ViewAll", GetAllRegistro()), message = "Datos enviados con exito" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Delete(int id)
        {
            try
            {
                using (DBModel5 db = new DBModel5())
                {
                    Registro reg = db.Registroes.Where(x => x.Id == id).FirstOrDefault<Registro>();
                    db.Registroes.Remove(reg);
                    db.SaveChanges();
                }
                return Json(new { success = true, html = GlobalClass.RenderRazorViewToString(this, "ViewAll", GetAllRegistro()), message = "Registro eliminado con exito." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}