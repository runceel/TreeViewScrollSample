using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TreeViewScrollSample
{
    public class Person
    {
        public string Name { get; set; }
        public IEnumerable<Person> Children { get; set; }
    }
}
