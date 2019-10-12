using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WebApiCoreDapper.Dtos;
using WebApiCoreDapper.Fillters;
using WebApiCoreDapper.Models;

namespace WebApiCoreDapper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : BaseController
    {
        private readonly string db;
        public ProductController(IConfiguration configuration, ILogger<ProductController> logger)
            : base(logger)
        {
            db = configuration.GetConnectionString("DbConnectionString");
        }
        // GET: api/Product
        [HttpGet(Name = "GetAll")]
        public async Task<IEnumerable<Product>> Get()
        {
            this.logger.LogTrace("test log product controller");
            using (var conect=new SqlConnection(db))
            {
                if(conect.State==System.Data.ConnectionState.Closed)
                {
                    conect.Open();
                }
                var result = await conect.QueryAsync<Product>("Get_Product_All", null, null, null, System.Data.CommandType.StoredProcedure);
                return result;

            }
        }

        // GET: api/Product/5
        [HttpGet("{id}", Name = "GetById")]
        public async Task<Product> Get(int id)
        {
            using (var conect = new SqlConnection(db))
            {
                if (conect.State == System.Data.ConnectionState.Closed)
                {
                    conect.Open();
                }
                var parameter = new DynamicParameters();
                parameter.Add("@id", id);
                var result = await conect.QueryAsync<Product>("GetProductById",parameter,null, null, System.Data.CommandType.StoredProcedure);
                return result.Single();

            }
        }
         // GET: api/Product/5
        [HttpGet("paging",Name = "GetPaging")]
        public async Task<PagedResult<Product>> GetPaging(string keyword,int categoryId,int pageIndex,int pageSize)
        {
            using (var conect = new SqlConnection(db))
            {
                if (conect.State == System.Data.ConnectionState.Closed)
                {
                    conect.Open();
                }
                var parameter = new DynamicParameters();
                parameter.Add("@keyword", keyword);
                parameter.Add("@categoryId", categoryId);
                parameter.Add("@pageIndex", pageIndex);
                parameter.Add("@pageSize", pageSize);
                parameter.Add("@totalRow", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);
                var result = await conect.QueryAsync<Product>("Get_Product_AllPaging",parameter,null, null, System.Data.CommandType.StoredProcedure);
                var totalRow = parameter.Get<int>("totalRow");
                var pagedResult = new PagedResult<Product>
                {
                    Items = result.ToList(),
                    TotalRow = totalRow,
                    PageIndex = pageIndex,
                    PageSize = pageSize
                };
                return pagedResult;
            }
        }

        // POST: api/Product
        [HttpPost]
        public async Task<int> Post([FromBody] Product product)
        {

            var newId = 0;
            using (var conect = new SqlConnection(db))
            {
                if (conect.State == System.Data.ConnectionState.Closed)
                {
                    conect.Open();
                }
                var parameter = new DynamicParameters();
                parameter.Add("@sku", product.Sku);
                parameter.Add("@price", product.Price);
                parameter.Add("@imageUrl", product.ImageUrl);
                parameter.Add("@isActive", product.IsActive);
                //báo rằng trong thủ tục này có 1 biến output là id(tức để lấy ra id)
                parameter.Add("@id",dbType:System.Data.DbType.Int32,direction:System.Data.ParameterDirection.Output);
                var result=await conect.ExecuteAsync("Create_Product", parameter, null, null, System.Data.CommandType.StoredProcedure);

                newId = parameter.Get<int>("@id");
            }
            return newId;
        }

        // PUT: api/Product/5
        [HttpPut("{id}")]
        [ValidateModel]
        public async Task<IActionResult> Put(int id, [FromBody] Product product)
        {
            using (var conect = new SqlConnection(db))
            {
                if (conect.State == System.Data.ConnectionState.Closed)
                {
                    conect.Open();
                }
                var parameter = new DynamicParameters();
                parameter.Add("@id", id);
                parameter.Add("@sku", product.Sku);
                parameter.Add("@price", product.Price);
                parameter.Add("@imageUrl", product.ImageUrl);
                parameter.Add("@isActive", product.IsActive);
                await conect.ExecuteAsync("Update_Product", parameter, null, null, System.Data.CommandType.StoredProcedure);
                int newId = parameter.Get<int>("@id");
                return Ok(newId);
            }
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            using (var conect = new SqlConnection(db))
            {
                if (conect.State == System.Data.ConnectionState.Closed)
                {
                    conect.Open();
                }
                var parameter = new DynamicParameters();
                parameter.Add("@id", id);
                await conect.ExecuteAsync("Delete_Product", parameter, null, null, System.Data.CommandType.StoredProcedure);
            }
        }
    }
}
