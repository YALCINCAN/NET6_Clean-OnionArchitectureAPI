using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Settings
{
    public class CacheSettings
    {
        public bool PreferRedis { get; set; }
        public string RedisURL { get; set; }
        public int RedisPort { get; set; }
    }
}
