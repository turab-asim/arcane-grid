namespace InputHandling
{
    public interface IPlayerInputHandler
    {
        bool Menu();

        bool Fire();

        float AimHorizontal();

        float AimVertical();

        float MoveHorizontal();

        float MoveVertical();

        bool Jump();
    }

}
