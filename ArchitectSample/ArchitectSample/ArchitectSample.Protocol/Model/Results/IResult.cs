﻿namespace ArchitectSample.Protocol.Model.Results
{
    public interface IResult
    {
        bool IsSuccess { get; }

        bool IsFailure { get; }

        int Code { get; }

        string Message { get; }
    }
}