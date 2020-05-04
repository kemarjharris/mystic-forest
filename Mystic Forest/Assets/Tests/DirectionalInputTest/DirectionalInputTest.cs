using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;

namespace DirectionalInputTest
{
    public class DirectionalInputTest
    {
        public void SetDirectionalService(float horizontal, float vertical)
        {
            IUnityInputService service = Substitute.For<IUnityInputService>();
            service.GetAxisRaw("Horizontal").Returns(horizontal);
            service.GetAxisRaw("Vertical").Returns(vertical);
            DirectionalInput.service = service;
        }

        [Test]
        public void GetSimpleDirectionNTest()
        {
            SetDirectionalService(0, 1);
            Assert.AreEqual(Direction.N, DirectionalInput.GetSimpleDirection());
        }

        [Test]
        public void GetSimpleDirectionETest()
        {
            SetDirectionalService(1, 0);
            Assert.AreEqual(Direction.E, DirectionalInput.GetSimpleDirection());
        }

        [Test]
        public void GetSimpleDirectionSTest()
        {
            SetDirectionalService(0, -1);
            Assert.AreEqual(Direction.S, DirectionalInput.GetSimpleDirection());
        }

        [Test]
        public void GetSimpleDirectionWTest()
        {
            SetDirectionalService(-1, 0);
            Assert.AreEqual(Direction.W, DirectionalInput.GetSimpleDirection());
        }

        [Test]
        public void GetSimpleDirectionNULLTest()
        {
            SetDirectionalService(0, 0);
            Assert.AreEqual(Direction.NULL, DirectionalInput.GetSimpleDirection());
        }

    }
}
