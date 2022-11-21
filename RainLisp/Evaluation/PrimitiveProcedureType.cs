﻿namespace RainLisp.Evaluation
{
    public enum PrimitiveProcedureType
    {
        Add,
        Subtract,
        Multiply,
        Divide,
        Modulo,
        GreaterThan,
        GreaterThanOrEqualTo,
        LessThan,
        LessThanOrEqualTo,
        EqualTo,
        LogicalXor,
        LogicalNot,
        Cons,
        Car,
        Cdr,
        List,
        IsNull,
        SetCar,
        SetCdr,
        StringLength,
        Display,
        Debug,
        Trace,
        NewLine,
        Error,
        Now,
        UtcNow,
        MakeDate,
        MakeDateTime,
        Year,
        Month,
        Day,
        Hour,
        Minute,
        Second,
        Millisecond,
        IsUtc,
        ToLocal,
        ToUtc,
        AddYears,
        AddMonths,
        AddDays,
        AddHours,
        AddMinutes,
        AddSeconds,
        AddMilliseconds,
        DaysDiff,
        HoursDiff,
        MinutesDiff,
        SecondsDiff,
        MillisecondsDiff,
        ParseDateTime,
        DateTimeToString
    }
}