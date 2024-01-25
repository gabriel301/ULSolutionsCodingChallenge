using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UL.Application.Abstractions.Command;

namespace UL.Application.Expression.Command;
public record EvaluateTreeExpressionCommand(string expression) : ICommand<double>;
