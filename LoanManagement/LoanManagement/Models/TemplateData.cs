using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoanManagement.Models
{
    public class TemplateData
    {
        private string _id;

        public string id
        {
            get { return _id; }
            set { _id = value; }
        }
        private string _text;

        public string text
        {
            get { return _text; }
            set { _text = value; }
        }
    }
}