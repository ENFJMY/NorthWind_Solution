using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// 호출하기 위한 추가
using NorthWindDAO; 
using NorthWindDTO;

namespace WinNorthWind_Project.Services
{
    class OrderService
    {
        public List<ComboItemDTO> GetCommonCode(string[] category)
        {
            CommonDAO db = new CommonDAO();
            List<ComboItemDTO> list = db.GetCodeByCategory(category);
            db.Dispose();

            return list;
        }
        public List<ProductInfoDTO> GetProductAllList()
        {
            OrderDAO db = new OrderDAO();
            List<ProductInfoDTO> list = db.GetProductAllList();
            db.Dispose();

            return list;
        }

        public bool AddOrder(OrderDTO order, List<OrderDetailDTO> cartList) // OrderDAO에서 복사해온다.
        {
            OrderDAO db = new OrderDAO();
            bool result = db.AddOrder(order, cartList);
            db.Dispose();

            return result;
        }

        public List<OrderDTO> GetOrderSearchList(string fromDt, string toDt, string customerID, int employeeID) // OrderDAO에서 복사해온다.
        {
            OrderDAO db = new OrderDAO();
            List<OrderDTO> list = db.GetOrderSearchList(fromDt, toDt, customerID, employeeID);
            db.Dispose();

            return list;
        }

        public List<OrderDetailDTO> GetOrderDetail(int orderID)
        {
            OrderDAO db = new OrderDAO();
            List<OrderDetailDTO> list = db.GetOrderDetail(orderID);
            db.Dispose();

            return list;
            // --> frmOrder로 가서 Service 호출
        }

        public bool UpdateOrder(OrderDTO order)
        {
            OrderDAO db = new OrderDAO();
            bool result = db.UpdateOrder(order);
            db.Dispose();

            return result;
        }

        public bool DeleteOrder(int orderID)
        {
            OrderDAO db = new OrderDAO();
            bool result = db.DeleteOrder(orderID);
            db.Dispose();

            return result;
        }
    }
}
