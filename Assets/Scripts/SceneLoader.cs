using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class SceneLoader : MonoBehaviour
{

	[SerializeField]
	private Text text;

	private string[] messages = {
		"Penteando o cabelo",
		"Tomando uma ducha",
		"Fazendo o dever de casa",
		"Arrumando o quarto",
		"Carregando o telefone",
		"Abrindo as janelas",
		"Escovando os dentes",
		"Estudando para teste surpresa",
		"Pesquisando vida alienigena",
		"Baixando músicas novas",
		"Assistindo videos na internet"
	};
	
	// Update is called once per frame
	void Start ()
	{
		StartCoroutine (LoadNewScene ());
	}

	IEnumerator LoadNewScene ()
	{

		AsyncOperation async = SceneManager.LoadSceneAsync ("FemaleAvatar");
		int i, porcentage;

		while (!async.isDone) {
			porcentage = Convert.ToInt32 (async.progress * 100.0f);
			i = porcentage % messages.Length;
			text.text = messages [i] + "...";
			yield return null;
		}

	}
}
