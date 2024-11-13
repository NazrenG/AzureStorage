using AzureStorageLibrary;
using AzureStorageLibrary.Models;
using AzureStorageLibrary.Services;
using AzureStorageMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AzureStorageMVC.Controllers
{
    public class HomeController : Controller
    {
           private readonly INoSqlStorage<Product> _productService;
            private readonly INoSqlStorage<Store> _storeService;

        public HomeController(INoSqlStorage<Product> productService, INoSqlStorage<Store> storeService)
        {
            _productService = productService;
            _storeService = storeService;
        }

        public async Task<IActionResult> Index()
        {
            var stores = await _storeService.All();
            ViewBag.Stores = stores?.ToList() ?? new List<Store>();  
            return View();
        }


        [HttpPost]
            public async Task<IActionResult> SaveProduct(Product product)
            {
                if (!ModelState.IsValid)
                { 
                product.RowKey=Guid.NewGuid().ToString();
                    await _productService.Add(product);
                    return RedirectToAction("Index");
                }
                return View("Index");
            }
        
    }
}
