using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public string Token { get; set; }
        public int Count { get; set; }
        public DateTime ExpirationTime { get; set; }
        public virtual User User { get; set; }

    }
}
