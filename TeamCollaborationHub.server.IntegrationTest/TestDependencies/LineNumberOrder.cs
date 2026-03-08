using Xunit.Abstractions;
using Xunit.Sdk;

namespace TeamCollaborationHub.server.IntegrationTest.TestDependencies;

public class LineNumberOrder:ITestCaseOrderer
{
    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases) where TTestCase : ITestCase
    {
        return testCases.OrderBy(testCase => testCase.SourceInformation.LineNumber);
    }
}