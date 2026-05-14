using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private bool onlyRotateY = true; // Se verdadeiro, a UI não inclina para cima/baixo
    [SerializeField] private bool invertForward = false; // Algumas UIs do Unity ficam de costas por padrão

    public Transform _mainCameraTransform;

    void LateUpdate()
    {
        if (_mainCameraTransform == null) return;

        // Calcula a direção para onde olhar
        Vector3 targetDirection = _mainCameraTransform.position - transform.position;

        if (onlyRotateY)
        {
            targetDirection.y = 0; // Trava a rotação no eixo vertical
        }

        if (targetDirection != Vector3.zero)
        {
            // Cria a rotação baseada na direção
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            // Inverte 180 graus se a UI estiver aparecendo "de costas"
            if (invertForward)
            {
                targetRotation *= Quaternion.Euler(0, 180, 0);
            }

            transform.rotation = targetRotation;
        }
    }
}