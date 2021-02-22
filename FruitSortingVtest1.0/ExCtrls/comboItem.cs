using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FruitSortingVtest1.ExCtrls
{
    public class comboItem : object
    {
        private Image img = null;
        private string text = null;

        public comboItem()
        {

        }

        public comboItem(string text)
        {
            Text = text;
        }

        public comboItem(string text, Image img)
        {
            Text = text;
            Img = img;
        }

        // item Img
        public Image Img
        {
            get
            {
                return img;
            }
            set
            {
                img = value;
            }
        }

        // item text
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
            }
        }

        // ToString() should return item text
        public override string ToString()
        {
            return text;
        }
    }
}
