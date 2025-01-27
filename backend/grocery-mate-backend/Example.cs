﻿using Microsoft.AspNetCore.Server.IIS.Core;

namespace grocery_mate_backend;

public class Example
{
    public User MadeBy { get; set; }

    public bool CanBeCanceled(User user)
    {
        if (user.IsAdmin /* || MadeBy == user */)
            return true;
        if (MadeBy == user)
            return true;

        return false;
    }
}

public class User
{
    public bool IsAdmin { get; set; }
}