﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Rubberduck.Inspections.Abstract;
using Rubberduck.Inspections.Resources;
using Rubberduck.Inspections.Results;
using Rubberduck.Parsing.Symbols;
using Rubberduck.Parsing.VBA;

namespace Rubberduck.Inspections
{
    public sealed class HungarianNotationInspection : InspectionBase
    {
        #region statics
        private static readonly List<string> HungarianPrefixes = new List<string>
        {
            "chk",
            "cbo",
            "cmd",
            "btn",
            "fra",
            "img",
            "lbl",
            "lst",
            "mnu",
            "opt",
            "pic",
            "shp",
            "txt",
            "tmr",
            "chk",
            "dlg",
            "drv",
            "frm",
            "grd",
            "obj",
            "rpt",
            "fld",
            "idx",
            "tbl",
            "tbd",
            "bas",
            "cls",
            "g",
            "m",
            "bln",
            "byt",
            "col",
            "dtm",
            "dbl",
            "cur",
            "int",
            "lng",
            "sng",
            "str",
            "udt",
            "vnt",
            "var",
            "pgr",
            "dao",
            "b",
            "by",
            "c",
            "chr",
            "i",
            "l",
            "s",
            "o",
            "n",
            "dt",
            "dat",
            "a",
            "arr"
        };

        private static readonly Regex HungarianIdentifierRegex = new Regex(string.Format("^({0})[A-Z0-9].*$", string.Join("|", HungarianPrefixes)));

        private static readonly List<DeclarationType> TargetDeclarationTypes = new List<DeclarationType>
        {
            DeclarationType.Parameter,
            DeclarationType.Constant,
            DeclarationType.Control,
            DeclarationType.ClassModule,
            DeclarationType.Member,
            DeclarationType.Module,
            DeclarationType.ProceduralModule,
            DeclarationType.UserForm,
            DeclarationType.UserDefinedType,
            DeclarationType.UserDefinedTypeMember,
            DeclarationType.Variable
        };

        #endregion

        public HungarianNotationInspection(RubberduckParserState state, CodeInspectionSeverity defaultSeverity = CodeInspectionSeverity.Suggestion)
            : base(state, defaultSeverity)
        {
        }

        public override string Description
        {
            get { return InspectionsUI.HungarianNotationInspectionName; }
        }

        public override CodeInspectionType InspectionType
        {
            get { return CodeInspectionType.MaintainabilityAndReadabilityIssues; }
        }

        public override IEnumerable<InspectionResultBase> GetInspectionResults()
        {
            var hungarians = UserDeclarations
                            .Where(declaration => TargetDeclarationTypes.Contains(declaration.DeclarationType) &&
                                        HungarianIdentifierRegex.IsMatch(declaration.IdentifierName))
                            .Select(issue => new HungarianNotationInspectionResult(this, issue))
                            .ToList();
            return hungarians;
        }
    }
}
