using System;
using System.Collections.Generic;
using System.Text;

namespace GeekBurguer.Ingredients.Contracts.Messages
{
    public class LabelImageAddedMessage
    {
        public string ItemName { get; set; }
        public List<string> Ingredients { get; set; }
    }
}
