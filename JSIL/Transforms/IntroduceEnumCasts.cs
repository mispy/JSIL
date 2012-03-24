﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ICSharpCode.Decompiler.ILAst;
using JSIL.Ast;
using JSIL.Internal;
using Mono.Cecil;

namespace JSIL.Transforms {
    public class IntroduceEnumCasts : JSAstVisitor {
        public readonly TypeSystem TypeSystem;
        public readonly TypeInfoProvider TypeInfo;

        private readonly HashSet<JSOperator> LogicalOperators;

        public IntroduceEnumCasts (TypeSystem typeSystem, TypeInfoProvider typeInfo) {
            TypeSystem = typeSystem;
            TypeInfo = typeInfo;

            LogicalOperators = new HashSet<JSOperator>() {
                JSOperator.LogicalAnd,
                JSOperator.LogicalOr,
                JSOperator.LogicalNot
            };
        }

        public void VisitNode (JSIndexerExpression ie) {
            var indexType = ie.Index.GetExpectedType(TypeSystem);

            if (
                !ILBlockTranslator.IsIntegral(indexType) &&
                ILBlockTranslator.IsEnum(indexType)
            ) {
                var cast = JSInvocationExpression.InvokeMethod(
                    new JSFakeMethod("valueOf", TypeSystem.Int32, indexType), ie.Index, null, true
                );

                ie.ReplaceChild(ie.Index, cast);
            }

            VisitChildren(ie);
        }

        public void VisitNode (JSUnaryOperatorExpression uoe) {
            var type = uoe.Expression.GetExpectedType(TypeSystem);
            var isEnum = ILBlockTranslator.IsEnum(type);

            // Detect attempts to perform boolean logic on enums.
            if (isEnum && LogicalOperators.Contains(uoe.Operator)) {
                var cast = JSInvocationExpression.InvokeMethod(
                    new JSFakeMethod("valueOf", TypeSystem.Int32, type), uoe.Expression, null, true
                );

                uoe.ReplaceChild(uoe.Expression, cast);
            }

            VisitChildren(uoe);
        }

        public void VisitNode (JSBinaryOperatorExpression boe) {
            var leftType = boe.Left.GetExpectedType(TypeSystem);
            var leftIsEnum = ILBlockTranslator.IsEnum(leftType);
            var rightType = boe.Right.GetExpectedType(TypeSystem);
            var rightIsEnum = ILBlockTranslator.IsEnum(rightType);

            // Detect attempts to perform boolean logic on enums.
            if ((leftIsEnum || rightIsEnum) && LogicalOperators.Contains(boe.Operator)) {
                if (leftIsEnum) {
                    var cast = JSInvocationExpression.InvokeMethod(
                        new JSFakeMethod("valueOf", TypeSystem.Int32, leftType), boe.Left, null, true
                    );

                    boe.ReplaceChild(boe.Left, cast);
                }

                if (rightIsEnum) {
                    var cast = JSInvocationExpression.InvokeMethod(
                        new JSFakeMethod("valueOf", TypeSystem.Int32, rightType), boe.Right, null, true
                    );

                    boe.ReplaceChild(boe.Right, cast);
                }
            }

            VisitChildren(boe);
        }

        public void VisitNode (JSSwitchStatement ss) {
            var conditionType = ss.Condition.GetExpectedType(TypeSystem);

            if (
                !ILBlockTranslator.IsIntegral(conditionType) &&
                ILBlockTranslator.IsEnum(conditionType)
            ) {
                var cast = JSInvocationExpression.InvokeMethod(
                    new JSFakeMethod("valueOf", TypeSystem.Int32, conditionType), ss.Condition, null, true
                );

                ss.ReplaceChild(ss.Condition, cast);
            }

            VisitChildren(ss);
        }

        public void VisitNode (JSCastExpression ce) {
            var currentType = ce.Expression.GetExpectedType(TypeSystem);
            var targetType = ce.NewType;
            JSExpression newExpression = null;

            if (ILBlockTranslator.IsEnum(currentType)) {
                var enumInfo = TypeInfo.GetExisting(currentType);

                if (targetType.MetadataType == MetadataType.Boolean) {
                    EnumMemberInfo enumMember;
                    if (enumInfo.ValueToEnumMember.TryGetValue(0, out enumMember)) {
                        newExpression = new JSBinaryOperatorExpression(
                            JSOperator.NotEqual, ce.Expression, 
                            new JSEnumLiteral(enumMember.Value, enumMember), TypeSystem.Boolean
                        );
                    } else if (enumInfo.ValueToEnumMember.TryGetValue(1, out enumMember)) {
                        newExpression = new JSBinaryOperatorExpression(
                            JSOperator.Equal, ce.Expression,
                            new JSEnumLiteral(enumMember.Value, enumMember), TypeSystem.Boolean
                        );
                    } else {
                        newExpression = new JSUntranslatableExpression(String.Format(
                            "Could not cast enum of type '{0}' to boolean because it has no zero value or one value",
                            currentType.FullName
                        ));
                    }
                }
            }

            if (newExpression != null) {
                ParentNode.ReplaceChild(ce, newExpression);
                VisitReplacement(newExpression);
            } else {
                VisitChildren(ce);
            }
        }
    }
}
