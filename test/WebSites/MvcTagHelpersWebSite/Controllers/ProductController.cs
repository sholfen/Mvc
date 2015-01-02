// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Mvc;
using MvcTagHelpersWebSite.Models;

namespace MvcTagHelpersWebSite.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Submit(Product product)
        {
            return View();
        }

        public IActionResult List()
        {
            return View();
        }
    }
}