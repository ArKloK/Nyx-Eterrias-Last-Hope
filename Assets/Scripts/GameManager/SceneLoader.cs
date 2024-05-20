using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Guarda el juego antes de cambiar de escena
            DataPersistenceManager.Instance.SaveGame();

            // Configura la bandera UseSavedPosition según tus necesidades
            DataPersistenceManager.Instance.UseSavedPosition = false;

            // Suscribe la función al evento OnSceneLoadedEvent
            DataPersistenceManager.OnSceneLoadedEvent += HandleSceneLoaded;

            // Cambia a la siguiente escena
            //SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);

            string nextSceneName = GetNextSceneName();
            Debug.Log("Loading Scene: " + nextSceneName);

            LevelManager.Instance.LoadScene(nextSceneName, "CrossFade");
        }
    }

    public string GetNextSceneName()
    {
        // Obtiene el índice de la escena actual
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Calcula el índice de la siguiente escena
        int nextSceneIndex = currentSceneIndex + 1;

        // Verifica si el índice de la siguiente escena es válido
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            // Obtiene el nombre de la siguiente escena
            string nextScenePath = SceneUtility.GetScenePathByBuildIndex(nextSceneIndex);
            string nextSceneName = System.IO.Path.GetFileNameWithoutExtension(nextScenePath);
            return nextSceneName;
        }
        else
        {
            Debug.LogWarning("No hay más escenas en el Build Settings.");
            return null;
        }
    }
    // Maneja la lógica después de que se carga la escena
    private void HandleSceneLoaded(Vector3 position)
    {
        // Accede a 'position' que se pasa como argumento al evento
        // y realiza las acciones necesarias, por ejemplo, mover al jugador al punto deseado
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = position;

        // Desuscribe la función para evitar problemas
        DataPersistenceManager.OnSceneLoadedEvent -= HandleSceneLoaded;
    }
}
