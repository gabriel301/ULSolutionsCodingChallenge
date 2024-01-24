using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UL.Application.Exceptions;
public class ApplicationValidationException: Exception
{
    private IEnumerable<ValidationError> _errors;

    public ApplicationValidationException(IEnumerable<ValidationError> errors)
    {
        _errors = errors;
    }
}
