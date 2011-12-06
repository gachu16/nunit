﻿// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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
using System.Collections;
using NUnit.Framework.Internal;
using NUnit.TestUtilities;

namespace NUnit.Framework.Constraints.Tests
{
    public class CollectionEquivalentConstraintTests
    {
        [Test]
        public void EqualCollectionsAreEquivalent()
        {
            ICollection set1 = new ICollectionAdapter("x", "y", "z");
            ICollection set2 = new ICollectionAdapter("x", "y", "z");

            Assert.That(new CollectionEquivalentConstraint(set1).Matches(set2).HasSucceeded);
        }

        [Test]
        public void WorksWithCollectionsOfArrays()
        {
            byte[] array1 = new byte[] { 0x20, 0x44, 0x56, 0x76, 0x1e, 0xff };
            byte[] array2 = new byte[] { 0x42, 0x52, 0x72, 0xef };
            byte[] array3 = new byte[] { 0x20, 0x44, 0x56, 0x76, 0x1e, 0xff };
            byte[] array4 = new byte[] { 0x42, 0x52, 0x72, 0xef };

            ICollection set1 = new ICollectionAdapter(array1, array2);
            ICollection set2 = new ICollectionAdapter(array3, array4);

            Constraint constraint = new CollectionEquivalentConstraint(set1);
            Assert.That(constraint.Matches(set2).HasSucceeded);

            set2 = new ICollectionAdapter(array4, array3);
            Assert.That(constraint.Matches(set2).HasSucceeded);
        }

        [Test]
        public void EquivalentIgnoresOrder()
        {
            ICollection set1 = new ICollectionAdapter("x", "y", "z");
            ICollection set2 = new ICollectionAdapter("z", "y", "x");

            Assert.That(new CollectionEquivalentConstraint(set1).Matches(set2).HasSucceeded);
        }

        [Test]
        public void EquivalentFailsWithDuplicateElementInActual()
        {
            ICollection set1 = new ICollectionAdapter("x", "y", "z");
            ICollection set2 = new ICollectionAdapter("x", "y", "x");

            Assert.False(new CollectionEquivalentConstraint(set1).Matches(set2).HasSucceeded);
        }

        [Test]
        public void EquivalentFailsWithDuplicateElementInExpected()
        {
            ICollection set1 = new ICollectionAdapter("x", "y", "x");
            ICollection set2 = new ICollectionAdapter("x", "y", "z");

            Assert.False(new CollectionEquivalentConstraint(set1).Matches(set2).HasSucceeded);
        }

        [Test]
        public void EquivalentHandlesNull()
        {
            ICollection set1 = new ICollectionAdapter(null, "x", null, "z");
            ICollection set2 = new ICollectionAdapter("z", null, "x", null);

            Assert.That(new CollectionEquivalentConstraint(set1).Matches(set2).HasSucceeded);
        }

        [Test]
        public void EquivalentHonorsIgnoreCase()
        {
            ICollection set1 = new ICollectionAdapter("x", "y", "z");
            ICollection set2 = new ICollectionAdapter("z", "Y", "X");

            Assert.That(new CollectionEquivalentConstraint(set1).IgnoreCase.Matches(set2).HasSucceeded);
        }

#if CS_3_0 || CS_4_0
        [Test]
        public void EquivalentHonorsUsing()
        {
            ICollection set1 = new ICollectionAdapter("x", "y", "z");
            ICollection set2 = new ICollectionAdapter("z", "Y", "X");

            Assert.That(new CollectionEquivalentConstraint(set1)
                .Using<string>((x, y) => String.Compare(x, y, true))
                .Matches(set2).HasSucceeded);
        }
#endif
    }
}