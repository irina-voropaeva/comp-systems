using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Lab2.Tree
{
    public class TreeBuilder
    {
        private CheckerHelper _checkerHelper;
        public TreeBuilder()
        {
            _checkerHelper = new CheckerHelper();
        }

        public List<Token> Parse(string source)
        {
            int? start = null;
            var tokens = new List<Token>();

            for (int i = 0; i < source.Length; i++)
            {
                if (!_checkerHelper.IsValidToken(source[i].ToString()))
                {
                    throw new InvalidOperationException($"Unknown symbol at position {i}, symbol - {source[i]}");
                }

                if (start != null)
                {
                    if (!_checkerHelper.IsValue(source[i].ToString()))
                    {
                        tokens.Add(new Token(source.Substring(start.Value, i + 1).ToCharArray(), start, i - 1));
                        tokens.Add(new Token(new Char[] { source[i] }, i, i));
                        start = null;
                    }
                }
                else
                {
                    if (_checkerHelper.IsValue(source[i].ToString()))
                    {
                        start = i;
                    }
                    else
                    {
                        tokens.Add(new Token(source.Substring(start.Value).ToCharArray(), start, source.Length - 1));
                    }
                }

                if (start != null)
                {
                    tokens.Add(new Token(source.ToCharArray(), start, source.Length - 1));
                }
            }
            
            return tokens;
        }

        public int FindParentheses(List<Token> tokens)
        {
            var open = 0;
            var hasValue = false;

            for (int i = 0; i < tokens.Count; i++)
            {
                if (_checkerHelper.IsOpenParentheses(tokens[i].Source.ToString()))
                {
                    open += 1;
                }
                else if (_checkerHelper.IsCloseParentheses(tokens[i].Source.ToString()))
                {
                    if (open == 0)
                    {
                        throw new InvalidOperationException($"Bad close parenthesis at {tokens[i].Start}");
                    }

                    open -= 1;
                }
                else if (_checkerHelper.IsValue(tokens[i].Source.ToString()))
                {
                    hasValue = true;
                }
                else if (_checkerHelper.IsOperation(tokens[i].Source.ToString()))
                {
                    if (!hasValue)
                    {
                        throw new InvalidOperationException($"Expected value at {tokens[i].Start}");
                    }
                }

                if (open == 0)
                {
                    return i + 1;
                }
            }

            throw new InvalidOperationException("Missed close parenthesis at { tokens[-1].end }");
        }

        public void AssertOperation(Token token)
        {
            if (!_checkerHelper.IsOperation(token.Source.ToString()))
            {
                throw new InvalidOperationException($"Expected operation at {token.Start}");
            }
        }

        public List<Token> SplitGroupsSequence(List<Token> tokens)
        {
            var res = new List<Token>();

            while (tokens.Count > 0)
            {
                var i = FindParentheses(tokens);
                res.AddRange(tokens.GetRange(0, i + 1));

                if (i == tokens.Count)
                {
                    break;
                }

                AssertOperation(tokens[i]);

                if (i == tokens.Count - 1)
                {
                    throw new InvalidOperationException($"Expected value at {tokens[i].End + 1}");
                }
                
                res.Add(tokens[i]);
                tokens = tokens.GetRange(i + 1, tokens.Count - i - 1);
            }

            return tokens;
        }

        public Node CreateNode(Token token, Node left, Node right)
        {
            var node = new Node(token);
            node.AddChild(left);
            node.AddChild(right);

            return node;
        }

        public bool IsOkParentheseses(List<Token> tokens)
        {
            var stack = new List<string>();

            foreach (var token in tokens)
            {
                if (_checkerHelper.IsOpenParentheses(token.Source.ToString()))
                {
                    stack.Add("(");
                }

                if (_checkerHelper.IsCloseParentheses(token.Source.ToString()))
                {
                    if (stack.Count > 0 && stack.Last() == "(")
                    {
                        var result = stack.Select(s => s).ToList();

                        result.RemoveAt(stack.Count - 1);

                        stack = result;
                    }
                }
            }

            return stack.Count == 0;
        }

        public List<Token> Trim(List<Token> tokens)
        {
            var left = tokens[0].Start;
            var right = tokens.Last().End;

            var forCheckOkParentheses = tokens.Select(s => s).Skip(1).ToList();

            while (tokens.Count > 0 && _checkerHelper.IsOpenParentheses(tokens[0].Source.ToString()) 
                                    && _checkerHelper.IsCloseParentheses(tokens.Last().Source.ToString())
                                    && IsOkParentheseses(forCheckOkParentheses))
            {
                tokens = forCheckOkParentheses;
            }

            if (tokens.Count == 0)
            {
                throw new InvalidOperationException($"Founded empty pertheses in {left}, {right}");
            }
            return tokens;
        }

        //def plot_graph(node, raw) :
        //    graph = graphviz.Digraph('Tree')
        //    node.plot(graph)
        //    with open('./graph.dot', 'w') as fl:
        //        fl.write(f'# {raw}\n\n')
        //        fl.write(graph.source)

        //def build(tokens) :
        //    if isinstance(tokens, Node) : return tokens
        //    if len(tokens) == 1: return Node(tokens[0]) if not isinstance(tokens[0], Node) else tokens[0]
        //    tokens = trim(tokens)
        //    seq = split_groups_seq(tokens)

        //    while len(seq) > 1:
        //        # op_ix, seq = get_max_op(seq)
        //        depth, op_ix = get_max_op(seq)
        //        print('upd', seq, op_ix)

        //        if op_ix == 0: raise Exception(f"No value at {seq[op_ix].start - 1} for operation")
        //        if op_ix == len(seq) - 1: raise Exception(f"No value at {seq[op_ix].end + 1} for operation")

        //        node = create_node(seq[op_ix], build(seq[op_ix - 1]), build(seq[op_ix + 1]))
        //        node._depth = depth + 1
        //        seq = seq[:op_ix - 1] + [node] + seq[op_ix + 2:]

        //    return seq[0]

        public List<Token> Build(List<Token> tokens)
        {
            if (tokens.Count == 1)
            {
                return tokens;
            }

            tokens = Trim(tokens);

            var seq = SplitGroupsSequence(tokens);

            while (seq.Count > 1)
            {
                //var                 
            }

            return new List<Token>() { seq[0] };
        }

        public int GetDepth(object x)
        {
            if (x.GetType() == typeof(Node))
            {
                return ((Node)x).Depth;
            }

            return 0;
        }

        //def get_max_op(seq) :
        //    max_op = None
        //    max_weight = 0
        //    for ix, item in enumerate(seq) :
        //        if isinstance(item, Token) :
        //            weight = OP_WEIGHTS[item.source]
        //            if weight > max_weight:
        //                max_weight = weight
        //                max_op = ix
        //    if max_op is None:
        //        raise Exception('No operations found')

        //    left_ix, right_ix, source = max_op, max_op, seq[max_op].source
        //    while left_ix > 1 and seq[left_ix - 2].source == source:
        //        left_ix -= 2
        //    while right_ix<len(seq) - 2 and seq[right_ix + 2].source == source:
        //        right_ix += 2

        //    pairs = [(max(get_depth(seq[i - 1]), get_depth(seq[i + 1])), i) for i in range(left_ix, right_ix + 1, 2)]
        //        print('find', seq)
        //    print('pairs', pairs)
        //    depth, ix = sorted(pairs)[0]
        //    return depth, ix

        //public Tuple<int, int> GetMaxOperations(List<Token> seq)
        //{
        //    var 
        //}
    }
}
