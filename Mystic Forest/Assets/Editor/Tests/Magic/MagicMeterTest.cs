using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NSubstitute;

namespace Tests
{
    public class MagicMeterTest
    {
        MagicMeter meter;
        MagicMeterSettings settings;
        // magic meter increments
        // magic meter decreaaes with time

        [SetUp]
        public void SetUp()
        {
            meter = new GameObject().AddComponent<MagicMeter>();
            settings = ScriptableObject.CreateInstance<MagicMeterSettings>();
            settings.decreasePerSecond = 1;
            settings.incrementBy = 10;
            settings.maxMana = 100;
            meter.settings = settings;
        }
        

        [Test]
        public void OnBattlerHit_BattlerHit_MagicMeterIncreasesTest()
        {
            IBattler battler = Substitute.For<IBattler>();
            meter.OnBattlerHit(battler);
            Assert.AreEqual(settings.incrementBy, meter.Value);
        }

        [Test]
        public void OnBattlerHit_MagicMeterIncrements_BoundedByMaxMana()
        {
            IBattler battler = Substitute.For<IBattler>();
            settings.incrementBy = settings.incrementBy = settings.maxMana + 1;
            meter.OnBattlerHit(battler);
            Assert.AreEqual(settings.maxMana, meter.Value);
        }

        [UnityTest]
        public IEnumerator Update_MagicMeterDecreasesByDecreaseByTest()
        {
            IBattler battler = Substitute.For<IBattler>();
            meter.OnBattlerHit(battler);
            yield return new WaitForSeconds(settings.decreasePerSecond);
            // decreased fast enough
            Assert.Less(meter.Value, settings.incrementBy - (settings.decreasePerSecond * 0.9f));
            // decreased slowly
            Assert.Greater(meter.Value, settings.incrementBy - (settings.decreasePerSecond * 1.1f));
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(meter.gameObject);
        }
    }
}
