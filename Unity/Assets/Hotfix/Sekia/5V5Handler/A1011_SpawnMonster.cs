﻿using ETModel;
using PF;
using Vector3 = UnityEngine.Vector3;
using UnityEngine;
using UnityEngine.AI;

namespace ETHotfix
{
    [MessageHandler]
    public class A1011_SpawnMonster : AMHandler<A1011_SpawnMonster_M2C>
    {
        protected override void Run(ETModel.Session session, A1011_SpawnMonster_M2C message)
        {
            //服务端通知客户端刷新一波士兵 2战士 1法师
            TimerComponent timerComponent = ETModel.Game.Scene.GetComponent<TimerComponent>();
            ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
            GameObject bundleGameObject = (GameObject)resourcesComponent.GetAsset("Unit.unity3d", "Unit");
            ConfigComponent config = Game.Scene.GetComponent<ConfigComponent>();
            Moba5V5Component moba = Moba5V5Component.instance;

            //创建24个野怪 其中61号Boss1跳过 配置编号49-72
            for (int i = 0; i < 24; i++)
            {
                if (i == 12) continue;

                //获取角色配置
                Moba5V5Config moba5V5Config = (Moba5V5Config)config.Get(typeof(Moba5V5Config), i + 49);
                GameObject prefab = bundleGameObject.Get<GameObject>(moba5V5Config.Prefab);
                prefab.transform.position = new Vector3((float)moba5V5Config.Position[0], (float)moba5V5Config.Position[1], (float)moba5V5Config.Position[2]);
                prefab.transform.eulerAngles = new Vector3(0, moba5V5Config.Rotation, 0);

                //创建角色
                Gamer gamer = ETModel.ComponentFactory.Create<Gamer>();
                gamer.GameObject = UnityEngine.Object.Instantiate(prefab);
                gamer.GameObject.transform.SetParent(GameObject.Find($"/Global/Unit").transform, false);
                int mobaID = moba.MobaID; //获取ID
                moba.mobaIDGamers.Add(mobaID, gamer); //添加到词典

                //添加Unity组件
                NavMeshAgent navMeshAgent = gamer.GameObject.AddComponent<NavMeshAgent>();
                navMeshAgent.radius = 0.2f;
                navMeshAgent.height = 0.8f;
                navMeshAgent.angularSpeed = 360f;
                navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

                //添加ET组件
                gamer.AddComponent<GamerAnimatorComponent>();
                gamer.AddComponent<GamerMoveComponent>();
                gamer.AddComponent<GamerTurnComponent>();
                gamer.AddComponent<GamerPathComponent>();

                //设置角色
                CharacterComponent character = gamer.AddComponent<CharacterComponent>();
                character.mobaID = mobaID;
                character.group = moba5V5Config.Group; //阵营
                character.type = moba5V5Config.Type; //类型
            }
        }
    }
}