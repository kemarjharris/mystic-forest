public interface IMagicMeter
{
    float Value { get; set; }

    void OnBattlerHit(IBattler battler);

    void DecreaseMana(float mana);
}