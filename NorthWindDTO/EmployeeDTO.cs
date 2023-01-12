using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthWindDTO
{
    public class EmployeeDTO
    {//                                          DB의 데이터 타입  |   C# 데이터 타입                       
        public int      EmployeeID      { get; set; } // int         => int 
        public string   FirstName       { get; set; }
        public string   LastName        { get; set; }
        public string   Title           { get; set; } // Varchar    => string
        public string   BirthDate       { get; set; }
        public string   HireDate        { get; set; } // Datetime   => DateTime or string
        public string   HomePhone       { get; set; }
        public string   Notes           { get; set; } // nText      => string
        public byte[]   Photo           { get; set; } // image      => byte[]
    }
}
