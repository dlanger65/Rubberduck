﻿using Rubberduck.Parsing.Grammar.Abstract.CodePathAnalysis;
using Rubberduck.Parsing.Symbols;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Rubberduck.CodeAnalysis.CodePathAnalysis.Execution
{
    public class CodePath
    {
        private readonly List<IExtendedNode> _nodes;
        private readonly Dictionary<IdentifierReference, Stack<IAssignmentNode>> _refs
            = new Dictionary<IdentifierReference, Stack<IAssignmentNode>>();
        private readonly Dictionary<IAssignmentNode, Stack<IEvaluatableNode>> _assignmentReads
            = new Dictionary<IAssignmentNode, Stack<IEvaluatableNode>>();
        private readonly List<IEvaluatableNode> _unassignedReads;

        public CodePath(IEnumerable<IExtendedNode> nodes = null, bool isErrorPath = false)
        {
            _nodes = new List<IExtendedNode>(nodes ?? Enumerable.Empty<IExtendedNode>());
            IsErrorPath = isErrorPath;
        }

        public bool IsErrorPath { get; }

        public IExtendedNode this[int index] 
            => _nodes[index];

        public IExtendedNode[] Nodes(int index) 
            => _nodes.ToArray();

        public int Count 
            => _nodes.Count;

        internal void OnAssignment(IdentifierReference reference)
        {
            Debug.Assert(reference.IsAssignment);
            if (_refs[reference] == null)
            {
                _refs[reference] = new Stack<IAssignmentNode>();
            }
            var assignment = LastAssignment(reference);
            _refs[reference].Push(assignment);
            _assignmentReads[assignment] = new Stack<IEvaluatableNode>();
        }

        internal void OnReference(IdentifierReference reference, IEvaluatableNode node)
        {
            if (reference.IsAssignment)
            {
                OnAssignment(reference);
            }
            else
            {
                var assignment = LastAssignment(reference);
                if (assignment != null && _assignmentReads.TryGetValue(assignment, out _))
                {
                    _assignmentReads[assignment].Push(node);
                }
                else if (assignment != null)
                {
                    // todo: flag unassigned read?
                }
            }
        }

        internal IAssignmentNode LastAssignment(IdentifierReference reference)
        {
            if (_refs.TryGetValue(reference, out var stack))
            {
                return stack?.Peek();
            }
            return null;
        }

        internal void Add(IExtendedNode node) 
            => _nodes.Add(node);

        internal void AddRange(IEnumerable<IExtendedNode> nodes)
        {
            foreach (var node in nodes)
            {
                Add(node);
            }
        }

        public IEvaluatableNode[] UnassignedReads 
            => _unassignedReads.ToArray();

        public IAssignmentNode[] UnreferencedAssignments 
            => _assignmentReads.Where(kvp => !kvp.Value.Any()).Select(kvp => kvp.Key).ToArray();

        public IExtendedNode[] UnreachableNodes 
            => _nodes.Where(node => !node.IsReachable).ToArray();
    }

    internal class MergedPath
    {
        private readonly List<CodePath> _paths = new List<CodePath>();

        public void Merge(CodePath path) => _paths.Add(path);

        public void Add(IExtendedNode node) => _paths.ForEach(path => path.Add(node));

        public CodePath[] Flatten() => _paths.ToArray();
    }
}
