using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using Core;

namespace Lab2.Tree
{
    public class ArithmeticTreeBuilder
    {
        public TreeNode<string> Build(string expression)
        {
            //example: (a+b)*(c+d)/((a+b)-c)

            var operandsStack = new Stack<string>();
            var valuesStack = new Stack<string>();
            TreeNode<string> tree = null;

            TreeNode<string> prevNode = null;
            string lostValue = null;

            foreach (var value in expression)
            {
                if (AvailableOperations.AlgebraicOperations.Contains(value.ToString()))
                {
                    if (tree == null)
                    {
                        tree = new TreeNode<string>(value.ToString());
                        prevNode = tree;

                        if (lostValue != null)
                        {
                            prevNode.AddChild(lostValue);
                        }
                    }
                    else
                    {
                        if (prevNode?.Parent != null)
                        {
                            prevNode.Parent.AddChild(value.ToString());
                        }
                        else
                        {
                            prevNode.AddChild(value.ToString());

                        }

                        prevNode = tree.FindTreeNode(t => t.Data == value.ToString());
                    }
                }
                else if(value.ToString() != ")" && value.ToString() != "(")
                {
                    if (prevNode != null)
                    {
                        prevNode.AddChild(value.ToString());
                    }
                }
                if (tree == null && prevNode == null && value.ToString() != ")" && value.ToString() != "(" && !AvailableOperations.AlgebraicOperations.Contains(value.ToString()))
                {
                    lostValue = value.ToString();
                }
            }

            ////full stack
            //foreach (var value in expression)
            //{
            //    if (AvailableOperations.AlgebraicOperations.Contains(value.ToString()))
            //    {
            //        operandsStack.Push(value.ToString());
            //    } 
            //    else
            //    {
            //        valuesStack.Push(value.ToString());
            //    }
            //}

            ////if parenthless exists
            //while (operandsStack.Count > 0)
            //{
            //    if (operandsStack.Count == 0 || valuesStack.Count == 0)
            //    {
            //        break;
            //    }

            //    var symbol = valuesStack.Pop();
            //    var operand = operandsStack.Pop();

            //        if (symbol.Equals(")") && tree == null)
            //        {
            //            tree = new TreeNode<string>(operand);

            //            if (symbol == ")" || symbol == "(")
            //            {
            //                continue;
            //            }

            //            var rightValue = valuesStack.Pop();

            //            if (rightValue == ")")
            //            {
            //                rightValue = valuesStack.Pop();
            //            }

            //            var nextOperand = operandsStack.Pop();
            //            var leftValue = valuesStack.Pop();

            //            tree.AddChild(nextOperand);

            //            var treeNode = tree.FindTreeNode(x => x.Data == nextOperand);
            //            treeNode.AddChild(rightValue);
            //            treeNode.AddChild(leftValue);
            //        }
            //}

            return tree;
        }
    }
}
