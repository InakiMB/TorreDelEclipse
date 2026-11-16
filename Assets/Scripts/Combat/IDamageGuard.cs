// Permite evitar un impacto sin acoplar Health al movimiento del jugador.
public interface IDamageGuard
{
    bool PreventsDamage { get; }
}
