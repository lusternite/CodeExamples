// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Character.h"
#include "CableComponent.h"
#include "SuperIntrospectCharacter.generated.h"



UCLASS()
class INTROSPECT_API ASuperIntrospectCharacter : public ACharacter
{
	GENERATED_BODY()

		//---------------------Components---------------------//

	/** Camera boom positioning the camera behind the character */
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = Camera, meta = (AllowPrivateAccess = "true"))
		class USpringArmComponent* CameraBoom;

	/** Follow camera */
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = Camera, meta = (AllowPrivateAccess = "true"))
		class UCameraComponent* FollowCamera;

	/** Grappling hook */
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = GrapplingHook, meta = (AllowPrivateAccess = "true"))
		class UCableComponent* GrapplingHook;

		//--------------------Public Variables----------------//
public:

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Mechanics")
		/** Determines whether the player is sliding */
		bool SlidingFlag;
	
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Mechanics")
		/** The max speed that the player moves at while sliding. */
		float SlidingSpeed;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Mechanics")
		/** Determines the rate at which character turns during slide
		The closer this value is to 0, the slower the turn */
		float SlideTurnRate;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Mechanics")
		/** Determines if the jump key is held down */
		bool JumpKeyHeld;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Mechanics")
		/** Determines if the player has double jumped since leaving the ground */
		float DoubleJumpFlag;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Movement")
		/** The default max speed that the character moves at */
		float DefaultMoveSpeed;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Movement")
		/** The default z velocity that the character gains when jumping */
		float DefaultJumpSpeed;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Movement")
		/** The maximum amount of time the player is allowed to jump after walking off a ledge */
		float JumpGracePeriodMax;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Movement")
		/** The remaining amount of time the player is allowed to jump after walking off a ledge */
		float JumpGracePeriodCurrent;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Movement")
		/** The default gravity scale of the player */
		float DefaultGravityScale;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Movement")
		/** The special gravity scale when super jumping */
		float SuperJumpGravity;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Movement")
		/** The maximum amount of time the player has to wait before ledge climbing consecutively */
		float LedgeClimbCooldownMax;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Movement")
		/** The remaining amount of time the player has to wait before ledge climbing */
		float LedgeClimbCooldownCurrent;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Movement")
		/** The amount of velocity to launch the character upwards when ledge climbing */
		float LedgeClimbZLaunchVelocity;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Movement")
		/** The amount of velocity to launch the character forwards when ledge climbing */
		float LedgeClimbXLaunchVelocity;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Movement")
		/** A flag that determines if the forward launch of the player has been applied yet */
		bool LedgeClimbForwardLaunchFlag;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "GrapplingHook")
		/** A flag that determines if the grapple hook has been fired */
		bool GrappleHookFired;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "GrapplingHook")
		/** A flag that determines if the grapple hook has hit a surface */
		bool GrappleHookHit;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "GrapplingHook")
		/** The component that the grapple hook has hit */
		USceneComponent* GrappleHookHitComponent;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "GrapplingHook")
		/** The name of the socket that the grapple hook has hit */
		FName GrappleHookHitSocketName;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "GrapplingHook")
		/** The remaining duration that the grapple hook can be out */
		float GrappleHookDurationCurrent;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "GrapplingHook")
		/** The maximum duration that the grapple hook can be out for */
		float GrappleHookDurationMax;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "GrapplingHook")
		/** The end location of the grapple hook in world space */
		FVector GrappleHookLocation;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "GrapplingHook")
		/** The direction the hook is travelling in world space */
		FVector GrappleHookDirection;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "GrapplingHook")
		/** The speed the hook travels per second */
		float GrappleHookTravelSpeed;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "GrapplingHook")
		/** The speed the hook pulls the player in per second */
		float GrappleHookReelSpeed;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "GrapplingHook")
		/** The default max hook length */
		float GrappleHookMaxLengthDefault;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "GrapplingHook")
		/** The current max hook length, adjusted when soft aiming */
		float GrappleHookMaxLengthCurrent;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "GrapplingHook")
		/** A flag that determines if the the grapple hook is seeking an aim assisted location */
		bool GrappleHookAimAssistFlag;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "GrapplingHook")
		/** The location of the aim assisted target component */
		FVector GrappleHookAimAssistLocation;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "GrapplingHook")
		/** A reference to the component that aim assist has found */
		USceneComponent* GrappleHookAimAssistComponent;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "GrapplingHook")
		/** The name of the socket/bone that the aim assist has found */
		FName GrappleHookAimAssistSocketName;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "GrapplingHook")
		/** A flag that determines if the player is hanging at the grapple point */
		bool GrappleHookHanging;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Camera")
		/** The default camera boom socket offset on the ground  */
		FVector DefaultCameraBoomSocketOffsetGround;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Camera")
		/** The default camera boom socket offset in the air  */
		FVector DefaultCameraBoomSocketOffsetAir;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Camera")
		/** The vector values that the camera boom socket offset interps to  */
		FVector DesiredCameraBoomSocketOffset;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Camera")
		/** The default speed at which the camera follows the character  */
		float DefaultCameraLagSpeed;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Camera")
		/** Determines if the camera is low angle or not */
		bool LowAngleCameraFlag;

		//--------------------Private Variables----------------//
private:
	float CharacterDeltaTime;

public:
	// Sets default values for this character's properties
	ASuperIntrospectCharacter();

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

public:	
	// Called every frame
	virtual void Tick(float DeltaTime) override;

	//----------------------Protected Functions-------------------//
protected:
	// Called to bind functionality to input
	virtual void SetupPlayerInputComponent(class UInputComponent* PlayerInputComponent) override;

	/** Called when movement mode is changed */
	virtual void OnMovementModeChanged(EMovementMode PrevMovementMode, uint8 PreviousCustomMode = 0) override;

	/** Called for camera yaw input */
	void TurnCameraLeftRight(float Val);

	void TurnCameraUpDown(float Val);

	/**
	* Called via input to turn at a given rate.
	* @param Rate	This is a normalized rate, i.e. 1.0 means 100% of desired turn rate
	*/
	void TurnAtRate(float Rate);

	/**
	* Called via input to turn look up/down at a given rate.
	* @param Rate	This is a normalized rate, i.e. 1.0 means 100% of desired turn rate
	*/
	void LookUpAtRate(float Rate);
	
	UFUNCTION(BlueprintCallable, Category = "Controls")
		/** Called for forwards/backward input */
		void MoveForward(float Value);

	UFUNCTION(BlueprintCallable, Category = "Controls")
		/** Called for side to side input */
		void MoveRight(float Value);

	UFUNCTION(BlueprintCallable, Category = "Controls")
		/** Called for jumping input */
		virtual void Jump() override;

	UFUNCTION(BlueprintCallable, Category = "Controls")
		/** Called when jumping input is released */
		void JumpStop();

	UFUNCTION(BlueprintCallable, Category = "Controls")
		/** Called for double jumping input */
		void DoubleJump();

	UFUNCTION(BlueprintCallable, Category = "Controls")
		/** Called for super jumping input */
		void SuperJump();

	UFUNCTION(BlueprintCallable, Category = "Controls")
		/** Handles checking for ledges and climbing them */
		void HandleLedgeClimbing();

	UFUNCTION(BlueprintCallable, Category = "Controls")
		/** Calculates the correct launch height for this ledge */
		FVector GetLedgeClimbHeight();

	UFUNCTION(BlueprintCallable, Category = "Controls")
		/** Called for slide motion */
		void Slide();

	UFUNCTION(BlueprintCallable, Category = "Controls")
		/** Called when slide motion stops */
		void SlideStop();

	UFUNCTION(BlueprintCallable, Category = "Controls")
		/** 
		* Implementation of character sliding
		* Makes the character slide forwards if slide key is held 
		*/
		void HandleSlide();

	UFUNCTION(BlueprintCallable, Category = "GrapplingHook")
		/**
		* Implementation of grapple hook
		* Handles grapple traversal, hitting objects, and reeling player in
		*/
		void HandleGrapplingHook();

	UFUNCTION(BlueprintCallable, Category = "GrapplingHook")
		/** Checks if grapple hook has been out for its maximum duration */
		void HandleGrapplingHookDuration();

	UFUNCTION(BlueprintCallable, Category = "GrapplingHook")
		/** Turns the grapple hook off */
		void ToggleHookOff();

	UFUNCTION(BlueprintCallable, Category = "GrapplingHook")
		/** 
		* Returns the end location of the grappling hook in world space
		* Used primarily after grapple has hit a target
		*/
		FVector GetGrapplingHookEndLocation();

	UFUNCTION(BlueprintCallable, Category = "GrapplingHook")
		/** Updates the grapple hook end location */
		void SetGrapplingHookEndLocation();

	UFUNCTION(BlueprintCallable, Category = "GrapplingHook")
		/** Checks and handles hook collisions */
		bool CheckHookCollision();

	UFUNCTION(BlueprintCallable, Category = "GrapplingHook")
		/** Sets variables and hook state once hook hits target */
		void SetHookHit();

	UFUNCTION(BlueprintCallable, Category = "GrapplingHook")
		/** Rotates the player towards the direction of the grappling hook */
		void TurnPlayerToGrappleDirection();

	UFUNCTION(BlueprintCallable, Category = "Camera")
		/** Interps the socket offset to the desired values */
		void HandleCameraBoomSocketOffsetInterp();

	UFUNCTION(BlueprintCallable, Category = "Camera")
		/** 
		* Changes Y value of the camera offset when camera is low
		* Used to avoid character blocking camera in low angles
		*/
		void HandleLowAngleCamera();

	//----------------------Private Functions-------------------//
private:
	UFUNCTION(BlueprintCallable, Category = "Camera")
		/** Counts down timers and calls functions specific to them */
		void HandleTimers();
};
