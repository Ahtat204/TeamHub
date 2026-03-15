

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;
namespace TeamCollaborationHub.server.IntegrationTest.TestDependencies;
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class TestPriorityAttribute(int priority) : Attribute
{
    public int Priority { get; } = priority;
}
public class PriorityOrderer : ITestCaseOrderer
{
    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases) where TTestCase : ITestCase
    {
      return testCases.OrderBy(tc =>
      {
          var attr = tc.TestMethod.Method
              .GetCustomAttributes(typeof(TestPriorityAttribute).AssemblyQualifiedName)
              .FirstOrDefault();
          return attr?.GetNamedArgument<int>("Priority") ?? 0;
      });
    }
}