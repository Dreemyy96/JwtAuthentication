using JWTAuthentication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace JWTAuthentication.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<IEnumerable<Product>>> Get()
        {
            try
            {
                var products = await _context.Products.ToListAsync();
                return Ok(products);
            }catch (Exception ex)
            {
                return StatusCode(500, "Internal server error" + ex.Message);
            }
            
        }
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Product>> GetAsync(int id)
        {
            try
            {
                Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
                return Ok(product);
            }catch(Exception ex)
            {
                return StatusCode(500, "Internal server error" + ex.Message);
            }
        }
        [HttpPost]
        public async Task<ActionResult<Product>> Post([FromBody]Product product)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _context.Products.AddAsync(product);
            try
            {
                await _context.SaveChangesAsync();
            }catch (DbUpdateException ex)
            {
                return StatusCode(500, "Failed to save product" + ex.Message);
            }

            return Ok(product);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> Delete(int id)
        {
            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            _context.Products.Remove(product);
            try
            {
               await _context.SaveChangesAsync();
            }catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            return Ok(product);
        }
        [HttpPut]
        public async Task<ActionResult<Product?>> Put([FromBody]Product product)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Product res = await _context.Products.FirstOrDefaultAsync(p => p.Id == product.Id);

            if(res != null)
            {
                res.Name = product.Name;
                res.Description = product.Description;
                res.Price = product.Price;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, "Failed to save changed product" + ex.Message);
            }

            return Ok(res);
        }
        [HttpPatch("{id}")]
        public async Task<ActionResult<Product>> Patch (int id, [FromBody] ProductPatch productPatch)
        {
            //Product? product = Get(id);
            //if(product != null)
            //{
            //    patch.ApplyTo(product);
            //    _context.Products.Update(product);
            //    return Ok(product);
            //}
            //_context.SaveChanges();
            //return NotFound();

            var product =  await GetAsync(id);
            if(product.Result is StatusCodeResult)
            {
                return BadRequest(product.Result);
            }

            Product? res = product.Value;
            if( res != null)
            {
                if(productPatch.Name != null)
                    res.Name = productPatch.Name;
                if(productPatch.Description != null)
                    res.Description = productPatch.Description;
                if(productPatch.Price != null)
                    res.Price = productPatch.Price.Value;

                _context.Products.Update(res);

                try
                {
                    await _context.SaveChangesAsync();
                }catch (DbUpdateException ex)
                {
                    return StatusCode(500, ex.Message);
                }
                
                return Ok(res);
            }
            return NotFound();
        }
    }
}
