﻿namespace P01_HospitalDatabase
{
    using P01_HospitalDatabase.Data;
    using P01_HospitalDatabase.Data.Models;
    using System;

    public class StartUp
    {
        public static void Main()
        {
            using (var db = new HospitalContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }
        }
    }
}
