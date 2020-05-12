using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class MixAnimatorTest
    {
        GameObject obj;
        MixAnimator animator;
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            obj = Object.Instantiate(Resources.Load<GameObject>("TestPrefabs/Test MixAnimator Game Object"));
            IPlayableAnim[] anims = new IPlayableAnim[]
            {
                Resources.Load<PlayableAnimSO>("TestSCriptableObjects/Test White To Black"),
                Resources.Load<PlayableAnimSO>("TestSCriptableObjects/Test White To Red"),
                Resources.Load<PlayableAnimSO>("TestSCriptableObjects/Test Move Right"),
                Resources.Load<PlayableAnimSO>("TestSCriptableObjects/Test Move Up"),
                Resources.Load<PlayableAnimSO>("TestSCriptableObjects/Test Empty Animation"),
                Resources.Load<PlayableAnimSO>("TestSCriptableObjects/Test Zero Speed")
            };
            animator = obj.GetComponent<MixAnimator>(); 
            animator.Initialize(anims);
        }

        // test animation plays
        [UnityTest]
        public IEnumerator PlayTest()
        {
            animator.Play("Test White To Black");
            yield return new WaitForSeconds(1.1f);
            Assert.AreEqual(Color.black, obj.GetComponent<SpriteRenderer>().color);
        }

        // test wrong string doesnt crash 
        [Test]
        public void InvalidStringDoesNotCrashTest()
        {
            Assert.DoesNotThrow(delegate
            {
                animator.Play("Nonexistant string");
            });
        }

        [UnityTest]
        public IEnumerator InvalidStringDoesNotInterruptTest()
        {
            animator.Play("Test White To Black");
            yield return new WaitForSeconds(0.5f);
            animator.Play("Nonexistant string");
            yield return new WaitForSeconds(0.52f);
            Assert.AreEqual(Color.black, obj.GetComponent<SpriteRenderer>().color);
        }

        // Test playing new animation interrupts
        [UnityTest]
        public IEnumerator PlayingDifferentAnimationInterruptsTest()
        {
            animator.Play("Test White To Black");
            yield return new WaitForSeconds(0.5f);
            animator.Play("Test White To Red");
            yield return new WaitForSeconds(1.1f);
            Assert.AreEqual(Color.red, obj.GetComponent<SpriteRenderer>().color);
        }
        
        // test playing aniamtion resets animation play time
        [UnityTest]
        public IEnumerator PlayingSameAnimResetsAnimTest()
        {
            animator.Play("Test White To Black");
            yield return new WaitForSeconds(0.5f);
            animator.Play("Test White To Black");
            yield return new WaitForSeconds(0.52f);
            Assert.AreNotEqual(Color.black, obj.GetComponent<SpriteRenderer>().color);
        }

        // test movement moves from point a to point b 
        [UnityTest]
        public IEnumerator MovesFromAToBTest()
        {
            Vector2 start = obj.transform.position;
            animator.Play("Test Move Right");
            yield return new WaitForSeconds(1.1f);
            Vector2 expected = obj.transform.position;
            Assert.AreEqual(start + new Vector2(5, 0), expected);
        }

        // test movement moves gradually
        [UnityTest]
        public IEnumerator MovesGraduallyTest()
        {
            Vector2 start = obj.transform.position;
            animator.Play("Test Move Right");
            yield return new WaitForSeconds(0.5f);
            // obj should be between 2 and 3
            Assert.Less(start.x + 2, obj.transform.position.x);
            Assert.Greater(start.x + 3, obj.transform.position.x);
            yield return new WaitForSeconds(0.51f);
            Vector2 expected = obj.transform.position;
            Assert.AreEqual(start + new Vector2(5, 0), expected);
        }

        // test movement moves from start position when new animation plays
        [UnityTest]
        public IEnumerator NewMovementStartsTest()
        {
            Vector2 start = obj.transform.position;
            animator.Play("Test Move Right");
            yield return new WaitForSeconds(0.5f);
            // should stop moving right and start moving up
            Vector2 newPos = obj.transform.position;
            animator.Play("Test Move Up");
            yield return new WaitForSeconds(1.1f);
            Vector2 expected = obj.transform.position;
            Assert.AreEqual(newPos + new Vector2(0, 5), expected);
        }

        [UnityTest]
        public IEnumerator ZeroSpeedDoesntCrashTest()
        {
            animator.Play("Test Zero Speed");
            yield return null;
        } 

    }
}
