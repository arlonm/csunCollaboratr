using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CollaboratR
{
    [Flags]
    public enum RoomPermissions
    {
        NULL = 0,
        READ = 1,
        WRITE = 2,
        ADMIN = 4
    }
}