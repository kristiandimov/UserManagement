using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Entities
{
    public class Task
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int StoryPoints { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime UpdatedTime { get; set; }
        public virtual Project Project { get; set; }
        public virtual User User { get; set; }

    }
}
