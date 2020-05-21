using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ViewAndControllersParameters.Models;

namespace ViewAndControllersParameters.Controllers
{
    public class ItemsController : Controller
    {
        private readonly APPContext _context;

        public ItemsController(APPContext context)
        {
            _context = context;
        }

        // GET: Items
        public async Task<IActionResult> Index()
        {
            return View(await _context.Item.ToListAsync());
        }

        // GET: Items/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Item
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // GET: Items/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Items/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Image,Description")] Item item)
        {
            if (ModelState.IsValid)
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        // GET: Items/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Item.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Image,Description")] Item item)
        {
            if (id != item.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(item.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        // GET: Items/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Item
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.Item.FindAsync(id);
            _context.Item.Remove(item);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemExists(int id)
        {
            return _context.Item.Any(e => e.Id == id);
        }
        public IActionResult ParameterTest() {
            #region ViewData
            base.ViewData["ViewDataItem"] = new Item()
            {
                Id = 1,
                Description = "ViewData傳值",
                Image = "照片位置",
                Name = "商品一",
                Price = 100
            };

            #endregion

            #region ViewBag
            base.ViewBag.Item = new Item()
            {
                Id = 1,
                Description = "ViewBag傳值",
                Image = "照片位置",
                Name = "商品一",
                Price = 100
            };
            base.ViewBag.once = "也可以單傳一筆";
            #endregion

            #region TempData
            base.TempData["TempDataItem"] = new Item()
            {
                Id = 1,
                Description = "TempData傳值",
                Image = "照片位置",
                Name = "商品一",
                Price = 100
            };

            #endregion

            //#region Session: 服務器內存內容一般都在HttpContext
            //#endregion

            #region Model
            var newItem = new Item()
            {
                Id = 1,
                Description = "Model傳值",
                Image = "照片位置",
                Name = "商品一",
                Price = 100
            };
            #endregion
            TempData.Keep();

            return View(newItem);
        }
        [HttpPost]
        public ActionResult ParameterTest(Item item)
        {
            var getTempDataItem = TempData["TempDataItem"];
            Console.WriteLine(item.Description);
            return RedirectToAction("ParameterTest");
        }
    }
}
