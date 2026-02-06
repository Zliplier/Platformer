using System;
using System.Collections;
using UnityEngine;
using Zlipacket.CoreZlipacket.Tools;
using Color = UnityEngine.Color;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Player.Script
{
	public class PlayerMovement : PlayerScript
	{
		public bool IsGrounded { get; private set; } = true;
		public bool IsFacingRight { get; private set; } = true;
		
		public float acceleration => IsGrounded? MoveStats.groundAccel : MoveStats.airAccel;
		public float deceleration => IsGrounded? MoveStats.groundDecel : MoveStats.airDecel;
		
		//Jump
		private int jumpUsed = 0;
		private float jumpBufferTime = 0f;
		private float jumpCoyoteTime = 0f;
		private Coroutine co_Jumping = null; //Coroutine will run until the apex time is reached or landing first.
		public bool IsJumping => co_Jumping != null;
		private bool jumpCutFlag = false;
		
		//Input
		private Vector3 movementInput = Vector3.zero;
		private Vector3 mouseInput => Camera.main.ScreenToWorldPoint(new Vector3(playerInputMap.mousePosition.x, playerInputMap.mousePosition.y, Mathf.Abs(Camera.main.transform.position.z)));
		
		//Aiming
		private Vector3 aimDirection = Vector3.zero;
		private float aimMultiplier = 1f;
		
		//Velocity
		private Vector3 velocity => moveVelocity + jumpVelocity + gravityVelocity + additionalyVelocity;
		private Vector3 moveVelocity = Vector3.zero;
		private Vector3 jumpVelocity = Vector3.zero;
		private Vector3 gravityVelocity = Vector3.zero;
		private Vector3 additionalyVelocity = Vector3.zero;

		private void OnEnable()
		{
			playerInputMap.movementEvent += MovementInput;
			playerInputMap.jumpEvent += JumpInput;
		}

		private void OnDisable()
		{
			playerInputMap.movementEvent -= MovementInput;
			playerInputMap.jumpEvent -= JumpInput;
		}

		private void MovementInput(Vector2 input) =>
			movementInput = input;

		private void JumpInput(bool isHolding)
		{
			//Jump Start
			if (isHolding)
			{
				jumpBufferTime = MoveStats.jumpBufferTime;
			}
			//Jump Cut
			else if (IsJumping)
			{
				JumpCut();
			}
		}

		private void Update()
		{
			UpdateTimer(Time.deltaTime);
			MouseSideCheck();
			TurnCheck();
			
			DebugVisualization();
		}

		private void FixedUpdate()
		{
			CollisionCheck();
			HandleAimDirection();
			
			ApplyGravity();
			HandleHorizontal();
			HandleVertical();
			ApplyMovement();
		}

		#region Turning

		private void MouseSideCheck()
		{
			if (mouseInput.x > bodyRoot.transform.position.x && !IsFacingRight)
			{
				Flip(true);
			}
			else if (mouseInput.x < bodyRoot.transform.position.x && IsFacingRight)
			{
				Flip(false);
			}
		}

		private void Flip(bool isFlipRight)
		{
			if (isFlipRight)
			{
				IsFacingRight = true;
				bodyRoot.transform.localRotation =
					Quaternion.Euler(0, 0, 0);
			}
			else
			{
				IsFacingRight = false;
				bodyRoot.transform.localRotation =
					Quaternion.Euler(0, 180, 0);
			}
		}
		
		private void TurnCheck()
        {
	        //Movement
	        if (moveVelocity.x > 0 && movementInput.x < 0)
		        Turn(false);
	        else if (moveVelocity.x < 0 && movementInput.x > 0)
		        Turn(true);
        }

        private void Turn(bool isTurnRight)
        {
	        moveVelocity.x -= moveVelocity.x * MoveStats.turnCompensation;
        }

		#endregion
		
		#region Movement
		
		private void HandleHorizontal()
		{
			float targetSpeed = 0;
			float accelRate = 0;

			if (!ZlipUtilities.ApproximatelyWithMargin(movementInput.x, 0f, 0.01f))
			{
				targetSpeed = movementInput.x * MoveStats.walkSpeed;
				accelRate = acceleration;
			}
			else
			{
				targetSpeed = 0;
				accelRate = deceleration;
			}
			
			moveVelocity.x = Mathf.Lerp(
				moveVelocity.x,
				targetSpeed * aimMultiplier,
				accelRate * Time.deltaTime
			);
			
			//Debug.Log(velocity);
			//Debug.Log(accelRate);
		}
		
		private void HandleVertical()
		{
			JumpCheck();
		}

		private void HandleAimDirection()
		{
			Vector3 movement = moveVelocity + jumpVelocity;
			if (movement == Vector3.zero)
				return;
			
			Vector3 normalizedMovement = movement.normalized;
			aimDirection = (mouseInput - transform.position).normalized;
			float dot = Vector3.Dot(normalizedMovement, aimDirection);
			
			if (dot >= MoveStats.aimDotThreshold)
			{
				aimMultiplier = 1f +
				                (MoveStats.aimMovementMultiplier * dot.Remap(MoveStats.aimDotThreshold, 1f, 0f, 1f));
			}
			else
			{
				aimMultiplier = 1f;
			}
			
			//Debug.Log("Aim Dot: " + dot);
		}
		
		private void ApplyMovement()
		{
			rb.linearVelocity = velocity;
		}
		
		#endregion

		#region Jump

		private void JumpCheck()
		{
			if (jumpBufferTime > 0)
			{
				InitiateJump();
			}
		}

		private void InitiateJump()
		{
			//Jump on ground.
            if (!IsJumping && (IsGrounded || jumpCoyoteTime > 0) && jumpUsed < MoveStats.jumpAllowed)
            	Jump(1);
            //Double Jump.
            else if (!IsGrounded && jumpUsed < MoveStats.jumpAllowed)
            	Jump(1);
            //Air Jump.
            /*else if (!IsGrounded && jumpUsed < MoveStats.jumpAllowed)
            	Jump(1);*/
		}
		
		private void Jump(int jumpUsage)
		{
			jumpUsed += jumpUsage;
			
			if (IsJumping)
				StopCoroutine(co_Jumping);
			
			co_Jumping = StartCoroutine(Jumping());

			jumpBufferTime = 0f;
		}

		private IEnumerator Jumping()
		{
			float jumpTime = 0f;
			float jumpPercentage = 0f;
			float initialJumpVelocity = Mathf.Abs((2f * MoveStats.jumpHeight) / Mathf.Pow(MoveStats.timeTillApex, 2f)) * MoveStats.timeTillApex;
			
			//Debug.Log("Start Jumping with Force: " + initialJumpVelocity);
			
			while (jumpTime < MoveStats.timeTillApex)
			{
				yield return new WaitForFixedUpdate();
				jumpPercentage = Mathf.Clamp(jumpTime / MoveStats.timeTillApex, 0f, 1f);
				
				jumpVelocity.y = Mathf.Lerp(initialJumpVelocity * aimMultiplier, 0f, MoveStats.jumpCurve.Evaluate(jumpPercentage));
				
				jumpTime += Time.fixedDeltaTime;
				if (jumpCutFlag)
					break;
			}
			
			//Debug.Log("Jump Apex Reached");
			jumpVelocity = Vector3.zero;
			jumpCutFlag = false;
			yield return new WaitForSeconds(MoveStats.hangTimeApex);
			co_Jumping = null;
			//Debug.Log("Stop Jumping");
		}

		private void JumpCut()
		{
			//CancelJump();
			jumpCutFlag = true;
		}

		private void CancelJump()
		{
			if (IsJumping)
			{
				StopCoroutine(co_Jumping);
				co_Jumping = null;
			}
			
			jumpCutFlag = false;
			jumpVelocity = Vector3.zero;
			gravityVelocity = Vector3.zero;
		}
		
		#endregion

		#region Gravity

		private void ApplyGravity()
		{
			if (IsJumping || IsGrounded)
				gravityVelocity = Vector3.zero;
			else
				gravityVelocity = Vector3.Lerp(gravityVelocity, MoveStats.gravity, MoveStats.gravityAccel * Time.fixedDeltaTime);
		}

		#endregion

		#region Timer

		private void UpdateTimer(float deltaTime)
		{
			JumpTimer(deltaTime);
		}

		private void JumpTimer(float deltaTime)
		{
			if (jumpBufferTime > 0)
				jumpBufferTime -= deltaTime;
			if (jumpCoyoteTime > 0)
				jumpCoyoteTime -= deltaTime;
		}

		#endregion
		
		#region Collision

		private void CollisionCheck()
        {
        	GroundCheck();
        }

        private void GroundCheck()
        {
        	Vector3 boxSize = MoveStats.groundBoxSize;
        	Vector3 boxOrigin = feetPos.position;

        	Physics.BoxCast(boxOrigin, boxSize / 2, Vector3.down, out var groundHit, Quaternion.identity,
        		MoveStats.groundRayDistance, MoveStats.groundLayer);
	        if (groundHit.collider != null)
	        {
		        if (!IsGrounded)
		        {
			        IsGrounded = true;
			        Landing();
		        }
	        }
	        else
	        {
		        if (IsGrounded)
		        {
			        IsGrounded = false;
			        TakeOff();
		        }
	        }
        }

        private void Landing()
        {
	        CancelJump();
	        
	        jumpUsed = 0;
	        jumpCoyoteTime = 0f;
	        
	        gravityVelocity = Vector3.zero;
        }

        private void TakeOff()
        {
	        jumpCoyoteTime = MoveStats.jumpCoyoteTime;
        }

		#endregion
		
		#region Debug

		private void DebugVisualization()
        {
	        DebugVelocity();
	        DebugGravity();
	        DebugIsJumping();
        	DebugGroundCheck();
        }

		private void DebugVelocity()
		{
			if (!MoveStats.showVelocity)
				return;
			
			Debug.Log(velocity);
		}

		private void DebugGravity()
		{
			if (!MoveStats.showGravity)
				return;
			
			Debug.Log(gravityVelocity);
		}
		
		private void DebugIsJumping()
		{
			if (!MoveStats.showJumping)
				return;
			
			Debug.Log(IsJumping);
		}
		
        private void DebugGroundCheck()
        {
        	if (!MoveStats.showGroundBox)
        		return;
        	
        	Vector3 boxSize = MoveStats.groundBoxSize;
        	Vector3 boxOrigin = feetPos.position;
        	Color color = IsGrounded ? Color.green : Color.red;
        	
        	ExtDebug.DrawBoxCastBox(boxOrigin, boxSize / 2, Quaternion.identity, Vector3.down, MoveStats.groundRayDistance, color);
        }

		#endregion
	}
}