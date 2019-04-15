using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAutogenerationTool.Models
{
    class ListItem
    {
        public string Text { get; set; }

        public string Value { get; set; }

        /// <summary>
        /// 返回ListItem中的Text值
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Text;
        }
    }
}
