using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.Data;
using Server.Models;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private RestaurantContext _context;

        public ReservationController(RestaurantContext context)
        {
            _context = context;
        }

        [Route("{id:int}")]
        public ActionResult<OrderTable> GetById(int id)
        {
            var order = _context.ReservationsTables.FirstOrDefault(p => p.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        [HttpPost]
        public ActionResult<OrderTable> Create(OrderTable orderTable)
        {
            _context.Add(orderTable);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = orderTable.Id }, orderTable);
        }

        [HttpPost, ActionName("Create")]
        public async Task<IActionResult> CreatePostAsync(OrderTable orderTable)
        {
            HttpClient httpclient = _httpClientFactory.CreateClient();
            HttpResponseMessage response = await httpclient.PostAsJsonAsync("http://localhost:54517/api/Reservation", orderTable);
            if (response.IsSuccessStatusCode)
            {
                OrderTable order = await response.Content.ReadAsAsync<OrderTable>();
                return RedirectToAction("ThankYouAsync", new { orderId = order.Id });
            }
            else
            {
                return View("Error");
            }
        }

        private async Task PopulateRestaurantBranchesDropDownListAsync()
        {
            HttpClient httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri("http://localhost:54517");
            HttpResponseMessage response = await httpClient.GetAsync("api/RestaurantBranches");
            if (response.IsSuccessStatusCode)
            {
                IEnumerable<RestaurantBranch> restaurantBranches = await response.Content.ReadAsAsync<IEnumerable<RestaurantBranch>>();
                ViewBag.RestaurantBranches = new SelectList(restaurantBranches, "Id", "City");
            }
        }
    }
}