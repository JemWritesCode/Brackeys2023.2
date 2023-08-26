using JadePhoenix.Gameplay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace JadePhoenix.Gameplay
{
    [RequireComponent(typeof(Character))]
    public class CharacterPathfinder : CharacterAbility
    {
        [Header("PathfindingTarget")]
        [Tooltip("The target transform towards which the character will move")]
        public Transform Target;
        [Tooltip("The distance to the waypoint at which the movement is considered complete")]
        public float DistanceToWaypointThreshold = 1f;

        [Header("Debug")]
        [Tooltip("Enable/disable path drawing for debugging purposes")]
        public bool DebugDrawPath;

        [ReadOnly]
        public NavMeshPath AgentPath;
        [ReadOnly]
        public Vector3[] Waypoints;
        [ReadOnly]
        public int NextWaypointIndex;
        [ReadOnly]
        public Vector3 NextWaypointDirection;
        [ReadOnly]
        public float DistanceToNextWaypoint;

        protected Vector3 _direction;
        protected Vector2 _newMovement;

        protected override void PreInitialization()
        {
            base.PreInitialization();
            AgentPath = new NavMeshPath();
        }

        /// <summary>
        /// Set a new destination for the character.
        /// </summary>
        /// <param name="destinationTransform">The new destination transform</param>
        public virtual void SetNewDestination(Transform destinationTransform)
        {
            if (destinationTransform == null) { return; }

            Target = destinationTransform;
            DeterminePath(this.transform.position, destinationTransform.position);
        }

        /// <summary>
        /// Determines the path from the starting position to the target position.
        /// </summary>
        /// <param name="startingPos">The starting position of the path</param>
        /// <param name="targetPos">The ending position of the path</param>
        protected virtual void DeterminePath(Vector3 startingPos, Vector3 targetPos)
        {
            NextWaypointIndex = 0;

            NavMesh.CalculatePath(startingPos, targetPos, NavMesh.AllAreas, AgentPath);
            Waypoints = AgentPath.corners;
            if (AgentPath.corners.Length >= 2)
            {
                NextWaypointIndex = 1;
            }
        }

        public override void ProcessAbility()
        {
            if (Target == null) { return; }

            DrawDebugPath();
            DetermineNextWaypoint();
            DetermineDistanceToNextWaypoint();
            MoveController();
        }

        /// <summary>
        /// Draws the debug path if DebugDrawPath is true.
        /// </summary>
        protected virtual void DrawDebugPath()
        {
            if (DebugDrawPath && AgentPath.corners.Length != 0)
            {
                for (int i = 0; i < AgentPath.corners.Length - 1; i++)
                {
                    Debug.DrawLine(AgentPath.corners[i], AgentPath.corners[i + 1], Color.red);
                }
            }
        }

        /// <summary>
        /// Determines the next waypoint on the path.
        /// </summary>
        protected virtual void DetermineNextWaypoint()
        {
            if (Waypoints.Length <= 0 || NextWaypointIndex < 0) { return; }

            if (Vector3.Distance(this.transform.position, Waypoints[NextWaypointIndex]) <= DistanceToWaypointThreshold)
            {
                if (NextWaypointIndex + 1 < Waypoints.Length)
                {
                    NextWaypointIndex++;
                }
                else
                {
                    NextWaypointIndex = 0;
                }
            }
        }

        /// <summary>
        /// Determines the distance to the next waypoint.
        /// </summary>
        protected virtual void DetermineDistanceToNextWaypoint()
        {
            if (NextWaypointIndex <= 0)
            {
                DistanceToNextWaypoint = 0;
            }
            else
            {
                DistanceToNextWaypoint = Vector3.Distance(this.transform.position, Waypoints[NextWaypointIndex]);
            }
        }

        /// <summary>
        /// Moves the character controller based on the current target and waypoints.
        /// </summary>
        protected virtual void MoveController()
        {
            if ((Target == null) || (Waypoints.Length <= 0))
            {
                _characterMovement.SetMovement(Vector2.zero);
                return;
            }
            else
            {
                _direction = (Waypoints[NextWaypointIndex] - this.transform.position).normalized;
                _newMovement.x = _direction.x;
                _newMovement.y = _direction.z;
                _characterMovement.SetMovement(_newMovement);
            }
        }
    }
}
