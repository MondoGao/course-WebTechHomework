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

        // ����ֵ��װ�࣬���˲��践�ص�ʵ������
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

        // Ϊ POST ���� Model Mapping ׼������
        public class OrderPostInfo
        {
            public int[] files;
        }

        public OrdersController(FileUploadContext context)
        {
            _context = context;
        }

        // ��ȡȫ���Ķ���
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


        // ���� id �������ȡ��Ӧ�Ķ������ļ��б�����
        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder([FromRoute] int id, [FromQuery] string password)
        {
            // ����������� File ����Ϣ�Ա�ʹ��
            var orders = _context.Order.Include(o => o.Files);
            var order = await orders.SingleOrDefaultAsync(m => m.ID == id);
            

            if (order == null)
            {
                return NotFound();
            }
            // ��֤����
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
            // ���� GUID ��ȡǰ��λ��Ϊ����
            string password = Guid.NewGuid().ToString().Split('-')[1];
            List<File> filesList = new List<File>();

            // �жϴ����� fileId �Ƿ���ڣ����������� order ʵ���������ϵ
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

            // �����ݿ�ͬ��
            _context.Order.Add(order);
            await _context.SaveChangesAsync();

            return Ok(new OrderReturnInfo(order, true));
        }

        // �ҵ���Ӧ id �� Order ��ɾ��
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