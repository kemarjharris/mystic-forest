using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;

namespace DirectionCommandPickerTest
{

    public class DirectionCommandPickerTest
    {
        DirectionCommandPicker<IDirectionPickable> picker;

        [SetUp]
        public void SetUp()
        {
            picker = new DirectionCommandPicker<IDirectionPickable>(1f);
            SetTimePassed(0);
            ReSetKeys();
            SetDirectionalService(0, 0);
            picker.Set(Substitute.For<IEnumerable<IDirectionCommand>>());
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

        public void ReSetKeys()
        {
            IUnityInputService service = Substitute.For<IUnityInputService>();
            service.GetKeyDown("").ReturnsForAnyArgs(false);
            picker.inputService = service;
        }

        [Test]
        public void WaitsToClearInputTest()
        {
            SetAndReadInput(0, -1);
            SetAndReadInput(0, 0);
            SetTimePassed(0.5f);
            picker.InputSelect();
            // Picker should still have existing input
            Assert.True(picker.ExistingInput());
        }

        // WaitsToClearInputAfterGameTimePassedGreaterThanTimeBeforeClearingInput
        [Test]
        public void WaitsToClearInputAfterGameTimePassedTest()
        {
            SetTimePassed(20);
            SetAndReadInput(0, -1);
            SetTimePassed(20.5f);
            SetAndReadInput(0, 0);
            picker.InputSelect();
            // Picker should still have existing input
            Assert.True(picker.ExistingInput());
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
            SetTimePassed(1.01f);
            SetAndReadInput(0, 0);
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

        // check clear time reSets after new input
        [Test]
        public void ClearTimeReSetsAfterInputTest()
        {

            SetAndReadInput(0, -1);
            // Set time passed
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
            picker.Set(new IDirectionPickable[] { expected });
            SetAndReadInput(0, -1);
            SetAndReadInput(0,0);
            SetKeyPress("z");
            IDirectionPickable result = picker.InputSelect();
            Assert.IsNull(picker.InputSelect());
        }

        [Test]
        public void SingleKeyAndDriectionInputOnDifferentFramesTest()
        {
            IDirectionCommand expected = new DirectionCommand(DirectionCommandButton.Z, Direction.S);
            picker.Set(new IDirectionPickable[] { expected });
            SetDirectionalService(0, -1);
            SetKeyPress("z");
            IDirectionPickable result = picker.InputSelect();
            Assert.AreEqual(expected, result);
        }

        // check rolling forward (236Z)
        [Test]
        public void RollForwardTest()
        {
            IDirectionCommand expected = new DirectionCommand(DirectionCommandButton.Z, Direction.S, Direction.E);
            picker.Set(new IDirectionPickable[] { expected });
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
            picker.Set(new IDirectionPickable[] { expected });
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
            picker.Set(new IDirectionPickable[] { expected });
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
            picker.Set(new IDirectionPickable[] { expected });
            SetAndReadInput(0, -1);
            SetKeyPress("z");
            Assert.NotNull(picker.InputSelect());
            Assert.False(picker.ExistingInput());
        }

        [Test]
        public void InputClearsAfterKeyPressTest()
        {
            IDirectionCommand expected = new DirectionCommand(DirectionCommandButton.Z, Direction.S);
            picker.Set(new IDirectionPickable[] { expected });
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
            picker.Set(new IDirectionPickable[] { expected });
            SetAndReadInput(0, -1);
            SetKeyPress("z");
            Assert.Null(picker.InputSelect());        }

        // existing input empty on start
        [Test]
        public void ExistingInputEmptyOnStartTest()
        {
            Assert.False(picker.ExistingInput());
        }

        [Test]
        public void OnSelectedEventFiresTest()
        {
            bool fired = false;
            // true if fired
            picker.OnSelected.AddAction((x) => fired = true);
            IDirectionCommand expected = new DirectionCommand(DirectionCommandButton.Z, Direction.S);
            picker.Set(new IDirectionPickable[] { expected });
            SetAndReadInput(0, -1);
            SetKeyPress("z");
            // event should fire
            picker.InputSelect();
            Assert.True(fired);
        }

        [Test]
        public void IteratesThroughInputTest()
        {
            IDirectionCommand expected = new DirectionCommand(DirectionCommandButton.Z, Direction.S, Direction.E);
            picker.Set(new IDirectionPickable[] { expected });
            SetAndReadInput(-1, 0);
            SetAndReadInput(0, -1);
            SetAndReadInput(1, 0);
            SetKeyPress("z");
            Assert.AreEqual(expected, picker.InputSelect());
        }

        [Test]
        public void AcceptsLongerInputFirstTest()
        {
            IDirectionCommand expected = new DirectionCommand(DirectionCommandButton.Z, Direction.W, Direction.S, Direction.E);
            picker.Set(new IDirectionPickable[] { expected,
                new DirectionCommand(DirectionCommandButton.Z, Direction.S, Direction.E)});
            SetAndReadInput(-1, 0);
            SetAndReadInput(0, -1);
            SetAndReadInput(1, 0);
            SetKeyPress("z");
            Assert.AreEqual(expected, picker.InputSelect());
        }

        [Test]
        public void SingleInputTest()
        {
            IDirectionCommand expected = new DirectionCommand(DirectionCommandButton.Z);
            picker.Set(new IDirectionPickable[] { expected });
            SetKeyPress("z");
            IDirectionPickable result = picker.InputSelect();
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void SingleInputIteratesThroughInputTest()
        {
            IDirectionCommand expected = new DirectionCommand(DirectionCommandButton.Z);
            picker.Set(new IDirectionPickable[] { expected });
            SetAndReadInput(0, -1);
            SetKeyPress("z");
            IDirectionPickable result = picker.InputSelect();
            Assert.AreEqual(expected, result);
        }
    }
}