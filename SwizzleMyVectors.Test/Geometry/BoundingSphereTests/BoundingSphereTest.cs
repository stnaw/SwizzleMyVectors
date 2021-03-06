﻿using System;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SwizzleMyVectors.Geometry;

namespace SwizzleMyVectors.Test.Geometry.BoundingSphereTests
{
    [TestClass]
    public class BoundingSphereTest
    {
        [TestMethod]
        public void AssertThat_Inflate_IncreasesRadius()
        {
            BoundingSphere s = new BoundingSphere(Vector3.Zero, 10);

            Assert.AreEqual(15, s.Inflate(10).Radius);
        }

        [TestMethod]
        public void AssertThat_Inflate_ByNegativeValue_DecreasesRadius()
        {
            BoundingSphere s = new BoundingSphere(Vector3.Zero, 10);

            Assert.AreEqual(5, s.Inflate(-10).Radius);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AssertThat_Inflate_ByNegativeValueLargerThanDiameter_Throws()
        {
            var s = new BoundingSphere(Vector3.Zero, 10);

            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            s.Inflate(-50);
        }

        

        [TestMethod]
        public void CreateMerged_Disjoint()
        {
            var a = new BoundingSphere(Vector3.Zero, 5);
            var b = new BoundingSphere(new Vector3(20), 5);
            var m = BoundingSphere.CreateMerged(a, b);

            Assert.AreEqual(ContainmentType.Contains, m.Contains(a));
            Assert.AreEqual(ContainmentType.Contains, m.Contains(b));
        }

        [TestMethod]
        public void CreateMerged_Intersect()
        {
            var a = new BoundingSphere(Vector3.Zero, 5);
            var b = new BoundingSphere(new Vector3(6, 0, 0), 5);
            var m = BoundingSphere.CreateMerged(a, b);

            Assert.AreEqual(ContainmentType.Contains, m.Contains(a));
            Assert.AreEqual(ContainmentType.Contains, m.Contains(b));
        }

        [TestMethod]
        public void CreateMerged_Contains()
        {
            var a = new BoundingSphere(Vector3.Zero, 5);
            var b = new BoundingSphere(new Vector3(1, 0, 0), 2);
            var m = BoundingSphere.CreateMerged(a, b);

            Assert.AreEqual(a, m);
        }

        [TestMethod]
        public void CreateMerged_Contains2()
        {
            var a = new BoundingSphere(new Vector3(1, 0, 0), 2);
            var b = new BoundingSphere(Vector3.Zero, 5);
            var m = BoundingSphere.CreateMerged(a, b);

            Assert.AreEqual(ContainmentType.Contains, b.Contains(a));
            Assert.AreEqual(ContainmentType.Intersects, a.Contains(b));
            Assert.AreEqual(b, m);
        }

        [TestMethod]
        public void CreateMerged_Fuzz()
        {
            var r = new Random(2436544);

            for (var i = 0; i < 1024; i++)
            {
                var a = new BoundingSphere(new Vector3((float)r.NextDouble() * 100, (float)r.NextDouble() * 100, (float)r.NextDouble() * 100), (float)r.NextDouble() * 100);
                var b = new BoundingSphere(new Vector3((float)r.NextDouble() * 100, (float)r.NextDouble() * 100, (float)r.NextDouble() * 100), (float)r.NextDouble() * 100);
                var m = BoundingSphere.CreateMerged(a, b);

                if (a.Contains(b) == ContainmentType.Contains)
                    Assert.AreEqual(a, m, $"a={a} b={b}");
                else if (b.Contains(a) == ContainmentType.Contains)
                    Assert.AreEqual(b, m);
                else
                    Assert.IsTrue(m.Contains(a) != ContainmentType.Disjoint && m.Contains(b) != ContainmentType.Disjoint);
            }
        }
    }
}
