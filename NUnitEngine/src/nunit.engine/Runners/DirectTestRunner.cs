﻿// ***********************************************************************
// Copyright (c) 2011 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.Collections.Generic;
using NUnit.Engine.Internal;

namespace NUnit.Engine.Runners
{
    /// <summary>
    /// DirectTestRunner is the abstract base for runners 
    /// that deal directly with a framework driver.
    /// </summary>
    public abstract class DirectTestRunner : ITestRunner
    {
        private List<IFrameworkDriver> drivers = new List<IFrameworkDriver>();
        private ServiceContext services;

        protected AppDomain TestDomain;
        protected TestPackage TestPackage;

        protected ServiceContext Services
        {
            get { return services; }
        }

        public DirectTestRunner(ServiceContext services)
        {
            this.services = services;
        }

        #region ITestRunner Members

        public virtual bool Load(TestPackage package)
        {
            this.TestPackage = package;

            IList<string> files = package.GetAssemblies();
            int count = 0;

            foreach (string testFile in files)
            {
                // TODO: Should get the appropriate driver for the file
                IFrameworkDriver driver = new NUnitFrameworkDriver(TestDomain);
                IDictionary<string, object> options = new Dictionary<string, object>();
                if (driver.Load(testFile, options))
                {
                    drivers.Add(driver);
                    count++;
                }
            }

            return count == files.Count;
        }

        public virtual void Unload()
        {
        }

        public TestResult Run(ITestEventHandler listener, ITestFilter filter)
        {
            List<TestResult> results = new List<TestResult>();

            foreach (NUnitFrameworkDriver driver in drivers)
                results.Add(driver.Run(new Dictionary<string, object>(), listener));

            switch (results.Count)
            {
                case 0:
                    return null;
                case 1:
                    return (TestResult)results[0];
                default:
                    return XmlHelper.CombineResults(results);
            }
        }

        #endregion

        #region IDisposable Members

        public virtual void Dispose()
        {
        }

        #endregion
    }
}