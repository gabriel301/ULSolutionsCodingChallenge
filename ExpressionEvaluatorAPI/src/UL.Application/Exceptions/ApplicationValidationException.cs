using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UL.Application.Exceptions;
public class ApplicationValidationException: Exception
{
    public IEnumerable<ValidationError> Errors { get; }

    public ApplicationValidationException(IEnumerable<ValidationError> errors)
    {
        Errors = errors;
    }
}
