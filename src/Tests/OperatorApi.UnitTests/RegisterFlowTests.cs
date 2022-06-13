using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using TGS.Services.Operator;

namespace TGS.Tests.OperatorApi.UnitTests;

public class RegisterFlowTests
{
    [Test]
    public void Add_Step_To_RegisterFlowDefinition()
    {
        var sut = new RegisterFlowDefinition();

        var privateList = typeof(RegisterFlowDefinition)
            .GetField("_stepDefinitions", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(sut) as List<IRegisterFlowStep>;

        Assert.AreEqual(0, privateList.Count);

        sut.AddStep(new StubRegisterFlowStep());

        Assert.AreEqual(1, privateList.Count);
    }

    [Test]
    public async Task After_Call_ConfirmCompleted_Will_Remain_Undone_Steps()
    {
        var step1 = new StubRegisterFlowStep();
        step1.SetConfirmCompletedDelegate(() => true);
        var step2 = new StubRegisterFlowStep();
        var step3 = new StubRegisterFlowStep();

        var sut = new RegisterFlow(new[] {
            step1,
            step2,
            step3
        });

        var actual = await sut.ConfirmCompletedAsync();

        var privateStack = typeof(RegisterFlow)
            .GetField("_flowSteps", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(sut) as Queue<IRegisterFlowStep>;

        Assert.IsFalse(actual);
        Assert.AreEqual(2, privateStack.Count);
    }

    [Test]
    public void Build_RegisterFlow()
    {
        var sut = new RegisterFlowDefinition();

        sut.AddStep(new StubRegisterFlowStep());

        var actual = sut.BuildRegisterFlow();

        var privateStack = typeof(RegisterFlow)
            .GetField("_flowSteps", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(actual) as Queue<IRegisterFlowStep>;

        Assert.AreEqual(1, privateStack.Count);
    }

    [Test]
    public void Create_RegisterFlowDefinition_From_RegisterFlowBuilder()
    {
        var serviceProvider = new ServiceCollection()
            .BuildServiceProvider();
        var sut = new RegisterFlowBuilder(
            serviceProvider,
            new Dictionary<string, Type>
            {
                ["Test1"] = typeof(StubRegisterFlowStep)
            }.ToImmutableDictionary());

        var actual = sut.LoadSettings(JsonDocument.Parse(@"[
            {
                ""type"": ""Test1""
            }]"));

        var privateList = typeof(RegisterFlowDefinition)
            .GetField("_stepDefinitions", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(actual) as List<IRegisterFlowStep>;

        Assert.IsNotNull(privateList);
        Assert.AreEqual(1, privateList.Count);
    }

    [Test]
    public async Task If_Has_Any_Undone_Step_ConfirmCompleted_Should_Return_True()
    {
        var sut = new RegisterFlow(Array.Empty<IRegisterFlowStep>());

        var actual = await sut.ConfirmCompletedAsync();

        Assert.IsTrue(actual);
    }

    [Test]
    public async Task Process_Input_If_Step_Undone_Will_Return_CurrentStepInfo()
    {
        var step1 = new StubRegisterFlowStep()
        {
            Type = "Step1"
        };
        var stepResponse = JsonDocument.Parse("{}");
        step1.SetProcessInputDelegate(_ => Task.FromResult(
            new RegisterFlowProcessResult(false, null, stepResponse)));
        var step2 = new StubRegisterFlowStep()
        {
            Type = "Step2"
        };
        var step3 = new StubRegisterFlowStep()
        {
            Type = "Step3"
        };

        var sut = new RegisterFlow(new[] {
            step1,
            step2,
            step3
        });

        var actual = await sut.ProcessInputAsync(JsonDocument.Parse("{}"));

        Assert.IsFalse(actual.IsCompleted);
        Assert.AreEqual("Step1", actual.NextStepType);
        Assert.AreEqual(stepResponse, actual.StepResponse);
    }

    [Test]
    public async Task Process_Input_Will_Return_NextStepInfo()
    {
        var step1 = new StubRegisterFlowStep()
        {
            Type = "Step1"
        };
        step1.SetProcessInputDelegate(_ => Task.FromResult(
            new RegisterFlowProcessResult(true, null, null)));
        var step2 = new StubRegisterFlowStep()
        {
            Type = "Step2"
        };
        var step3 = new StubRegisterFlowStep()
        {
            Type = "Step3"
        };

        var sut = new RegisterFlow(new[] {
            step1,
            step2,
            step3
        });

        var actual = await sut.ProcessInputAsync(JsonDocument.Parse("{}"));

        Assert.IsFalse(actual.IsCompleted);
        Assert.AreEqual("Step2", actual.NextStepType);
    }

    [Test]
    public async Task Undone_Flow_Call_ConfirmCompleted_Should_Return_False()
    {
        var sut = new RegisterFlow(new[] {
            new StubRegisterFlowStep()
        });

        var actual = await sut.ConfirmCompletedAsync();

        Assert.IsFalse(actual);
    }
}
