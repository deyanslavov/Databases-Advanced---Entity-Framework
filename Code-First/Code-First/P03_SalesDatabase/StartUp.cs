﻿namespace P03_SalesDatabase
{
    using Microsoft.EntityFrameworkCore;
    using P03_SalesDatabase.Data;
    using System;

    public class StartUp
    {
        public static void Main()
        {
            using (var db = new SalesContext())
            {
            }
        }
    }
}
