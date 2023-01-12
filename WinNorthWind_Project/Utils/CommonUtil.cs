using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// 추가
using System.Windows.Forms;
using NorthWindDTO;
using System.Text.RegularExpressions; // 정규화 네임스페이스
using System.Drawing;
using System.ComponentModel;

namespace WinNorthWind_Project.Utils
{
    class CommonUtil
    {
        public static void ComboBinding(ComboBox cbo, List<ComboItemDTO> source,string category, bool blankItem = true, string blankText = " == Select == ")
        {
            // 확장메서드
            //var list = (from item in source
            //            where item.Category == category
            //            select item).ToList();

            //
            //var list = source.Where<ComboItemDTO>((element) => element.Category == category).ToList();
            
            // Linq
            var list = source.FindAll((element) => element.Category == category);

            if (blankItem == true)
            {
                ComboItemDTO newItem = new ComboItemDTO()
                {
                    Category = category,
                    Code = "", // Code는 출력하지 않기 때문에 빈값을 준다.
                    Name = blankText
                };
                list.Insert(0, newItem); // 콤보박스 출력시 0번째에 newItem(== 선택 ==)을 추가
            }
            
            cbo.DisplayMember = "Name";
            cbo.ValueMember = "Code";
            cbo.DataSource = list;
        }

        public static bool IsPhoneNumber(string input) // 전화번호 정규식 함수
        {
            string phonePattern = @"\d{2,3}-\d{3,4}-\d{4}";
            if (!Regex.IsMatch(input, phonePattern))
                return false;
            else
                return true;
        }

        public static string InputTextCheck(Panel pnl)
        {   // 반환 문자열이 빈 문자열이면 필수항목 모두 입력한 경우
            
            StringBuilder sb = new StringBuilder();

            foreach (Control ctrl in pnl.Controls)
            {
                if (ctrl is GudiTextBox txt)
                {
                    if (txt.InputType == validType.Required || txt.InputType == validType.RequiredNumeric)
                    {
                        if (string.IsNullOrWhiteSpace(txt.Text))
                        {
                            string msg = $"- {txt.Tag.ToString()}은 필수입력 항목입니다.";
                            sb.AppendLine(msg);
                        }
                    }
                }
            }
            return sb.ToString();
        }

        
        public static Image ByteToImage(byte[] data) //byte[] => image 변환
        {
            TypeConverter tc = TypeDescriptor.GetConverter(typeof(Bitmap));
            return (Bitmap)tc.ConvertFrom(data);
        }
        public static byte[] ImageToByte(Image img) //image => byte[]
        {
            ImageConverter ic = new ImageConverter();
            return (byte[])ic.ConvertTo(img, typeof(byte[]));
        }
    }
}
