using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using StoreDataAccessLayer.Entities;

namespace StoreDataLayer.Entities
{
    public class SearchingLogs
    {
        public int Id { get; set; }
        public string SearchKeyWord { get; set; } = string.Empty;
        public DateTime SearchDate { get; set; }
        public int? ClientId { get; set; }
        public Client? Client
        {
            get;
            set;
        } 
    }
}
