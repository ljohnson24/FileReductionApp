using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserApp
{
    public class Data
    {
        private int linenumber;
        private List<string> listofcsvlines;

        public int Linenumber
        {
            get{return linenumber;}
            set{linenumber = value;}
        }

        public List<string> Listofcsvlines
        {
            get{return listofcsvlines;}
            set{listofcsvlines = value;}
        }

        public Data()
        {
            this.linenumber = 0;
            this.listofcsvlines = new List<string>();
        }

        
    }
}
