using AutoMapper;
using ExpressMeals.Contracts.ViewModels;
using ExpressMeals.Contracts.Wrappers;
using ExpressMeals.Domains.Entities;
using ExpressMeals.Infrastructures.Context;
using ExpressMeals.WebServer.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpressMeals.WebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppDataContext _context;
        private readonly IMapper _mapper;

        public OrdersController(AppDataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ApiResponse<IEnumerable<OrderVm>>> GetAllOrders()
        {
            List<OrderDto> order = new List<OrderDto>();
            IEnumerable<Order> orderHeaderList = await _context.Orders.ToListAsync();
            IEnumerable<OrderDetail> orderDetailList = await _context.OrderDetails.ToListAsync();

            foreach (Order header in orderHeaderList)
            {
                OrderDto orderDto = new()
                {
                    OrderHeader = header,
                    OrderDetails = orderDetailList.Where(u => u.OrderId == header.Id),
                };
                order.Add(orderDto);
            }
            var orders = _mapper.Map<IEnumerable<OrderDto>, IEnumerable<OrderVm>>(order);

            return new ApiResponse<IEnumerable<OrderVm>>(true, null, orders);
        }


        [HttpGet("{id}")]
        public async Task<ApiResponse<OrderVm>> GetOrder(int id)
        {
            OrderDto order = new()
            {
                OrderHeader = await _context.Orders.FirstOrDefaultAsync(u => u.Id == id),
                OrderDetails = _context.OrderDetails.Where(u => u.OrderId == id),
            };
            var orders = _mapper.Map<OrderDto, OrderVm>(order);

            return new ApiResponse<OrderVm>(true, null, orders);
        }


        

    }
}
