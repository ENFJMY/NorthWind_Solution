using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthWindDTO
{
    public class ItemBOMDTO
    {
        public string ITEM_CD { get; set; }
        public string ITEM_NM { get; set; }
        public decimal QTY { get; set; }
        public string ITEM_NM2 { get; set; }
        public int levels { get; set; }
        public string sortOrder { get; set; }

    }
}
