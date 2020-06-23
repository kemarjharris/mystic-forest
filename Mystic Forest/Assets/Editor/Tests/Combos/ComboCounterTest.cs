using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NSubstitute;

namespace Tests
{
    public class ComboCounterTest
    {
        ComboCounter counter;

        [SetUp]
        public void SetUp()
        {
            counter = new GameObject().AddComponent<ComboCounter>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(counter.gameObject);
        }

        // battler hit
        // count increments
        [Test]
        public void OnBattlerHit_BattlerHit_IncrementsCount()
        {
            IBattler battler = Substitute.For<IBattler>();
            int c = 0;
            counter.onCountIncremented = (i) => c = i; 
            counter.OnBattlerHit(battler);
            Assert.AreEqual(1, c);
        }

        [Test]
        public void OnBattlerHit_IncrememntsCount_MultipleTimes()
        {
            IBattler battler = Substitute.For<IBattler>();
            int c = 0;
            counter.onCountIncremented = (i) => c = i;
            counter.OnBattlerHit(battler);
            counter.OnBattlerHit(battler);
            Assert.AreEqual(2, c);
        }

        // battler hit then battler recovers
        // -> on hit combo finished triggers
        [Test]
        public void OnBattlerRecovered_BattlerRecovers_CallsOnComboFinished()
        {
            IBattler battler = Substitute.For<IBattler>();
            bool called = false;
            counter.onComboFinished = () => called = true;
            counter.OnBattlerHit(battler);
            counter.OnBattlerRecovered(battler);
            Assert.True(called);
        }

        //  two battlers hit, one recovers 
        // onCombofinished not called
        [Test]
        public void OnBattlerRecovered_SomeStillFlinching_DoesNotCallComboFinished()
        {
            IBattler battler = Substitute.For<IBattler>();
            IBattler battlerTwo = Substitute.For<IBattler>();
            bool called = false;
            counter.onComboFinished = () => called = true;
            counter.OnBattlerHit(battler);
            counter.OnBattlerHit(battlerTwo);
            counter.OnBattlerRecovered(battler);
            Assert.False(called);
        }

        // two battlers hit, both eventually recover
        // oncombo finished called
        [Test]
        public void OnBattlerRecovered_BothRecovered_CallsComboFinished()
        {
            IBattler battler = Substitute.For<IBattler>();
            IBattler battlerTwo = Substitute.For<IBattler>();
            bool called = false;
            counter.onComboFinished = () => called = true;
            counter.OnBattlerHit(battler);
            counter.OnBattlerHit(battlerTwo);
            counter.OnBattlerRecovered(battler);
            counter.OnBattlerRecovered(battlerTwo);
            Assert.True(called);
        }

        [Test]
        public void OnBattlerRecovered_ComboEnded_ResetsCounter()
        {
            IBattler battler = Substitute.For<IBattler>();
            int c = 0;
            counter.onCountIncremented = (i) => c = i;
            counter.OnBattlerHit(battler);
            counter.OnBattlerRecovered(battler);
            counter.OnBattlerHit(battler);
            Assert.AreEqual(1, c);
        }



    }
}
