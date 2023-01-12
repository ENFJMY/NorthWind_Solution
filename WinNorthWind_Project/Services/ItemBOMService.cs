using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NorthWindDAO;
using NorthWindDTO;


namespace WinNorthWind_Project.Services
{
    class ItemBOMService
    {
        public List<ComboItemDTO> GetProductList()
        {
            ItemBOMDAO db = new ItemBOMDAO();
            List<ComboItemDTO> list = db.GetProductList();
            db.Dispose();

            return list;
        }
        public List<ItemBOMDTO> GetBOMList(string itemCD)
        {
            ItemBOMDAO db = new ItemBOMDAO();
            List<ItemBOMDTO> list = db.GetBOMList(itemCD);
            db.Dispose();

            return list;
        }
    }
}
