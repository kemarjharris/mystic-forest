using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class MixAnimatorTest
    {
        GameObject obj;
        MixAnimator animator;
        IDictionary<string, IPlayableAnim> animMap;

        [SetUp]
        public void SetUp()
        {
            obj = Object.Instantiate(Resources.Load<GameObject>("TestPrefabs/Test MixAnimator Game Object"));
            animMap = new Dictionary<string, IPlayableAnim>();
            IPlayableAnim[] anims = new IPlayableAnim[]
            {
                Resources.Load<PlayableAnimSO>("TestScriptableObjects/Test White To Black"),
                Resources.Load<PlayableAnimSO>("TestScriptableObjects/Test White To Red"),
                Resources.Load<PlayableAnimSO>("TestScriptableObjects/Test Move Right"),
                Resources.Load<PlayableAnimSO>("TestScriptableObjects/Test Move Up"),
                Resources.Load<PlayableAnimSO>("TestScriptableObjects/Test Zero Speed")
            };

            animator = obj.GetComponent<MixAnimator>(); 
            for (int i = 0; i < anims.Length; i ++)
            {
                animator.AddAnimation(anims[i]);
                animMap.Add(anims[i].GetName(), anims[i]);
            }
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(obj);
        }

        // test animation plays
        [UnityTest]
        public IEnumerator PlayTest()
        {
            animator.Play(animMap["Test White To Black"]);
            yield return new WaitForSeconds(1.1f);
            Assert.AreEqual(Color.black, obj.GetComponent<SpriteRenderer>().color);
        }

        // test wrong string doesnt crash 
        [Test]
        public void InvalidStringDoesNotCrashTest()
        {
            Assert.DoesNotThrow(delegate
            {
                animator.Play(Resources.Load<PlayableAnimSO>("TestScriptableObjects/Test Empty Animation"));
            });
        }

        [UnityTest]
        public IEnumerator NewAnimationInterruptsTest()
        {
            animator.Play(animMap["Test White To Black"]);
            yield return new WaitForSeconds(0.5f);
            animator.Play(Resources.Load<PlayableAnimSO>("TestScriptableObjects/Test White To Blue"));
            yield return new WaitForSeconds(1.1f);
            Color color = obj.GetComponent<SpriteRenderer>().color;
            Assert.AreEqual(0, color.r);
            Assert.AreEqual(0, color.g);
            Assert.AreEqual(1, color.b);
            Assert.AreEqual(1, color.a);
        }

        // Test playing new animation interrupts
        [UnityTest]
        public IEnumerator PlayingDifferentAnimationInterruptsTest()
        {
            animator.Play(animMap["Test White To Black"]);
            yield return new WaitForSeconds(0.5f);
            animator.Play(animMap["Test White To Red"]);
            yield return new WaitForSeconds(1.1f);
            Assert.AreEqual(Color.red, obj.GetComponent<SpriteRenderer>().color);
        }
        
        // test playing aniamtion resets animation play time
        [UnityTest]
        public IEnumerator PlayingSameAnimResetsAnimTest()
        {
            animator.Play(animMap["Test White To Black"]);
            yield return new WaitForSeconds(0.5f);
            animator.Play(animMap["Test White To Black"]);
            yield return new WaitForSeconds(0.52f);
            Assert.AreNotEqual(Color.black, obj.GetComponent<SpriteRenderer>().color);
        }

        // test movement moves from point a to point b 
        [UnityTest]
        public IEnumerator MovesFromAToBTest()
        {
            Vector2 start = obj.transform.position;
            animator.Play(animMap["Test Move Right"]);
            yield return new WaitForSeconds(1.1f);
            Vector2 expected = obj.transform.position;
            Assert.AreEqual(start + new Vector2(5, 0), expected);
        }

        // test movement moves gradually
        [UnityTest]
        public IEnumerator MovesGraduallyTest()
        {
            Vector2 start = obj.transform.position;
            animator.Play(animMap["Test Move Right"]);
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
            animator.Play(animMap["Test Move Right"]);
            yield return new WaitForSeconds(0.5f);
            // should stop moving right and start moving up
            Vector2 newPos = obj.transform.position;
            animator.Play(animMap["Test Move Up"]);
            yield return new WaitForSeconds(1.1f);
            Vector2 expected = obj.transform.position;
            Assert.AreEqual(newPos + new Vector2(0, 5), expected);
        }

        [UnityTest]
        public IEnumerator ZeroSpeedDoesntCrashTest()
        {
            animator.Play(animMap["Test Zero Speed"]);
            yield return null;
        } 

        [UnityTest]
        public IEnumerator StopStopsAnimationTest()
        {
            animator.Play(animMap["Test White To Black"]);
            yield return new WaitForSeconds(0.5f);
            animator.Stop();
            yield return new WaitForSeconds(0.6f);
            // animation should have returned to plain white
            Assert.AreEqual(Color.white, obj.GetComponent<SpriteRenderer>().color);
        }

        [UnityTest]
        public IEnumerator PlayPlaysIfStoppedTest()
        {
            animator.Stop();
            animator.Play(animMap["Test White To Black"]);
            yield return new WaitForSeconds(1.1f);
            Assert.AreEqual(Color.black, obj.GetComponent<SpriteRenderer>().color);
        }

    }
}
