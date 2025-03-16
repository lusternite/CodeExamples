// Fill out your copyright notice in the Description page of Project Settings.

#include "Introspect.h"
#include "SuperIntrospectCharacter.h"


// Sets default values
ASuperIntrospectCharacter::ASuperIntrospectCharacter()
{
	// Set this character to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;

	// Set default movement variable values
	SlidingSpeed = 1000.0f;
	SlideTurnRate = 0.2f;
	JumpGracePeriodMax = 0.5f;
	SuperJumpGravity = 2.0f;
	LedgeClimbCooldownMax = 0.2f;
	LedgeClimbZLaunchVelocity = 1500.0f;
	LedgeClimbXLaunchVelocity = 300.0f;

	// Don't rotate when the controller rotates. Let that just affect the camera.
	bUseControllerRotationPitch = false;
	bUseControllerRotationYaw = false;
	bUseControllerRotationRoll = false;

	// Configure character movement component properties
	//GetCharacterMovement()->bUseControllerDesiredRotation = true;
	GetCharacterMovement()->bOrientRotationToMovement = true; // Character moves in the direction of input...	
	GetCharacterMovement()->RotationRate = FRotator(0.0f, 90.0f, 0.0f); // ...at this rotation rate
	GetCharacterMovement()->JumpZVelocity = 600.f;
	GetCharacterMovement()->AirControl = 0.8f;

	// Create a camera boom (pulls in towards the player if there is a collision)
	CameraBoom = CreateDefaultSubobject<USpringArmComponent>(TEXT("CameraBoom"));
	CameraBoom->SetupAttachment(RootComponent);
	CameraBoom->TargetArmLength = 500.0f; // The camera follows at this distance behind the character	
	CameraBoom->bUsePawnControlRotation = true; // Rotate the arm based on the controller
	CameraBoom->bEnableCameraLag = true;
	CameraBoom->bEnableCameraRotationLag = true;
	CameraBoom->CameraLagMaxDistance = 120.0f;
	CameraBoom->CameraLagSpeed = 3.0f;
	CameraBoom->SocketOffset.Z = 50.0f;

	// Create a follow camera
	FollowCamera = CreateDefaultSubobject<UCameraComponent>(TEXT("FollowCamera"));
	FollowCamera->SetupAttachment(CameraBoom, USpringArmComponent::SocketName); // Attach the camera to the end of the boom and let the boom adjust to match the controller orientation
	FollowCamera->bUsePawnControlRotation = false; // Camera does not rotate relative to arm

	// Create a grappling hook
	GrapplingHook = CreateDefaultSubobject<UCableComponent>(TEXT("GrapplingHook"));
	GrapplingHook->SetupAttachment(RootComponent);
	GrapplingHook->SetRelativeLocation(FVector(0.0f, 0.0f, 0.0f));
}

// Called when the game starts or when spawned
void ASuperIntrospectCharacter::BeginPlay()
{
	Super::BeginPlay();

	//Set default variables
	DefaultMoveSpeed = GetMovementComponent()->GetMaxSpeed();
	DefaultJumpSpeed = GetCharacterMovement()->JumpZVelocity;
	DefaultGravityScale = GetCharacterMovement()->GravityScale;
	DefaultCameraBoomSocketOffsetGround = CameraBoom->SocketOffset;
	DesiredCameraBoomSocketOffset = CameraBoom->SocketOffset;
}

// Called every frame
void ASuperIntrospectCharacter::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

	CharacterDeltaTime = DeltaTime;

	// Check and resolve cooldown timers
	HandleTimers();

	// Check and resolve character sliding
	HandleSlide();

	// Check and resolve camera position interpolation
	HandleCameraBoomSocketOffsetInterp();

	// Check and resolve low angle camera position
	HandleLowAngleCamera();

	// Check and resolve ledge climbing
	HandleLedgeClimbing();
}

// Called to bind functionality to input
void ASuperIntrospectCharacter::SetupPlayerInputComponent(UInputComponent* PlayerInputComponent)
{
	Super::SetupPlayerInputComponent(PlayerInputComponent);

	//Bind camera movement
	PlayerInputComponent->BindAxis("Turn", this, &ASuperIntrospectCharacter::TurnCameraLeftRight);
	PlayerInputComponent->BindAxis("TurnRate", this, &ASuperIntrospectCharacter::TurnAtRate);
	PlayerInputComponent->BindAxis("LookUp", this, &ASuperIntrospectCharacter::TurnCameraUpDown);
	PlayerInputComponent->BindAxis("LookUpRate", this, &ASuperIntrospectCharacter::LookUpAtRate);

	//Bind directional movement
	PlayerInputComponent->BindAxis("MoveForward", this, &ASuperIntrospectCharacter::MoveForward);
	PlayerInputComponent->BindAxis("MoveRight", this, &ASuperIntrospectCharacter::MoveRight);

	//Bind jumping action
	PlayerInputComponent->BindAction("Jump", IE_Pressed, this, &ASuperIntrospectCharacter::Jump);
	PlayerInputComponent->BindAction("Jump", IE_Released, this, &ASuperIntrospectCharacter::JumpStop);

	//Bind sliding action
	PlayerInputComponent->BindAction("Slide", IE_Pressed, this, &ASuperIntrospectCharacter::Slide);
	PlayerInputComponent->BindAction("Slide", IE_Released, this, &ASuperIntrospectCharacter::SlideStop);
}

void ASuperIntrospectCharacter::OnMovementModeChanged(EMovementMode PrevMovementMode, uint8 PreviousCustomMode)
{
	//Changes made if just left walking movement state
	if (PrevMovementMode == EMovementMode::MOVE_Walking)
	{
		DesiredCameraBoomSocketOffset = DefaultCameraBoomSocketOffsetAir;
	}
	//Changes made if just left jumping/falling movement state
	else if (PrevMovementMode == EMovementMode::MOVE_Falling)
	{
		DoubleJumpFlag = false;
		DesiredCameraBoomSocketOffset = DefaultCameraBoomSocketOffsetGround;
		GetCharacterMovement()->GravityScale = DefaultGravityScale;
	}

	//Changes made on any movement mode change
	LowAngleCameraFlag = !LowAngleCameraFlag;
}

void ASuperIntrospectCharacter::TurnCameraLeftRight(float Val)
{
	// Turn the camera using default unreal function
	AddControllerYawInput(Val * 1);

	// Work out the target tilting rotation
	//TargetTiltingRotation = TargetTiltingRotation + ((Val * 10.0f) - TargetTiltingRotation) * 0.1f;
	//TargetTiltingRotation = FMath::Clamp(TargetTiltingRotation, -50.0f, 50.0f);
}

void ASuperIntrospectCharacter::TurnCameraUpDown(float Val)
{
	// Turn the camera using default unreal function
	AddControllerPitchInput(Val * 1);
}

void ASuperIntrospectCharacter::TurnAtRate(float Rate)
{
	// calculate delta for this frame from the rate information
	AddControllerYawInput(Rate * 1 * 1 * GetWorld()->GetDeltaSeconds());
}

void ASuperIntrospectCharacter::LookUpAtRate(float Rate)
{
	// calculate delta for this frame from the rate information
	AddControllerPitchInput(Rate * 1 * 1 * GetWorld()->GetDeltaSeconds());
}

void ASuperIntrospectCharacter::MoveForward(float Value)
{
	if ((Controller != NULL) && (Value != 0.0f))
	{
		// find out which way is forward
		const FRotator Rotation = Controller->GetControlRotation();
		const FRotator YawRotation(0, Rotation.Yaw, 0);

		// get forward vector
		const FVector Direction = FRotationMatrix(YawRotation).GetUnitAxis(EAxis::X);

		// If sliding, decrease input vector value so that turning is slowed.
		if (SlidingFlag == true)
		{
			AddMovementInput(Direction, Value * SlideTurnRate);
		}
		// Otherwise, move at normal rates.
		else
		{
			AddMovementInput(Direction, Value);
		}

	}
}

void ASuperIntrospectCharacter::MoveRight(float Value)
{
	//Check if should be moving on the ground
	if ((Controller != NULL) && (Value != 0.0f))
	{
		// find out which way is right
		const FRotator Rotation = Controller->GetControlRotation();
		const FRotator YawRotation(0, Rotation.Yaw, 0);

		// get right vector 
		const FVector Direction = FRotationMatrix(YawRotation).GetUnitAxis(EAxis::Y);

		// If sliding, decrease input vector value so that turning is slowed.
		if (SlidingFlag == true)
		{
			AddMovementInput(Direction, Value * SlideTurnRate);
		}
		// Otherwise, move at normal rates.
		else
		{
			AddMovementInput(Direction, Value);
		}
	}
}

void ASuperIntrospectCharacter::Jump()
{
	//Check if the player is on the ground
	if (GetCharacterMovement()->MovementMode == EMovementMode::MOVE_Walking)
	{
		//Check if sliding for super jump
		if (SlidingFlag == true)
		{
			SuperJump();
		}
		//Otherwise perform normal jump
		else
		{
			GetCharacterMovement()->DoJump(bClientUpdating);
		}
	}
	//Otherwise check if the player can double jump
	else if (!DoubleJumpFlag)
	{
		DoubleJump();
	}
	JumpKeyHeld = true;
}

void ASuperIntrospectCharacter::JumpStop()
{
	JumpKeyHeld = false;
}

void ASuperIntrospectCharacter::DoubleJump()
{
	if (GetCharacterMovement()->Velocity.Z > 0.0f)
	{
		GetCharacterMovement()->JumpZVelocity += GetCharacterMovement()->Velocity.Z * 1.2f;
	}
	GetCharacterMovement()->DoJump(bClientUpdating);
	GetCharacterMovement()->JumpZVelocity = DefaultJumpSpeed;
	DoubleJumpFlag = true;
}

void ASuperIntrospectCharacter::SuperJump()
{
	SlidingFlag = false;
	GetCharacterMovement()->JumpZVelocity *= 3.0f;
	GetCharacterMovement()->DoJump(bClientUpdating);
	GetCharacterMovement()->JumpZVelocity = DefaultJumpSpeed;
	DoubleJumpFlag = true;
	GetCharacterMovement()->GravityScale = SuperJumpGravity;
}

void ASuperIntrospectCharacter::HandleLedgeClimbing()
{
	// Check if the player is in the air, is holding the jump key, and ledge climb not on cooldown
	if (GetCharacterMovement()->MovementMode == EMovementMode::MOVE_Falling && JumpKeyHeld && LedgeClimbCooldownCurrent <= 0.0f)
	{
		// Get start point
		FVector TraceStartPosition = GetActorLocation();
		//TraceStartPosition.Z += 40.0f;

		// Get end point
		FVector TraceEndPosition = TraceStartPosition + GetActorForwardVector() * 150.0f;

		FName TraceTag("MyTraceTag");
		GetWorld()->DebugDrawTraceTag = TraceTag;

		// Set sphere trace parameters
		FHitResult hit;
		FCollisionObjectQueryParams ObjectList;
		ObjectList.AddObjectTypesToQuery(ECC_WorldStatic);
		ObjectList.AddObjectTypesToQuery(ECC_Pawn);
		ObjectList.AddObjectTypesToQuery(ECC_Destructible);
		FCollisionQueryParams TraceParams = FCollisionQueryParams(FName(TEXT("TraceParams")), false, this);
		//TraceParams.TraceTag = TraceTag;

		// Perform first sphere trace to check for a surface to climb
		if (GetWorld()->SweepSingleByObjectType(hit, TraceStartPosition, TraceEndPosition, FQuat::Identity, ObjectList, FCollisionShape::MakeSphere(15.0f), TraceParams))
		{
			// If a hit has been detected, do another box trace to check if this is a ledge or a blocking object
			if (hit.bBlockingHit)
			{
				TraceStartPosition = GetActorLocation();
				TraceStartPosition.Z += 155.0f;
				TraceEndPosition = TraceStartPosition + GetActorForwardVector() * 450.0f;

				// If no hit has been detected, then do a ledge climb
				if (!GetWorld()->SweepSingleByObjectType(hit, TraceStartPosition, TraceEndPosition, FQuat::Identity, ObjectList, FCollisionShape::MakeBox(FVector(75.0f, 75.0f, 20.0f)), TraceParams))
				{
					// Launch the character upwards
					LaunchCharacter(GetLedgeClimbHeight(), true, true);
					LedgeClimbCooldownCurrent = LedgeClimbCooldownMax;
					LedgeClimbForwardLaunchFlag = true;
					//TODO Play sfx for climbing and start thrusters


				}
			}
		}
	}
}

FVector ASuperIntrospectCharacter::GetLedgeClimbHeight()
{
	// Initialise querying variables
	float RaycastHeight = 115.0f; // The height above actor z to test for a ledge
	float LaunchVelocity = LedgeClimbZLaunchVelocity; // The velocity to launch the player upwards

	// Get start point
	FVector TraceStartPosition = GetActorLocation();
	TraceStartPosition.Z += RaycastHeight;

	// Get end point
	FVector TraceEndPosition = TraceStartPosition + GetActorForwardVector() * 300.0f;

	// Used for debug display
	FName TraceTag("MyTraceTag");
	GetWorld()->DebugDrawTraceTag = TraceTag;

	// Set line trace parameters
	FHitResult hit;
	FCollisionObjectQueryParams ObjectList;
	ObjectList.AddObjectTypesToQuery(ECC_WorldStatic);
	ObjectList.AddObjectTypesToQuery(ECC_Pawn);
	ObjectList.AddObjectTypesToQuery(ECC_Destructible);
	FCollisionQueryParams TraceParams = FCollisionQueryParams(FName(TEXT("TraceParams")), false, this);
	//TraceParams.TraceTag = TraceTag;

	// Check 3 specific heights that the ledge could be at

	for (int i = 0; i < 2; i++)
	{
		if (GetWorld()->SweepSingleByObjectType(hit, TraceStartPosition, TraceEndPosition, FQuat::Identity, ObjectList, FCollisionShape::MakeSphere(15.0f), TraceParams))
		{
			return FVector(0.0f, 0.0f, LaunchVelocity);
		}
		else
		{
			RaycastHeight -= 40.0f;
			LaunchVelocity *= 0.9;

			TraceStartPosition = GetActorLocation();
			TraceStartPosition.Z += RaycastHeight;
			TraceEndPosition = TraceStartPosition + GetActorForwardVector() * 300.0f;
		}
	}
	return FVector(0.0f, 0.0f, LaunchVelocity);
}

void ASuperIntrospectCharacter::Slide()
{
	SlidingFlag = true;
	GetCharacterMovement()->MaxWalkSpeed = SlidingSpeed;
}

void ASuperIntrospectCharacter::SlideStop()
{
	SlidingFlag = false;
	GetCharacterMovement()->MaxWalkSpeed = DefaultMoveSpeed;
}

void ASuperIntrospectCharacter::HandleSlide()
{
	if (SlidingFlag == true)
	{
		GetMovementComponent()->AddInputVector(GetActorForwardVector());
	}
}

void ASuperIntrospectCharacter::HandleGrapplingHook()
{
	// Decrement the grapple hook duration to see if it should be turned off
	HandleGrapplingHookDuration();
	// Check if the hook has been fired
	if (GrappleHookFired)
	{
		// Check if the hook has hit a target
		if (GrappleHookHit)
		{
			TurnPlayerToGrappleDirection();
			SetGrapplingHookEndLocation();
			// Check if the hook is far enough from player
			if ((GetGrapplingHookEndLocation() - GetActorLocation()).Size() >= 100.0f)
			{
				GrappleHookHanging = false;
				// Do Smart Grapple Reeling

			}
			// If it is too close, then toggle grapple hook hanging on
			else
			{
				GetCharacterMovement()->Velocity *= 0.0f;
				GrappleHookHanging = true;
			}
		}
		// Otherwise move hook forward and check for collisions
		else
		{
			// If hook has hit auto aim target
			if (CheckHookCollision())
			{
				// Set the hook hit location and parameters
				SetHookHit();
			}
			// If it hasn't
			else
			{
				// Move the grapple hook forward
				GrappleHookLocation += GrappleHookDirection * (GrappleHookTravelSpeed * CharacterDeltaTime);

				//Check if the hook has reached max length
				if ((GrappleHookLocation - GetActorLocation()).Size() >= GrappleHookMaxLengthCurrent)
				{
					// Disable the hook if max length is reached
					ToggleHookOff();
				}
				else
				{
					// Otherwise set the hook location
					GetGrapplingHookEndLocation();
				}
			}
		}
	}
	// Otherwise just set the hook location to actor location
	else
	{
		GrappleHookLocation = GetActorLocation();
	}
}

void ASuperIntrospectCharacter::HandleGrapplingHookDuration()
{
	// If am grapple hanging, increase time grapple can be out
	if (GrappleHookHanging)
	{
		GrappleHookDurationCurrent -= CharacterDeltaTime / 2.0f;
	}
	else
	{
		GrappleHookDurationCurrent -= CharacterDeltaTime;
	}
	if (GrappleHookDurationCurrent <= 0.0f)
	{
		ToggleHookOff();
	}
}

void ASuperIntrospectCharacter::ToggleHookOff()
{
	GrappleHookFired = false;
	if (GrappleHookHit)
	{
		GrappleHookDirection = (GetGrapplingHookEndLocation() - GetActorLocation()).GetSafeNormal();
		GrappleHookHit = false;
	}
	// REVISIT THESE NUMBERS
	GetCharacterMovement()->GravityScale = 4.0f;
	GrappleHookHanging = false;
	GetCharacterMovement()->bOrientRotationToMovement = true;
	GetCharacterMovement()->FallingLateralFriction = 0.16f;
	GrappleHookAimAssistFlag = false;
}

FVector ASuperIntrospectCharacter::GetGrapplingHookEndLocation()
{
	// Make sure the grapple has hit a component
	if (GrappleHookHitComponent)
	{
		// Check if hit component has a skeleton
		USkeletalMeshComponent* HitComponentSkeleton = Cast<USkeletalMeshComponent>(GrappleHookHitComponent);
		// If it does, use its hit socket location to calculate
		if (HitComponentSkeleton)
		{
			return (HitComponentSkeleton->GetSocketLocation(GrappleHookHitSocketName) - GrappleHookLocation);
		}
		// If it doesn't, just use actor location to calculate
		else
		{
			return (GrappleHookHitComponent->GetComponentLocation() - GrappleHookLocation);
		}
	}
	// If it hasn't, return an empty vector
	return FVector();
}

void ASuperIntrospectCharacter::SetGrapplingHookEndLocation()
{
	FVector TargetEndLocation;
	if (GrappleHookHit)
	{
		TargetEndLocation = GetActorRotation().UnrotateVector(GetGrapplingHookEndLocation() - GetActorLocation());
		
	}
	else
	{
		TargetEndLocation = GetActorRotation().UnrotateVector(GrappleHookLocation - GetActorLocation());
	}
	GrapplingHook->EndLocation = FMath::VInterpTo(GrapplingHook->EndLocation, TargetEndLocation, CharacterDeltaTime, 400.0f);
}

bool ASuperIntrospectCharacter::CheckHookCollision()
{
	// *Assume all grapple hook collisions are from aim assist
	FVector HitLocation;

	FVector PredictedGrappleLocation = GrappleHookLocation + GrappleHookDirection * GrappleHookTravelSpeed * CharacterDeltaTime;
	// This variable determines the distance between the hook location and target that can be auto hit
	float AutoHitThreshold = FMath::Clamp(GrapplingHook->CableLength / 5.0f, 1000.0f, 5000.0f);

	if ((PredictedGrappleLocation - GrappleHookAimAssistLocation).Size() <= AutoHitThreshold)
	{
		HitLocation = GrappleHookAimAssistLocation;
		//Play grapple hook hit sound
		//Start playing grapple hook swinging sfx
		GrappleHookHitComponent = GrappleHookAimAssistComponent;
		if (Cast<USkeletalMeshComponent>(GrappleHookHitComponent))
		{
			GrappleHookHitSocketName = GrappleHookAimAssistSocketName;
			GrappleHookLocation = Cast<USkeletalMeshComponent>(GrappleHookHitComponent)->GetSocketLocation(GrappleHookAimAssistSocketName) - HitLocation;
		}
		else
		{
			GrappleHookLocation = GrappleHookHitComponent->GetComponentLocation() - HitLocation;
		}
		//Set high velocity flag to false
		//Spawn grapple hook hit particle at hit location
		return true;
	}
	return false;
}

void ASuperIntrospectCharacter::SetHookHit()
{
	GrappleHookHit = true;
	GetCharacterMovement()->GravityScale = 0.0f;

}

void ASuperIntrospectCharacter::TurnPlayerToGrappleDirection()
{
	// Get the target direction that the player should be
	FVector GrappleDirection = (GetGrapplingHookEndLocation() - GetActorLocation()).Rotation().Vector();

	FRotator NewRotation = FMath::VInterpTo(GetActorRotation().Vector(), GrappleDirection, CharacterDeltaTime, 5.0f).Rotation();
	SetActorRotation(NewRotation);
}

void ASuperIntrospectCharacter::HandleCameraBoomSocketOffsetInterp()
{
	if (CameraBoom->SocketOffset != DesiredCameraBoomSocketOffset)
	{
		CameraBoom->SocketOffset = FMath::VInterpTo(CameraBoom->SocketOffset, DesiredCameraBoomSocketOffset, CharacterDeltaTime, 3.0f);
	}
}

void ASuperIntrospectCharacter::HandleLowAngleCamera()
{
	// Start panning the camera to the right if it is low enough
	if (CameraBoom->RelativeRotation.Pitch > 32.0f)
	{
		// The camera is panned based on how low it is.
		DesiredCameraBoomSocketOffset.Y = (CameraBoom->RelativeRotation.Pitch - 32.0f) * 3.2f;
		if (CameraBoom->CameraLagSpeed != 50.0f)
		{
			CameraBoom->CameraLagSpeed = 50.0f;
		}
	}
	// Reset low angle camera values 
	else if (CameraBoom->RelativeRotation.Pitch <= 32.0f && DesiredCameraBoomSocketOffset.Y != 0.0f)
	{
		DesiredCameraBoomSocketOffset.Y = 0.0f;
		CameraBoom->CameraLagSpeed = DefaultCameraLagSpeed;
	}
}

void ASuperIntrospectCharacter::HandleTimers()
{
	// Check if should countdown ledge climb cooldown
	if (LedgeClimbCooldownCurrent > 0.0f)
	{
		LedgeClimbCooldownCurrent -= CharacterDeltaTime;
		// If ledge climb has half finished cooling down, launch the character forwards to finish ledge climbing
		if (LedgeClimbCooldownCurrent <= LedgeClimbCooldownMax / 2.5f && LedgeClimbForwardLaunchFlag)
		{
			LaunchCharacter(GetActorForwardVector() * LedgeClimbXLaunchVelocity + GetCharacterMovement()->Velocity * 0.1f, true, true);
			LedgeClimbForwardLaunchFlag = false;
		}
	}
}
