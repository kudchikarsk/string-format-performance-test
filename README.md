---
title: C# String.Format() and StringBuilder
description: A complete collection of c# pattern programs. This article explains the list of star pattern programs in c# programming language.
image: "/images/star-pattern/cover.jpg"
date: Sat Oct 12 17:56:37 2019
last_modified_at: Sat Oct 12 17:56:39 2019
categories: [C#]
comments: false
---


Recently, I saw some code that looked something like this:

```cs
StringBuilder builder = new StringBuilder();
builder.Append(String.Format("{0} {1}", firstName, lastName));
// Do more with builder...
```

Now, I don't won't get into arguments about how String.Concat() is more performant here. String.Format() allows code to be more easily localized and it is being used for that purpose here. The real problem is that StringBuilder.AppendFormat() should be used instead:

```cs
StringBuilder builder = new StringBuilder();
builder.AppendFormat("{0} {1}", firstName, lastName);
// Do more with builder...
```

The reason that this is important is because, internally, String.Format() actually creates a StringBuilder and calls StringBuilder.AppendFormat()! String.Format() is implemented like this:

```cs
public static string Format(IFormatProvider provider, string format, params object[] args)
{
  if (format == null || args == null)
   throw new ArgumentNullException((format == null ? "format" : "args"));

  StringBuilder builder = new StringBuilder(format.Length + (args.Length * 8));
  builder.AppendFormat(provider, format, args);
  return builder.ToString();
}
```

It turns out that the formatting logic is actually implemented in StringBuilder.AppendFormat(). So, the original code actually caused a second StringBuilder to be created when it wasn't needed.

This is also important to know if you are trying to avoid creating a StringBuilder by concatentating strings with String.Format(). For example:

```cs
string nameString = "<td>" + String.Format("{0} {1}", firstName, lastName) + "</td>"
  + "<td>" + String.Format("{0}, {1}", id, department) + "</td>";
```

That code will actually create two StringBuilders. So, creating one StringBuilder and using AppendFormat() will be more performent:

```cs
StringBuilder nameBuilder = new StringBuilder();
nameBuilder.Append("<td>");
nameBuilder.AppendFormat("{0} {1}", firstName, lastName);
nameBuilder.Append("</td>");
nameBuilder.Append("<td>");
nameBuilder.AppendFormat("{0}, {1}", id, department);
nameBuilder.Append("</td>");
string nameString = nameBuilder.ToString();
```

I decided to run some performance tests to verify my claims. First, I timed code that demonstrates the very reason that StringBuilder exists:

```cs
const int LOOP_SIZE = 10000;
const string firstName = "Dustin";
const string lastName = "Campbell";
const int id = 1;
const string department = "IDE Tools Team";

static void PerformanceTest1()
{
  string nameString = String.Empty;

  for (int i = 0; i < LOOP_SIZE; i++)
    nameString += String.Format("{0} {1}", firstName, lastName);
}
```

The above code creates a new string and then concatenates to it inside of a for-loop. This causes two new strings to be created on each pass--one from String.Format() and one from the concatenation. This is woefully inefficient.

Next, I tested the same code modified to use a StringBuilder with String.Format():

```cs
static void PerformanceTest2()
{
  StringBuilder builder = new StringBuilder((firstName.Length + lastName.Length + 1) * LOOP_SIZE);

  for (int i = 0; i < LOOP_SIZE; i++)
    builder.Append(String.Format("{0} {1}", firstName, lastName));

  string nameString = builder.ToString();
}
```

Finally, I tested code that uses StringBuilder.AppendFormat() instead of String.Format():

```cs
static void PerformanceTest3()
{
  StringBuilder builder = new StringBuilder((firstName.Length + lastName.Length + 1) * LOOP_SIZE);

  for (int i = 0; i < LOOP_SIZE; i++)
    builder.AppendFormat("{0} {1}", firstName, lastName);

  string nameString = builder.ToString();
}
```

These three methods ran with the following timings:

```
PerformanceTest1: 0.21045 seconds
PerformanceTest2: 0.001585 seconds
PerformanceTest3: 0.0010846 seconds
```

Obviously, concatenating a string in a loop without using a StringBuilder is amazing inefficient. And, removing the call to String.Format also yields a performance boost.

Next, I tested the following two methods:

```cs
static void PerformanceTest4()
{
  string htmlString;
  for (int i = 0; i < LOOP_SIZE; i++)
    htmlString = "<td>" + String.Format("{0} {1}", firstName, lastName) + "</td><td>"
      + String.Format("{0}, {1}", id, department) + "</td>";
}
static void PerformanceTest5()
{
  StringBuilder builder = new StringBuilder(256);

  string htmlString;
  for (int i = 0; i < LOOP_SIZE; i++)
  {
    builder.Append("<td>");
    builder.AppendFormat("{0} {1}", firstName, lastName);
    builder.Append("</td><td>");
    builder.AppendFormat("{0}, {1}", id, department);
    builder.Append("</td>");
    htmlString = builder.ToString();
    builder.Length = 0;
  }
}
```

These two methods ran with the following timings:

```
PerformanceTest4: 0.0050095 seconds
PerformanceTest5: 0.0044467 seconds
```

As you can see, it is important to know when to use String.Format and when to use StringBuilder.AppendFormat(). While the performance boosts that can be achieved are fairly small, they are too easy to code.
