﻿using Rubberduck.Parsing.Grammar.Abstract.CodePathAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubberduck.Parsing.Grammar
{
    public partial class VBAParser
    {
        public partial class GoToStmtContext : IJumpNode
        {
            public void Execute(IExecutionContext context)
            {
                IsReachable = true;
            }

            public bool IsReachable { get; set; }
            public IExtendedNode Target { get; set; }
        }

        public partial class OnErrorStmtContext : IJumpNode
        {
            public void Execute(IExecutionContext context)
            {
                IsReachable = true;
            }

            public bool IsReachable { get; set; }
            public IExtendedNode Target { get; set; }
        }

        public partial class ResumeStmtContext : IJumpNode
        {
            public void Execute(IExecutionContext context)
            {
                IsReachable = true;
            }

            public bool IsReachable { get; set; }
            public IExtendedNode Target { get; set; }
        }

        public partial class ReturnStmtContext : IJumpNode
        {
            public void Execute(IExecutionContext context)
            {
                IsReachable = true;
            }

            public bool IsReachable { get; set; }
            public IExtendedNode Target { get; set; }
        }

        public partial class GoSubStmtContext : IJumpReferenceNode
        {
            public void Execute(IExecutionContext context)
            {
                IsReachable = true;
            }

            public bool IsReachable { get; set; }
            public IExtendedNode Target { get; set; }
        }
    }
}
