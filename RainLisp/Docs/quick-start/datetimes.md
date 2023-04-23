# Datetimes
Datetimes is another primitive data type of RainLisp. Unlike the other
primitive types we have seen so far, a datetime does not have a literal
representation. You can make one and handle it with primitive procedures.

Let's create a datetime that represents the last day of 2023.

```scheme
(make-date 2023 12 31)
```
-> *2023-12-31 00:00:00.000*

We specified the year, followed by the month, followed by the day. Note that we did not set a time.

Now, let's specify a datetime with a time too.

```scheme
(make-datetime 2023 12 31 21 30 45 999)
```
-> *2023-12-31 21:30:45.999*

This time, after the day, we specified the hour, minute, second and millisecond in that order.

There are also accessor procedures that, given a datetime, let you access its constituent parts.

Let's try them with the current datetime returned by calling the primitive procedure `now`.

```scheme
(year (now))
(month (now))
(day (now))
(hour (now))
(minute (now))
(second (now))
(millisecond (now))
```
->
```
2023
4
23
21
3
19
583
```

There are many operations that help you work with datetimes:

- [<](../primitives/less.md)
- [<=](../primitives/less-or-equal.md)
- [=](../primitives/equal.md)
- [>](../primitives/greater.md)
- [>=](../primitives/greater-or-equal.md)
- [add-days](../primitives/add-days.md)
- [add-hours](../primitives/add-hours.md)
- [add-milliseconds](../primitives/add-milliseconds.md)
- [add-minutes](../primitives/add-minutes.md)
- [add-months](../primitives/add-months.md)
- [add-seconds](../primitives/add-seconds.md)
- [add-years](../primitives/add-years.md)
- [datetime-to-string](../primitives/datetime-to-string.md)
- [day](../primitives/day.md)
- [days-diff](../primitives/days-diff.md)
- [hour](../primitives/hour.md)
- [hours-diff](../primitives/hours-diff.md)
- [make-date](../primitives/make-date.md)
- [make-datetime](../primitives/make-datetime.md)
- [millisecond](../primitives/millisecond.md)
- [milliseconds-diff](../primitives/milliseconds-diff.md)
- [minute](../primitives/minute.md)
- [minutes-diff](../primitives/minutes-diff.md)
- [month](../primitives/month.md)
- [now](../primitives/now.md)
- [parse-datetime](../primitives/parse-datetime.md)
- [second](../primitives/second.md)
- [seconds-diff](../primitives/seconds-diff.md)
- [to-local](../primitives/to-local.md)
- [to-utc](../primitives/to-utc.md)
- [utc-now](../primitives/utc-now.md)
- [utc?](../primitives/is-utc.md)
- [year](../primitives/year.md)

Next, let's learn about [variables](variables.md).