using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FileUpload.Models;

namespace FileUpload.Controllers
{
    [Produces("application/json")]
    [Route("api/Orders")]
    public class OrdersController : Controller
    {
        private readonly FileUploadContext _context;

        // 返回值包装类，过滤不需返回的实体属性
        private class OrderReturnInfo
        {
            public int id;
            public string generateTime;
            public string password;
            public ICollection<FilesController.FileReturnInfo> files;

            public OrderReturnInfo(Order order, bool isShowPassword = false)
            {
                id = order.ID;
                generateTime = order.GenerateTime.ToString("G");
                if (isShowPassword)
                {
                    password = order.Password;
                }

                List<FilesController.FileReturnInfo> emitedList = new List<FilesController.FileReturnInfo> { };
                foreach (var file in order.Files.ToList())
                {
                    emitedList.Add(new FilesController.FileReturnInfo(file));
                }

                files = emitedList;
            }
        }

        // 为 POST 动作 Model Mapping 准备的类
        public class OrderPostInfo
        {
            public int[] files;
        }

        public OrdersController(FileUploadContext context)
        {
            _context = context;
        }

        // 获取全部的订单
        // GET: api/Orders
        [HttpGet]
        public IActionResult GetOrder()
        {
            var orders = _context.Order.Include(o => o.Files);

            List<OrderReturnInfo> emitedList = new List<OrderReturnInfo>();
            foreach (var order in orders)
            {
                emitedList.Add(new OrderReturnInfo(order));
            }
            return Ok(emitedList);
        }


        // 根据 id 和密码获取对应的订单的文件列表及详情
        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder([FromRoute] int id, [FromQuery] string password)
        {
            // 读入相关联的 File 的信息以便使用
            var orders = _context.Order.Include(o => o.Files);
            var order = await orders.SingleOrDefaultAsync(m => m.ID == id);
            

            if (order == null)
            {
                return NotFound();
            }
            // 验证密码
            else if (order.Password != password)
            {
                return Forbid();
            }

            return Ok(new OrderReturnInfo(order));
        }

        // POST: api/Orders
        [HttpPost]
        public async Task<IActionResult> PostOrder([FromBody]OrderPostInfo orderInfo)
        {
            DateTime generateTime = DateTime.Now;
            // 生成 GUID 并取前四位作为密码
            string password = Guid.NewGuid().ToString().Split('-')[1];
            List<File> filesList = new List<File>();

            // 判断传来的 fileId 是否存在，存在则建立与 order 实体的所属关系
            foreach (var fileId in orderInfo.files)
            {
                var file = await _context.File.SingleOrDefaultAsync(m => m.ID == fileId);
                if (file != null && file.Order == null)
                {
                    filesList.Add(file);
                }
            }
            if (filesList.Count < 1)
            {
                return BadRequest();
            }

            Order order = new Order { GenerateTime = generateTime, Password = password, Files = filesList };

            // 与数据库同步
            _context.Order.Add(order);
            await _context.SaveChangesAsync();

            return Ok(new OrderReturnInfo(order, true));
        }

        // 找到对应 id 的 Order 并删除
        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = await _context.Order.SingleOrDefaultAsync(m => m.ID == id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Order.Remove(order);
            await _context.SaveChangesAsync();

            return Ok(order);
        }

        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.ID == id);
        }
    }
}