﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eng1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var a = new ScheduleGenerator();
            a.GenerateSchedule();
            a.ScheduleParseToJson();
            a.ScheduleParseFromJson(@"Schedule1.json");
        }
    }
}
