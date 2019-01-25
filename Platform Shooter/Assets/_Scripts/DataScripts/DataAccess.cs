using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DataAccess {

	public EngineMoveData EngineMoveData { get { return engineMoveData; } }
	EngineMoveData engineMoveData;

	public void Load() {
		engineMoveData = AssetDatabase.LoadAssetAtPath("Assets/Data/Move/EngineMoveData.asset", typeof(EngineMoveData)) as EngineMoveData;
	}

	// Singleton
	private DataAccess() {
		Load();
	}

	private static DataAccess instance;
	public static DataAccess Instance {
		get {
			if (instance == null) instance = new DataAccess();
			return instance;
		}
	}

}
