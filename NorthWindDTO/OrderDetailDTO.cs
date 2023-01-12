using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthWindDTO
{
    public class OrderDetailDTO
    {
        // DTO 용도 : 1. [주문하기]에서 [저장]으로 서비스에 전달할 목적
        //            2. 장바구니 목록으로 DataGridView에 바인딩할 목적  

        public int      OrderID         { get; set; }
        public string   CategoryName    { get; set; }
        public int      ProductID       { get; set; }
        public string   ProductName     { get; set; }
        public decimal  UnitPrice       { get; set; }
        public int      Quantity        { get; set; }

    }
}
