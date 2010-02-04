﻿using System;
using System.Collections.Generic;
using Antlr.Runtime.Tree;
using IronJS.Runtime2.Binders;
using IronJS.Runtime2.Js;
using IronJS.Tools;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Utils;

namespace IronJS.Compiler.Ast
{
    using AstUtils = Microsoft.Scripting.Ast.Utils;
    using Et = Expression;

    public class ArrayNode : Node, INode
    {
        public List<INode> Values { get; protected set; }

        public ArrayNode(List<INode> values, ITree node)
            : base(NodeType.Array, node)
        {
            Values = values;
        }

        public override Type ExprType
        {
            get
            {
                return IjsTypes.Object;
            }
        }

        public override INode Analyze(FuncNode astopt)
        {
            for (int i = 0; i < Values.Count; ++i)
                Values[i] = Values[i].Analyze(astopt);

            return this;
        }
    }
}