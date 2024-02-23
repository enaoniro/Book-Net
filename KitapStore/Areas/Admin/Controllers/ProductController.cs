using Kitap.DataAccess.Data;
using Kitap.DataAccess.Repository.IRepository;
using Kitap.Models.Models;
using Kitap.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Web.Mvc;

namespace KitapStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;

        }



        public IActionResult Index()
        {
            List<Product> products = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();

            return View(products);
        }

        public IActionResult Detail(int? productId)
        {


            Product? singleProduct = _unitOfWork.Product.Get(u => u.Id == productId);
               
              
            
            return View(singleProduct);
        }

        public IActionResult Add()
        {
          
            return View();
        }

        [HttpPost]

        public IActionResult Add(Product obj, IFormFile? file)
        {

            string wwwRootPath = _webHostEnvironment.WebRootPath;
            if(file != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string productPath = Path.Combine(wwwRootPath, @"images\product");

                using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                obj.ImageUrl = @"\images\product\" + fileName;


            }
            _unitOfWork.Product.Add(obj);
            _unitOfWork.Save();


                //create
                return RedirectToAction("Index");
            


            
            
            //else
            //{
            //    //update
            //    productVM.Product = _unitOfWork.Product.Get(u => u.Id == id, includeProperties: "ProductImages");
            //    return View(productVM);
            //}

        }


        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? productFromDb = _unitOfWork.Product.Get(u => u.Id == id, includeProperties:"Category");
            if (productFromDb == null)
            {
                return NotFound();
            }
            return View(productFromDb);
        }


        [HttpPost]
        public IActionResult Edit(Product obj, IFormFile? file)
        {

            string wwwRootPath = _webHostEnvironment.WebRootPath;
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string productPath = Path.Combine(wwwRootPath, @"images\product");

                if(!string.IsNullOrEmpty(obj.ImageUrl))
                {
                    var oldImagePath = Path.Combine(wwwRootPath,obj.ImageUrl.TrimStart('\\'));

                    if(System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                obj.ImageUrl = @"\images\product\" + fileName;


            }

           
                _unitOfWork.Product.Update(obj);
     
            
                _unitOfWork.Save();
               
            
            return RedirectToAction("Index");
            
        }



        public IActionResult Delete(int? id)
        {
            Product? productFromDb = _unitOfWork.Product.Get(u => u.Id == id);
            return View(productFromDb);
        }

        [HttpPost]
        public IActionResult Delete(Product obj)
        {
            if (obj == null)
            {
                return NotFound();
            }
            else
            {

                _unitOfWork.Product.Delete(obj);
                _unitOfWork.Save();
                RedirectToAction("Index");


            }
            return RedirectToAction("Index");
        }


        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> products = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new {data= products});
        }
        #endregion
    }
}
