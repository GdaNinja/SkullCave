using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controla a detecção e interação com objetos interativos no mundo (via raycast da câmera)
public class InteractController : MonoBehaviour
{
    private Transform cam;

    // Armazena o último objeto interativo detectado (para esconder o texto se necessário)
    private IInteractable lastInteractable;

    public float distanceDetect;

    public bool detectInteract;

    // Layer onde os objetos interativos estão (filtro do raycast)
    public LayerMask interactLayer;

    void Awake()
    {
        cam = Camera.main.transform;
    }

    void Update()
    {
        Interact();

        // Desenha uma linha azul na cena representando o raycast de interação (só visível no editor)
        Debug.DrawLine(cam.transform.position, cam.transform.position + cam.transform.forward * distanceDetect, Color.blue);
    }

    void Interact()
    {
        RaycastHit hitInfo;

        // Lança um raycast da câmera para frente até a distância definida, apenas nas camadas especificadas
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hitInfo, distanceDetect, interactLayer))
        {
            // Tenta obter o componente IInteractable no objeto atingido
            IInteractable obj = hitInfo.transform.GetComponent<IInteractable>();

            // Se for um objeto diferente do último detectado
            if (lastInteractable != obj)
            {
                // Se havia um anterior, esconde o texto de interação dele
                if (lastInteractable != null)
                {
                    lastInteractable.ShowTxt(false);
                }

                // Atualiza o último objeto detectado
                lastInteractable = obj;
            }

            detectInteract = true;

            // Mostra o texto de interação no objeto atual
            obj.ShowTxt(true);

            // Se o jogador pressionar a tecla E, executa a ação do objeto
            if (Input.GetKeyDown(KeyCode.E))
            {
                obj.Interact();
            }
        }
        else
        {
            // Nenhum objeto foi detectado pelo raycast
            detectInteract = false;

            // Se havia um objeto anteriormente detectado, esconde o texto dele e reseta
            if (lastInteractable != null)
            {
                lastInteractable.ShowTxt(false);
                lastInteractable = null;
            }
        }
    }
}
