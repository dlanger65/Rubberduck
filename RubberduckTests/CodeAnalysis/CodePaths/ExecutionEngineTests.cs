﻿using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using NUnit.Framework;
using Rubberduck.CodeAnalysis.CodePathAnalysis.Execution;
using Rubberduck.CodeAnalysis.CodePathAnalysis.Execution.ExtendedNodeVisitor;
using Rubberduck.Parsing.Grammar.Abstract.CodePathAnalysis;
using Rubberduck.Parsing.Symbols;
using RubberduckTests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubberduckTests.CodeAnalysis.CodePaths
{
    [TestFixture]
    public class ExecutionEngineTests
    {
        [Test][Category("CodePathAnalysis")]
        public void SingleCodePathInputYieldsSingleCodePath()
        {
            const string inputCode = @"Option Explicit
Public Sub DoSomething()
    MsgBox ""hello""
End Sub
";
            var paths = GetCodePaths(inputCode);
            Assert.AreEqual(1, paths.Count());
        }

        [Test][Category("CodePathAnalysis")]
        public void BranchingCodePathsInputYieldsTwoCodePaths()
        {
            const string inputCode = @"Option Explicit
Public Sub DoSomething()
    Debug.Print ""hi from path 1""
    If True Then
        MsgBox ""hello from path 2""
    End If
    Debug.Print ""still in path 1""
End Sub
";
            var paths = GetCodePaths(inputCode);
            Assert.AreEqual(2, paths.Count());
        }

        [Test][Category("CodePathAnalysis")]
        public void BranchingCodePathsInput_CodeAfterBranchIsInBothPaths()
        {
            const string inputCode = @"Option Explicit
Public Sub DoSomething()
    Debug.Print ""hi from path 1""
    If True Then
        MsgBox ""hello from path 2""
    End If
    Debug.Print ""I'm in both path 1 and path 2""
End Sub
";
            var paths = GetCodePaths(inputCode);
            if (paths.Count() != 2) { Assert.Inconclusive("Expecting 2 code paths"); }

            var lastNode = paths.Select(path => path[path.Count - 1]).ToArray();
            Assert.AreEqual("\"I'm in both path 1 and path 2\"", ((ParserRuleContext)lastNode[0]).GetText());
            Assert.AreEqual("\"I'm in both path 1 and path 2\"", ((ParserRuleContext)lastNode[1]).GetText());
        }

        [Test][Category("CodePathAnalysis")]
        public void ElseBlockIsCodePath()
        {
            const string inputCode = @"Option Explicit
Public Sub DoSomething()
    Debug.Print ""In path 1""
    If True Then
        MsgBox ""hello from path 2""
    Else
        Debug.Print ""hello from path 3""
    End If
End Sub
";
            var paths = GetCodePaths(inputCode);
            Assert.AreEqual(3, paths.Count());
        }

        [Test][Category("CodePathAnalysis")]
        public void ElseBlockHasParentPathAssignmentMetadata()
        {
            const string inputCode = @"Option Explicit
Public Sub DoSomething()
    Dim foo
    foo = 1
    If True Then
        foo = 2
    Else
        foo = 3
    End If
End Sub
";
            var paths = GetCodePaths(inputCode);
            foreach (var path in paths.Select((p, i) => (p, i)))
            {
                var fistAssignment = path.p.AllAssignments
                    .Where(a => a.Key.IdentifierName == "foo")
                    .Select(a => ((ParserRuleContext)a.Value.Last()).GetText())
                    .First();

                var lastAssignment = path.p.AllAssignments
                    .Where(a => a.Key.IdentifierName == "foo")
                    .Select(a => ((ParserRuleContext)a.Value.Peek()).GetText())
                    .Last();

                Assert.AreEqual("foo = 1", fistAssignment);
                Assert.AreEqual("foo = " + (path.i + 1).ToString(), lastAssignment);
            }
        }
        
        private IEnumerable<CodePath> GetCodePaths(string inputCode)
        {
            using (var state = MockParser.ParseString(inputCode, out var qmn))
            {
                var result = new List<CodePath>();
                foreach (var member in state.DeclarationFinder.Members(qmn))
                {
                    if (member is ModuleBodyElementDeclaration element)
                    {
                        var visitor = new ExtendedNodeVisitor(element, state);
                        result.AddRange(visitor.GetAllCodePaths());
                    }
                }
                return result;
            }
        }
    }
}