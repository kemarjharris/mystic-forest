using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;

namespace DirectionCommandPicker
{

    public class DirectionCommandPickerTest
    {
        DirectionCommandPicker<IDirectionPickable> picker;

        [SetUp]
        public void SetUp()
        {
            picker = new DirectionCommandPicker<IDirectionPickable>(1f);
            SetTimePassed(0);
            ResetKeys();
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
            IUnityTimeService service = Substitute.For<IUnityTimeService>();
            service.unscaledTime.Returns(secondsPassed);
            picker.service = service;
        }

        public void SetAndReadInput(float horizontal, float vertical)
        {
            SetDirectionalService(horizontal, vertical);
            picker.InputSelect();
        }

        public void SetKeyPress(string key)
        {
            IUnityInputService service = Substitute.For<IUnityInputService>();
            service.GetKeyDown(key).Returns(true);
            picker.inputService = service;
        }

        public void ResetKeys()
        {
            IUnityInputService service = Substitute.For<IUnityInputService>();
            service.GetKeyDown("").ReturnsForAnyArgs(false);
            picker.inputService = service;
        }

        // check input clears after no input 
        [Test]
        public void InputClearsAfterNoInputTest()
        {
            //prep for key down
            SetDirectionalService(0, -1);
            //read input
            picker.InputSelect();
            //make sure input got read
            Assert.True(picker.ExistingInput());
            IUnityTimeService timeService = Substitute.For<IUnityTimeService>();
            // One second later 
            timeService.unscaledTime.Returns(1.01f);
            SetDirectionalService(0, 0);
            picker.InputSelect();
            // Input should be empty
            Assert.False(picker.ExistingInput());
        }

        [Test]
        public void ReadsInputOnFrameClearedTest()
        {
            SetDirectionalService(0, -1);
            //read input
            picker.InputSelect();
            //make sure input got read
            Assert.True(picker.ExistingInput());
            IUnityTimeService timeService = Substitute.For<IUnityTimeService>();
            // One second later 
            timeService.unscaledTime.Returns(1.01f);
            //input should clear but because of the
            // down key being simulated there should still be input
            picker.InputSelect();
            // Input should be empty
            Assert.True(picker.ExistingInput());
        }

        // check clear time resets after new input
        [Test]
        public void ClearTimeResetsAfterInputTest()
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

        // check button plus direction (6Z)
        [Test]
        public void ButtonPlusDirectionTest()
        {
            IDirectionCommand expected = new DirectionCommand(DirectionCommandButton.Z, Direction.S);
            picker.set(new IDirectionPickable[] { expected });
            SetAndReadInput(0, -1);
            SetKeyPress("z");
            IDirectionPickable result = picker.InputSelect();
            Assert.AreEqual(expected, result);
        }

        // check rolling forward (236Z)
        [Test]
        public void RollForwardTest()
        {
            IDirectionCommand expected = new DirectionCommand(DirectionCommandButton.Z, Direction.S, Direction.E);
            picker.set(new IDirectionPickable[] { expected });
            SetAndReadInput(0, -1);
            SetAndReadInput(1, 0);
            SetKeyPress("z");
            Assert.AreEqual(expected, picker.InputSelect());
        }

        // check rolling backwards (214Z)
        [Test]
        public void RollBackwardsTest()
        {
            IDirectionCommand expected = new DirectionCommand(DirectionCommandButton.Z, Direction.S, Direction.W);
            picker.set(new IDirectionPickable[] { expected });
            SetAndReadInput(0, -1);
            SetAndReadInput(-1, 0);
            SetKeyPress("z");
            Assert.AreEqual(expected, picker.InputSelect());
        }

        // Check back to back input (22Z)
        [Test]
        public void SameDirectionTest()
        {
            IDirectionCommand expected = new DirectionCommand(DirectionCommandButton.Z, Direction.S, Direction.S);
            picker.set(new IDirectionPickable[] { expected });
            SetAndReadInput(0, -1);
            SetAndReadInput(0, 0);
            SetAndReadInput(0, -1);
            SetKeyPress("z");
            Assert.AreEqual(expected, picker.InputSelect());
        }

        // check input clears after selected
        [Test]
        public void InputClearsAfterSelectingTest()
        {
            IDirectionCommand expected = new DirectionCommand(DirectionCommandButton.Z, Direction.S);
            picker.set(new IDirectionPickable[] { expected });
            SetAndReadInput(0, -1);
            SetKeyPress("z");
            Assert.NotNull(picker.InputSelect());
            Assert.False(picker.ExistingInput());
        }

        [Test]
        public void InputClearsAfterKeyPressTest()
        {
            IDirectionCommand expected = new DirectionCommand(DirectionCommandButton.Z, Direction.S);
            picker.set(new IDirectionPickable[] { expected });
            SetAndReadInput(0, -1);
            SetKeyPress("x");
            // read input
            picker.InputSelect();
            Assert.False(picker.ExistingInput());
        } 

        [Test]
        public void NullOnFailiureTest()
        {
            IDirectionCommand expected = new DirectionCommand(DirectionCommandButton.X, Direction.S);
            picker.set(new IDirectionPickable[] { expected });
            SetAndReadInput(0, -1);
            SetKeyPress("z");
            Assert.Null(picker.InputSelect());        }

        // existing input empty on start
        [Test]
        public void ExistingInputEmptyOnStartTest()
        {
            Assert.False(picker.ExistingInput());
        }
    }
}