using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Blake3;

public class Scene : MonoBehaviour {

	// Use this for initialization
	void Start () {
		byte[] input = new byte[]{1,2,3,4,5};
		byte[] output = new byte[Hasher.OUTPUT_SIZE];

		Hasher.Calc(input, output);
		Debug.Log(string.Format("in : {0}, out:{1}", input, BytesToString(output)));
	}
	
	string BytesToString(byte[] input)
	{
		var builder = new System.Text.StringBuilder();
		foreach(var v in input)
		{
			builder.Append(v.ToString("X"));
		}

		return builder.ToString();
	}
}
