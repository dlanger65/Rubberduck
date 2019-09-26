﻿using Rubberduck.Parsing.Grammar.Abstract.CodePathAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubberduck.Parsing.Grammar.PartialExtensions
{
    public partial class VBAParser
    {
        public partial class BlockStmtContext : IExecutableNode
        {
            public void Execute(IExecutionContext context)
            {
                IsReachable = true;
            }

            public bool IsReachable { get; set; }
        }
    }
}