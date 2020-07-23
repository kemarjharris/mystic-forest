public interface IStaminaController
{
    void DecreaseStamina(float value);
    void StartRestoring();
    void StopRestoring();
    float stamina { get; }
}