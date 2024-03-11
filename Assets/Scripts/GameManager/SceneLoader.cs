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
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
            Debug.Log("Scene Loaded");
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
