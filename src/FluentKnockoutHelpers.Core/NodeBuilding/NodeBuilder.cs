﻿using System;
using System.Text;
using FluentKnockoutHelpers.Core.AttributeBuilding;

namespace FluentKnockoutHelpers.Core.NodeBuilding
{
    public class NodeBuilder : AttributeBuilder
    {
        private Node _node;

        public NodeBuilder(Node node)
        {
            _node = node;
        }

        public string GetNodeBegin()
        {
            var sb = new StringBuilder();

            sb.Append(_node.BeginTagBegin);
            sb.Append(base.ToString());
            sb.Append(_node.BeginTagEnd);

            return sb.ToString();
        }

        public string GetNodeEnd()
        {
            if(!(_node is DisposeClosingNode))
                throw new InvalidOperationException();

            return ((DisposeClosingNode) _node).EndTag;
        }
    }
}