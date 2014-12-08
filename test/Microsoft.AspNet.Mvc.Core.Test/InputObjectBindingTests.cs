// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.AspNet.Mvc
{
    public class InputObjectBindingTests
    {
    }

    public class Person
    {
        public string Name { get; set; }
    }

    public class User : Person
    {
        [MinLength(5)]
        public string UserName { get; set; }
    }

    public class Customers
    {
        [Required]
        public List<User> Users { get; set; }
    }
}