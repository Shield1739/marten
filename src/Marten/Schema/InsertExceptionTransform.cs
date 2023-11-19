using System;
using JasperFx.Core.Exceptions;
using Marten.Exceptions;

namespace Marten.Schema;

public sealed class InsertExceptionTransform<T>: IExceptionTransform
{
    private const string ExpectedMessage = "23505: duplicate key value violates unique constraint";
    private readonly object id;
    private readonly string tableName;

    public InsertExceptionTransform(object id, string tableName)
    {
        this.id = id;
        this.tableName = tableName;
    }

    public bool TryTransform(Exception original, out Exception transformed)
    {
        transformed = null;

        if (original.Message?.IndexOf(ExpectedMessage, StringComparison.OrdinalIgnoreCase) > -1 &&
            original.Message?.IndexOf(tableName, StringComparison.Ordinal) > -1)
        {
            transformed = new DocumentAlreadyExistsException(original, typeof(T), id, null);
            return true;
        }

        return false;
    }
}
