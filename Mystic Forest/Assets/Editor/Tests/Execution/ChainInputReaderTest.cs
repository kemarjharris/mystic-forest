using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NSubstitute;

namespace ChainInputReaderTest
{
    public class ChainInputReaderTest
    {
        ChainInputReader reader = new ChainInputReader();

        [SetUp]
        public void SetUp()
        {
            reader = new ChainInputReader();
            IUnityInputService service = Substitute.For<IUnityInputService>();
            foreach (DirectionCommandButton button in EnumUtil.values<DirectionCommandButton>())
            {
                if (button == DirectionCommandButton.NULL) continue;
                string key = button.ToString().ToLower();
                service.GetKeyDown(key).Returns(false);
                service.GetKey(key).Returns(false);
                service.GetKeyUp(key).Returns(false);
            }
            reader.service = service;
        }

        [Test]
        public void ReadsGetKeyAcceptableInputTest()
        {
            reader.service.GetKey("j").Returns(true);
            Assert.AreEqual("j", reader.ReadInput());
        }

        [Test]
        public void ReadsKeyUpAcceptableInputTest()
        {
            reader.service.GetKeyUp("j").Returns(true);
            Assert.AreEqual("j", reader.ReadInput());
        }

        [Test]
        public void IgnoresUnacceptableInputTest()
        {
            reader.service.GetKey("space").Returns(true);
            Assert.AreEqual("", reader.ReadInput());
        }
    }
}
