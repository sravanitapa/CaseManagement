using System;
using System.Collections.Generic;
using System.Text;

namespace CaseManagement.Infrastructure.Identity.Entities;

//Keeping role names in one place avoids typos like "Admin" vs "admin"
public static class Roles
{
    public const string Admin = "Admin";
    public const string User = "User";
}
