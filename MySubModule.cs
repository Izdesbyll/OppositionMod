namespace OppositionMod
{


    using System;
    using System.Linq;
    using System.Collections.Generic;

    using TaleWorlds.Core;
    using TaleWorlds.Engine;
    using TaleWorlds.Library;
    using TaleWorlds.Localization;
    using TaleWorlds.MountAndBlade;
    using TaleWorlds.InputSystem;


    public class MySubModule : MBSubModuleBase
    {


        protected override void OnSubModuleLoad()
        {
            Module.CurrentModule.AddInitialStateOption(new InitialStateOption("Message",
            new TextObject("Mod", null),
            9990,
            () => { InformationManager.DisplayMessage(new InformationMessage("This is Opposition Mod")); },
            () => { return (false, null); }));
        }

        public override void OnMissionBehaviorInitialize(Mission mission)
        {
            if (Mission.Current != null)
            {

                InformationManager.DisplayMessage(new InformationMessage("We are on the mission!"));
            }
        }

            protected override void OnApplicationTick(float dt)
             {
                 Game gameCurrent = Game.Current;
                 if (gameCurrent == null) return;
                 if (gameCurrent.CurrentState != Game.State.Running) return;
            if (Mission.Current == null) return;
                FleeingBehavior.Tick(dt);
                 }
             }
    internal static class FleeingBehavior
    {
        private const float RANGE = 10f;
        internal static void Tick(float f)
        {
            Mission missionCurrent = Mission.Current;
            
            if (missionCurrent.MainAgent != null)
            {
                Agent a = missionCurrent.MainAgent;
                Vec3 pos = a.Position;
                Team team = a.Team;
                int i = 0;
                // we want to get the name of nearby enemy unit 
                IEnumerable<Agent> agents = null;
                IEnumerable<Agent> farAgents = null;
                Agent.ControllerType onController = Agent.ControllerType.AI;


                if (pos != null && team != null)
                {
                    farAgents = missionCurrent.GetNearbyEnemyAgents(pos.AsVec2, RANGE + 5f, team);
                    agents = missionCurrent.GetNearbyEnemyAgents(pos.AsVec2, RANGE, team);
                    Agent agent1 = agents != null && agents.Count() > 0 ? agents.ElementAt(0) : null; ;

                    //we want to select our enemy out of all nearby enemies
                    if (agents.Count() > 1)
                    {
                        for (i = 1; i < agents.Count(); i++)
                        {
                            Agent enemy1 = agents.ElementAt(i);
                            Agent enemy2 = agents.ElementAt(i - 1);
                            Vec3 vector1 = enemy1.Position;
                            float distance1 = MathF.Sqrt((vector1.x - pos.x) * (vector1.x - pos.x) + (vector1.y - pos.y) * (vector1.y - pos.y));
                            Vec3 vector2 = enemy2.Position;
                            float distance2 = MathF.Sqrt((vector2.x - pos.x) * (vector2.x - pos.x) + (vector2.y - pos.y) * (vector2.y - pos.y));
                            if (distance1 < distance2)
                            {
                                agent1 = enemy1;
                            }
                            else
                            {
                                agent1 = enemy2;
                            }
                        }
                    }

                    Agent enemy = agent1;
                    foreach (Agent agent in farAgents)
                    {
                        agent.Controller = onController;
                    }

                    if (enemy != null)
                    {
                        
                        string name = enemy.Name.ToString();
                        //InformationManager.DisplayMessage(new InformationMessage("Player 2 -> " + name));

                        // we want the enemy to act on pressing button

                        Vec2 forwardDirection = enemy.LookDirection.AsVec2;
                        Vec3 enemyPosition = enemy.Position;


                        Agent.ControllerType offController = Agent.ControllerType.None;
                        
                        Agent.MovementControlFlag goForward = Agent.MovementControlFlag.Forward;
                        Agent.MovementControlFlag goBackward = Agent.MovementControlFlag.Backward;
                        Agent.MovementControlFlag goRight = Agent.MovementControlFlag.StrafeRight;
                        Agent.MovementControlFlag goLeft = Agent.MovementControlFlag.StrafeLeft;
                        Agent.MovementControlFlag attackRight = Agent.MovementControlFlag.AttackRight;
                        Agent.MovementControlFlag attackLeft = Agent.MovementControlFlag.AttackLeft;
                        Agent.MovementControlFlag attackUp = Agent.MovementControlFlag.AttackUp;
                        Agent.MovementControlFlag attackDown = Agent.MovementControlFlag.AttackDown;
                        Agent.MovementControlFlag defendRight = Agent.MovementControlFlag.DefendRight;
                        Agent.MovementControlFlag defendLeft = Agent.MovementControlFlag.DefendLeft;
                        Agent.MovementControlFlag defendUp = Agent.MovementControlFlag.DefendUp;
                        Agent.MovementControlFlag defendDown = Agent.MovementControlFlag.DefendUp;
                        Agent.MovementControlFlag defend = Agent.MovementControlFlag.DefendAuto;
                
                        enemy.Controller = offController;

                        


                        float Xdifference = pos.x - enemyPosition.x;
                        float Ydifference = pos.y - enemyPosition.y;
                        double PlayersAngle = Math.Atan2(Xdifference, Ydifference);
                        double enemyAngle = Math.Atan2(forwardDirection.x, forwardDirection.y);
                        InformationManager.DisplayMessage(new InformationMessage("PlayersAngle -> " + PlayersAngle));
                        InformationManager.DisplayMessage(new InformationMessage("EnemyAngle -> " + enemyAngle));
                        InformationManager.DisplayMessage(new InformationMessage("Ydifference -> " + Ydifference));
                        InformationManager.DisplayMessage(new InformationMessage("Xdifference -> " + Xdifference));

                            if (PlayersAngle < enemyAngle)
                            {
                                forwardDirection.RotateCCW(0.1f);
                                enemy.SetMovementDirection(in forwardDirection);
                            }
                            if (PlayersAngle > enemyAngle)
                            {
                                forwardDirection.RotateCCW(-0.1f);
                                enemy.SetMovementDirection(in forwardDirection);
                            }


                        if (Input.IsKeyDown(InputKey.NumpadPlus))
                        {
                            enemy.MovementFlags = defendUp;
                        }
                        if (Input.IsKeyReleased(InputKey.NumpadPlus))
                        { 
                            enemy.MovementFlags = Agent.MovementControlFlag.Action;
                        }


                        if (Input.IsKeyDown(InputKey.Numpad8))
                        {

                            if (Input.IsKeyPressed(InputKey.Numpad0))
                            {
                                enemy.MovementFlags = attackUp;
                            }
                            if (Input.IsKeyReleased(InputKey.Numpad0))
                            {
                                enemy.MovementFlags = Agent.MovementControlFlag.Action;
                            }

                        }

                        if (Input.IsKeyDown(InputKey.Numpad5))
                        { 
                            if (Input.IsKeyPressed(InputKey.Numpad0))
                            {
                                enemy.MovementFlags = attackDown;
                            }
                            if (Input.IsKeyReleased(InputKey.Numpad0))
                            {
                                enemy.MovementFlags = Agent.MovementControlFlag.Action;
                            }

                        }


                        if (Input.IsKeyDown(InputKey.Numpad4))
                        {

                            if (Input.IsKeyPressed(InputKey.Numpad0))
                            {
                                enemy.MovementFlags = attackLeft;
                            }
                            if (Input.IsKeyReleased(InputKey.Numpad0))
                            {
                                enemy.MovementFlags = Agent.MovementControlFlag.Action;
                            }

                        }
                        if (Input.IsKeyDown(InputKey.Numpad6))
                        {
                            if (Input.IsKeyPressed(InputKey.Numpad0))
                            {
                                enemy.MovementFlags = attackRight;
                            }
                            if (Input.IsKeyReleased(InputKey.Numpad0))
                            {
                                enemy.MovementFlags = Agent.MovementControlFlag.Action;
                            }
                        }

                        if (Input.IsKeyDown(InputKey.O))
                        {
                            enemy.MovementFlags = goBackward;
                        }

                        if (Input.IsKeyReleased(InputKey.O))
                        {
                            enemy.MovementFlags = Agent.MovementControlFlag.Action;
                        }


                        if (Input.IsKeyDown(InputKey.L))
                        {
                            enemy.MovementFlags = goForward;
                        }
                        if (Input.IsKeyReleased(InputKey.L))
                        {
                            enemy.MovementFlags = Agent.MovementControlFlag.Action;
                        }

                        if (Input.IsKeyDown(InputKey.SemiColon))
                        {
                            enemy.MovementFlags = goLeft;
                        }
                        if (Input.IsKeyReleased(InputKey.SemiColon))
                        {
                            enemy.MovementFlags = Agent.MovementControlFlag.Action;
                        }

                        if (Input.IsKeyDown(InputKey.K))
                        {
                            enemy.MovementFlags = goRight;
                        }
                        if (Input.IsKeyReleased(InputKey.K))
                        {
                            enemy.MovementFlags = Agent.MovementControlFlag.Action;
                        }

                    }
                 
                }

                // we want this code to work only with one unit till his death or getting out of range
                
            }
        }

    }
    

    }
