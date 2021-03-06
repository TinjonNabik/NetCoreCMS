﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NetCoreCMS.Framework.Core.Data;
using NetCoreCMS.ImageSlider.Models;
using NetCoreCMS.HelloWorld.Models;

namespace NetCoreCMS.ImageSlider.Models
{
    public class HelloModelBuilder : INccModuleBuilder
    {
        public void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HelloModel>().ToTable("Ncc_Hello");
        }
    }
}
