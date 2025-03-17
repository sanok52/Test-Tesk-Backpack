using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

public class WebManager : Singleton<WebManager>
{
    [SerializeField] private string url;
    [SerializeField] private bool useToken = true;
    [SerializeField] private string bearerToken = "kPERnYcWAY46xaSy8CEzanosAgsWM84Nx7SKM4QBSqPq6c7StWfGxzhxPfDh8MaP"; //Не безопасно здесь хранить, но для тестового пока сойдёт

    public UnityEvent<string> OnRequest;

    public void SendRequestPost(string data, UnityAction<UnityWebRequest> onComplite = null)
    {
        StartCoroutine(SendRequestPostCoroutine(data, onComplite));
    }

    private IEnumerator SendRequestPostCoroutine(string data, UnityAction<UnityWebRequest> onComplite = null)
    {
        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(url, data))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            //request.SetRequestHeader("Content-Type", "application/json");
            if (useToken)
                request.SetRequestHeader("Authorization", "Bearer " + bearerToken);

            yield return request.SendWebRequest();

            onComplite?.Invoke(request);
            OnRequest?.Invoke(request.downloadHandler.text.Replace(":", ":<color=yellow>").Replace(",", "</color></color></color>,\n"));
            Debug.Log($"{request.result}:\n" +
                $"{request.downloadHandler.text}");

        }
    }
}