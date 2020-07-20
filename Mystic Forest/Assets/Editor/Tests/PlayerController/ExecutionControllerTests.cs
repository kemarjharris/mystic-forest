using Zenject;
using System.Collections;
using UnityEngine.TestTools;
using UnityEngine;
using NSubstitute;
using NUnit.Framework;
using UnityEngine.SceneManagement;

public class ExecutionControllerTests : SceneTestFixture
{
    IUnityTimeService timeService;
    IUnityInputService inputService;
    IExecutionModule module;

    protected GameObject go;
    ExecutionController controller;
    IBattler battler;
    IExecutionState state;

    private class CloseRangeSO : RangeSO
    {
        public override bool BattlerInRange(Transform battlerTranform) => true;
    }

    private class LongRangeSO : RangeSO
    {
        public override bool BattlerInRange(Transform battlerTranform) => false;
    }

    [OneTimeTearDown]
    public void OneTimeTearDown() => TestTools.DestroyAllGameObjects();

    public virtual IEnumerator InitScene()
    {
        timeService = Substitute.For<IUnityTimeService>();
        inputService = Substitute.For<IUnityInputService>();
        module = Substitute.For<IExecutionModule>();
        StaticContext.Container.Bind<IUnityInputService>().FromInstance(inputService).AsSingle();
        StaticContext.Container.Bind<IUnityTimeService>().FromInstance(timeService).AsSingle();
        StaticContext.Container.Bind<IExecutionModule>().FromInstance(module).AsSingle();

        yield return LoadScene("Controller Test Scene");
        yield return null;
        controller = Object.FindObjectOfType<ExecutionController>();
        go = controller.gameObject;
        yield return WaitUntilGrounded();

        controller.closeRange = ScriptableObject.CreateInstance<CloseRangeSO>();
        controller.assault = ScriptableObject.CreateInstance<ExecutableChainSO>();
        battler = Substitute.For<IBattler>();
        battler.IsGrounded.Returns(true);

        state = Substitute.For<IExecutionState>();
        battler.executionState.Returns(state);

        controller.battler = battler;


        yield return new WaitForSecondsRealtime(0.5f);
        // frame of execution logic
        yield return null;
    }

    void SimulateKeyDown()
    {
        inputService.GetKeyDown("j").Returns(true);
        inputService.GetKey("j").Returns(true);
    }

    void SimulateNoInput()
    {
        inputService.GetKeyDown("j").Returns(false);
        inputService.GetKey("j").Returns(false);
        inputService.GetKeyDown("k").Returns(false);
        inputService.GetKey("k").Returns(false);
    }

    void SimulateSkillKeyDown()
    {
        inputService.GetKeyDown("k").Returns(true);
        inputService.GetKey("k").Returns(true);
    }

    public IEnumerator WaitUntilGrounded()
    {
        IBattlerPhysics p = go.GetComponent<IBattlerPhysics>();
        Vector3 oldPos;
        do
        {
            oldPos = go.transform.position;
            yield return new WaitForFixedUpdate();

        } while (!p.IsGrounded && oldPos != go.transform.position);
    }

    // attack button, skill button
    // attacking, not attacking, able to cancel attack
    // normal, comboing, selecting skills

    
    

    // normal state, close range, attack key down, fires basic chain
    [UnityTest]
    public IEnumerator Attack_AttackKeyDown_StartsExecution()
    {

        yield return InitScene();
        SimulateKeyDown();
        yield return null;
        module.Received().StartExecution(battler.ChainSet, battler, Arg.Any<ITargetSet>());
    }

    // normal state, long range, attack key down, fires assault chain
    [UnityTest]
    public IEnumerator Attack_AttackKeyDownOutOfRange_CallsAssault()
    {
        yield return InitScene();
        controller.closeRange = ScriptableObject.CreateInstance<LongRangeSO>();
        SimulateKeyDown();
        yield return null;
        module.Received().StartExecution(controller.assault, battler, Arg.Any<ITargetSet>());
    }

    // normal state, jumping, fires basic chain
    [UnityTest]
    public IEnumerator Attack_AttackKeyDownAirBorne_StartsExecution()
    {
        yield return InitScene();
        battler.IsGrounded.Returns(false);
        SimulateKeyDown();
        yield return null;
        module.Received().StartExecution(battler.ChainSet, battler, Arg.Any<ITargetSet>());
    }

    // comboing but not executing, attack key down, fires basic chain
    [UnityTest]
    public IEnumerator Attack_AttackKeyDownComboingButNotExecuting_StartsExecution()
    {
        yield return InitScene();
        state.comboing.Returns(true);
        state.combatState.Returns(CombatState.NOT_ATTACKING);

        SimulateKeyDown();
        yield return null;
        module.Received().StartExecution(battler.ChainSet, battler, Arg.Any<ITargetSet>());
    }

    // comboing but not executing, attack key down outside of basic range fires basic chain
    [UnityTest]
    public IEnumerator Attack_AttackKeyDownComboingNotExecutingOutsideOfRange_AttackKeyDown_StartsExecution()
    {
        yield return InitScene();
        state.comboing.Returns(true);
        state.combatState.Returns(CombatState.NOT_ATTACKING);
        controller.closeRange = ScriptableObject.CreateInstance<LongRangeSO>();
        SimulateKeyDown();
        yield return null;
        module.Received().StartExecution(battler.ChainSet, battler, Arg.Any<ITargetSet>());
    }

    // normal state, skill key down, starts selecting skill
    [UnityTest]
    public IEnumerator UseSkill_SkillKeyDownNormalState_StartsModuleExecution() 
    {
        yield return InitScene();
        SimulateSkillKeyDown();
        yield return null;
        module.Received().StartExecution(battler.ChainSet, battler, Arg.Any<ITargetSet>());
    }

    // normal state, skill key down changes battler state
    [UnityTest]
    public IEnumerator UseSkill_SkillKeyDownNormalState_ChangesBattlerState()
    {
        yield return InitScene();
        SimulateSkillKeyDown();
        yield return null;
        state.Received().selectingSkill = true;
    }

    // normal state, skill key up after a second Stops Execution
    [UnityTest]
    public IEnumerator UseSkill_SkillKeyDownWaitASecond_StopsModuleExecution()
    {
        yield return InitScene();
        SimulateSkillKeyDown();
        yield return null;
        SimulateNoInput();
        yield return new WaitForSecondsRealtime(1.1f);
        module.Received().StopExecution();
    }

    // skill key timeout stops selecting skill
    [UnityTest]
    public IEnumerator UseSkill_SkillKeyDownWaitASecond_ChangesBattlerState()
    {
        yield return InitScene();
        SimulateSkillKeyDown();
        yield return null;
        SimulateNoInput();
        yield return new WaitForSecondsRealtime(1.1f);
        state.Received().selectingSkill = false;
    }

    // comboing unable to select skill
    [UnityTest]
    public IEnumerator UseSkill_SkillKeyDownComboing_DoesNotStartSelectingSkill()
    {
        yield return InitScene();
        state.comboing.Returns(true);
        SimulateSkillKeyDown();
        yield return null;
        state.DidNotReceive().selectingSkill = true;
        module.DidNotReceive().StartExecution(battler.ChainSet, battler, Arg.Any<ITargetSet>());
    }

    // Executing unable to start execution
    [UnityTest]
    public IEnumerator Attack_AttackKeyDownAttacking_DoesNotStartExecution()
    {
        yield return InitScene();
        state.combatState.Returns(CombatState.ATTACKING);
        SimulateKeyDown();
        yield return null;
        module.DidNotReceive().StartExecution(battler.ChainSet, battler, Arg.Any<ITargetSet>());
        module.DidNotReceive().StartExecution(controller.assault, battler, Arg.Any<ITargetSet>());
    }

    [UnityTest]
    public IEnumerator Attack_AttackKeyDownAbleToCancelAttack_DoesNotStartExecution()
    {
        yield return InitScene();
        state.combatState.Returns(CombatState.ABLE_TO_CANCEL_ATTACK);
        SimulateKeyDown();
        yield return null;
        module.DidNotReceive().StartExecution(battler.ChainSet, battler, Arg.Any<ITargetSet>());
        module.DidNotReceive().StartExecution(controller.assault, battler, Arg.Any<ITargetSet>());
    }

    [UnityTest]
    public IEnumerator UseSkill_SkillKeyDownAttacking_DoesNotStartSelectingSkill()
    {
        yield return InitScene();
        state.combatState.Returns(CombatState.ATTACKING);
        SimulateSkillKeyDown();
        yield return null;
        state.DidNotReceive().selectingSkill = true;
        module.DidNotReceive().StartExecution(battler.ChainSet, battler, Arg.Any<ITargetSet>());
    }
}