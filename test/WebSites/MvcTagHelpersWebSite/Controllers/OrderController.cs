// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Mvc;
using MvcTagHelpersWebSite.Models;

namespace MvcTagHelpersWebSite.Controllers
{
    public class OrderController : Controller
    {
        [HttpPost]
        public IActionResult Submit(Order order)
        {
            return View();
        }
    }
}