﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai
{
    public class NullSink : Sink<object>
    {
        public override void Process(object input)
        {
        }
    }
}
