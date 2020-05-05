using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class DirectionCommandPickerTest
    {

        DirectionCommandPicker<IDirectionPickable> picker;

        [SetUp]
        public void SetUp()
        {
            picker = new DirectionCommandPicker<IDirectionPickable>(1f);
        }

        public void SetDirectionalService(float horizontal, float vertical)
        {
            IUnityAxisService service = Substitute.For<IUnityAxisService>();
            service.GetAxisRaw("Horizontal").Returns(horizontal);
            service.GetAxisRaw("Vertical").Returns(vertical);
            DirectionalInput.service = service;
        }

        public void SetTimePassed(float secondsPassed)
        {
            IUnityTimeService service = new UnityTimeService();
            service.unscaledTime.Returns(secondsPassed);
            picker.service = service;
        }

        public void SetAndReadInput(float horizontal, float vertical)
        {
            SetDirectionalService(horizontal, vertical);
            picker.InputSelect();
        }

        // check input clears after no input 
        [Test]
        public void InputClearsAfterNoInput()
        {
            //prep for key down
            SetDirectionalService(0, -1);
            //read input
            picker.InputSelect();
            //make sure input got read
            Assert.True(picker.ExistingInput());
            IUnityTimeService timeService = Substitute.For<IUnityTimeService>();
            // One second later 
            timeService.unscaledTime.Returns(1);
            picker.InputSelect();
            // Input should be empty
            Assert.False(picker.ExistingInput());
        }

        // check clear time resets after new input
        [Test]
        public void ClearTimeResetsAfterInput()
        {

            SetAndReadInput(0, -1);
            // set time passed
            SetTimePassed(0.5f);
            // read input again
            SetAndReadInput(1, 0);
            // increase time passed to > 1 second, but less than 1.5s, input should not be cleared
            // Set direction serviceto nothing so no more input is read
            SetDirectionalService(0, 0);
            SetTimePassed(1.01f);
            picker.InputSelect();

            Assert.True(picker.ExistingInput());
        }

        // check no direction succeeds 


        // check button plus direction (6Z)
        // check rolling forward (236Z)
        // check rolling backwards (214Z)
        // Check back to back input (22Z)
        // check input clears after selected

        // existing input empty on start
        [Test]
        public void ExistingInputEmptyOnStartTest()
        {
            Assert.False(picker.ExistingInput());
        }
    }
}