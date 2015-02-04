using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CollaboratR.Models
{
    public class ModuleModel
    {
        public int ModuleId { get; set; }
        public int ModuleX { get; set; }
        public int ModuleY { get; set; }
        public int ModuleWidth { get; set; }
        public int ModuleHeight { get; set; }
        public int ModulePermissions { get; set; }
        public String ModuleContent { get; set; }
        public int ModuleTypeId { get; set; }
        public String ModuleRaw { get; set; }
    }
}