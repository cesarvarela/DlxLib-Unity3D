﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DlxLib;
using NUnit.Framework;

namespace DlxLibTests
{
    [TestFixture]
    internal class DlxLibEventTests
    {
        [TestCase(0, false)]
        [TestCase(1, true)]
        [TestCase(2, true)]
        [TestCase(3, true)]
        public void StartEventFiresOnlyOnceAndOnlyIfWeActuallyStartToEnumerateTheSolutions(int numSolutionsToTake, bool expectStartedEventToBeRaisedOnce)
        {
            var matrix = new[,]
                {
                    {1, 0, 0, 0},
                    {0, 1, 1, 0},
                    {1, 0, 0, 1},
                    {0, 0, 1, 1},
                    {0, 1, 0, 0},
                    {0, 0, 1, 0}
                };
            var dlx = new Dlx();
            var numStartedEventsRaised = 0;
            dlx.Started += (_, __) => numStartedEventsRaised++;

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            dlx.Solve(matrix).Take(numSolutionsToTake).ToList();
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed

            Assert.That(numStartedEventsRaised, expectStartedEventToBeRaisedOnce ? Is.EqualTo(1) : Is.EqualTo(0));
        }

        [TestCase(0, false)]
        [TestCase(1, true)]
        [TestCase(2, true)]
        [TestCase(3, true)]
        public void FinishedEventFiresOnlyOnceAndOnlyIfWeActuallyStartToEnumerateTheSolutions(int numSolutionsToTake, bool expectFinishedEventToBeRaisedOnce)
        {
            var matrix = new[,]
                {
                    {1, 0, 0, 0},
                    {0, 1, 1, 0},
                    {1, 0, 0, 1},
                    {0, 0, 1, 1},
                    {0, 1, 0, 0},
                    {0, 0, 1, 0}
                };
            var dlx = new Dlx();
            var numFinishedEventsRaised = 0;
            dlx.Finished += (_, __) => numFinishedEventsRaised++;

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            dlx.Solve(matrix).Take(numSolutionsToTake).ToList();
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed

            Assert.That(numFinishedEventsRaised, expectFinishedEventToBeRaisedOnce ? Is.EqualTo(1) : Is.EqualTo(0));
        }

        [Test]
        public void CancelledEventFiresUsingObsoleteCancelMethod()
        {
            var matrix = new bool[0, 0];
            var dlx = new Dlx();
            var cancelledEventHasBeenRaised = false;
            
            dlx.Cancelled += (_, __) => cancelledEventHasBeenRaised = true;
            dlx.Started += (sender, __) => dlx.Cancel();

            dlx.Solve(matrix).ToList();            

            Assert.That(cancelledEventHasBeenRaised, Is.True, "Expected the Cancelled event to have been raised");
        }

        [Test]
        public void SearchStepEventFires()
        {
            var matrix = new[,]
                {
                    {0, 0, 1, 0, 1, 1, 0},
                    {1, 0, 0, 1, 0, 0, 1},
                    {0, 1, 1, 0, 0, 1, 0},
                    {1, 0, 0, 1, 0, 0, 0},
                    {0, 1, 0, 0, 0, 0, 1},
                    {0, 0, 0, 1, 1, 0, 1}
                };
            var dlx = new Dlx();
            var searchStepEventHasBeenRaised = false;
            dlx.SearchStep += (_, __) => searchStepEventHasBeenRaised = true;

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            dlx.Solve(matrix).First();
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed

            Assert.That(searchStepEventHasBeenRaised, Is.True, "Expected the SearchStep event to have been raised");
        }

        [Test]
        public void SearchStepEventsHaveIncreasingIteration()
        {
            var matrix = new[,]
                {
                    {0, 0, 1, 0, 1, 1, 0},
                    {1, 0, 0, 1, 0, 0, 1},
                    {0, 1, 1, 0, 0, 1, 0},
                    {1, 0, 0, 1, 0, 0, 0},
                    {0, 1, 0, 0, 0, 0, 1},
                    {0, 0, 0, 1, 1, 0, 1}
                };
            var dlx = new Dlx();
            var searchStepEventArgs = new List<SearchStepEventArgs>();
            dlx.SearchStep += (_, e) => searchStepEventArgs.Add(e);

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            dlx.Solve(matrix).First();
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed

            Assert.That(searchStepEventArgs.Count, Is.GreaterThanOrEqualTo(5));
            foreach (var index in Enumerable.Range(0, 5))
            {
                Assert.That(searchStepEventArgs[index].Iteration, Is.EqualTo(index));
            }
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void SolutionFoundEventFiresOnceForEachSolutionTaken(int numSolutionsToTake)
        {
            var matrix = new[,]
                {
                    {1, 0, 0, 0},
                    {0, 1, 1, 0},
                    {1, 0, 0, 1},
                    {0, 0, 1, 1},
                    {0, 1, 0, 0},
                    {0, 0, 1, 0}
                };
            var dlx = new Dlx();
            var solutionFoundEventArgs = new List<SolutionFoundEventArgs>();
            dlx.SolutionFound += (_, e) => solutionFoundEventArgs.Add(e);

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            dlx.Solve(matrix).Take(numSolutionsToTake).ToList();
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed

            Assert.That(solutionFoundEventArgs.Count, Is.EqualTo(numSolutionsToTake));
            foreach (var index in Enumerable.Range(0, numSolutionsToTake))
            {
                Assert.That(solutionFoundEventArgs[index].SolutionIndex, Is.EqualTo(index));
            }
        }
    }
}
