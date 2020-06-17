using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class DirectionalInputTest
    {
        public void SetDirectionalService(float horizontal, float vertical)
        {
            IUnityAxisService service = Substitute.For<IUnityAxisService>();
            service.GetAxisRaw("Horizontal").Returns(horizontal);
            service.GetAxisRaw("Vertical").Returns(vertical);
            DirectionalInput.service = service;
        }

        public void SetFrame(int frame)
        {
            IUnityTimeService timeService = Substitute.For<IUnityTimeService>();
            timeService.frameCount.Returns(frame);
            DirectionalInput.timeService = timeService;
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

        public void SetAxisByBool(float axisValue, bool axis)
        {
            if (axis)
            {
                SetDirectionalService(axisValue, 0);
            } else
            {
                SetDirectionalService(0, axisValue);
            }
        }

        public void CheckAxisValue(float value, bool horizontal)
        {
            if (horizontal)
            {
                float result = DirectionalInput.GetAxisDown("Horizontal");
                Assert.AreEqual(value, result);
            } else {
                float result = DirectionalInput.GetAxisDown("Vertical");
                Assert.AreEqual(value, result);
            }
        }

        public void Reset()
        {
            DirectionalInput.Reset();
        }

        // Get Axis Down
        // No horizontal key axis does nothing
        public void GetAxisDown_NoAxisInput_ReturnsZero(bool axis)
        {
            Reset();
            SetDirectionalService(0, 0);
            CheckAxisValue(0, axis);
        }

        // axis changed sets axis down
        public void GetAxisDown_AxisDown_ReturnsValue(float value, bool axis)
        {
            Reset();
            SetAxisByBool(value, axis);
            CheckAxisValue(value, axis);
        }

        // axis not positive next frame gives false
        public void GetAxisDown_AxisUp_ReturnsZero(float value, bool axis)
        {
            Reset();
            SetAxisByBool(value, axis);
            CheckAxisValue(value, axis);
            SetFrame(1);
            SetDirectionalService(0, 0);
            CheckAxisValue(0, axis);
        }

        // axis still positive next frame gives zero
        public void GetAxisDown_AxisStaysDown_ReturnsZero(float value, bool axis)
        {
            Reset();
            SetFrame(1);
            SetAxisByBool(value, axis);
            CheckAxisValue(value, axis);
            SetFrame(2);
            CheckAxisValue(0, axis);
        }
        
        // axis down twice in same frame gives true both times
        public void GetAxisDown_TwiceInSameFrame_ReturnsValueSecondTime(float value, bool axis)
        {
            Reset();
            SetFrame(0);
            SetAxisByBool(value, axis);
            CheckAxisValue(value, axis);
            // check second time
            CheckAxisValue(value, axis);
        }

        public void GetAxisDown_CalledAgain_ReturnsValue(float value, bool axis)
        {
            Reset();
            SetFrame(1);
            SetAxisByBool(value, axis);
            CheckAxisValue(value, axis);
            SetFrame(2);
            SetAxisByBool(0, axis);
            CheckAxisValue(0, axis);
            SetFrame(3);
            SetAxisByBool(value, axis);
            CheckAxisValue(value, axis);
        }


        public void RunTests(float axisValue, bool axis)
        {
            GetAxisDown_CalledAgain_ReturnsValue(axisValue, axis);
            GetAxisDown_NoAxisInput_ReturnsZero(axis);
            GetAxisDown_AxisDown_ReturnsValue(axisValue, axis);
            GetAxisDown_AxisUp_ReturnsZero(axisValue, axis);
            GetAxisDown_AxisStaysDown_ReturnsZero(axisValue, axis);
            GetAxisDown_TwiceInSameFrame_ReturnsValueSecondTime(axisValue, axis);
        }

        [Test]
        public void PositiveHorizontalAxisTests()
        {
            RunTests(1, true);
        } 

        [Test]
        public void NegativeHorziontalAxisTests()
        {
            RunTests(-1, true);
        }

        [Test]
        public void PositiveVerticalAxisTests()
        {
            RunTests(1, false);
        }

        [Test]
        public void NegativeVerticalAxisTests()
        {
            RunTests(-1, false);
        }
    }
}
