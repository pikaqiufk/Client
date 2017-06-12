using UnityEngine;

namespace ToLuaEx
{
	public class LuaComponentCollider : LuaComponent
	{
		//函数名字定义
		private static class FuncNameCollider
		{
			public static readonly string OnTriggerEnter = "OnTriggerEnter";
			public static readonly string OnTriggerExit = "OnTriggerExit";
			public static readonly string OnTriggerStay = "OnTriggerStay";
			public static readonly string OnCollisionEnter = "OnCollisionEnter";
			public static readonly string OnCollisionExit = "OnCollisionExit";
			public static readonly string OnCollisionStay = "OnCollisionStay";
		};
            
		//初始化函数，可以被重写，已添加其他
		protected override bool Init()
		{
			if (!base.Init())
			{
				UnityEngine.Debug.Log("Init()");
				return false;
			}

			AddFunc(FuncNameCollider.OnTriggerEnter);
			AddFunc(FuncNameCollider.OnTriggerExit);
			AddFunc(FuncNameCollider.OnTriggerStay);
			AddFunc(FuncNameCollider.OnCollisionEnter);
			AddFunc(FuncNameCollider.OnCollisionExit);
			AddFunc(FuncNameCollider.OnCollisionStay);
			return true;
		}


		private void OnTriggerEnter(Collider other)
		{
			CallLuaFunction(FuncNameCollider.OnTriggerEnter, mSelfTable, gameObject, other);
		}

		private void OnTriggerExit(Collider other)
		{
			CallLuaFunction(FuncNameCollider.OnTriggerExit, mSelfTable, gameObject, other);
		}

		private void OnTriggerStay(Collider other)
		{
			CallLuaFunction(FuncNameCollider.OnTriggerStay, mSelfTable, gameObject, other);
		}

		private void OnCollisionEnter(Collision other)
		{
			CallLuaFunction(FuncNameCollider.OnCollisionEnter, mSelfTable, gameObject, other);
		}

		private void OnCollisionExit(Collision other)
		{
			CallLuaFunction(FuncNameCollider.OnCollisionExit, mSelfTable, gameObject, other);
		}

		private void OnCollisionStay(Collision other)
		{
			CallLuaFunction(FuncNameCollider.OnCollisionStay, mSelfTable, gameObject, other);
		}
	}
}