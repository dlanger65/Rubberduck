﻿using Rubberduck.Parsing.Annotations;
using Rubberduck.Parsing.Rewriter;
using Rubberduck.Parsing.Symbols;
using Rubberduck.Parsing.VBA;

namespace Rubberduck.UI.Command.Annotations
{
    public sealed class AnnotateTestModuleCommand : AnnotateCommandBase
    {
        public AnnotateTestModuleCommand(IRewritingManager manager, IAnnotationUpdater updater) 
            : base(manager, updater) { }

        protected override IAnnotation GetAnnotation(Declaration target)
            => new TestModuleAnnotation();
    }
}
