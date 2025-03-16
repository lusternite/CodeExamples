// Fill out your copyright notice in the Description page of Project Settings.

#include "Introspect.h"
#include "WeaponBase.h"
#include "IntrospectCharacter.h"
#include "GameEntities/Weapons/CanTakeDamage.h"
#include "Kismet/KismetMathLibrary.h"
#include "Engine.h"

AWeaponBase::AWeaponBase()
{
	PrimaryActorTick.bCanEverTick = true;

	SpringArm = CreateDefaultSubobject<USpringArmComponent>(TEXT("Spring Arm"));
	RootComponent = SpringArm;

	WeaponBody = CreateDefaultSubobject<USkeletalMeshComponent>(TEXT("Weapon Body"));
	WeaponBody->SetupAttachment(SpringArm);
	WeaponBody->SetMaterial(0, Material);
}

void AWeaponBase::BeginPlay()
{
	Super::BeginPlay();

	TargetHoverYPos = 7.5f;
	TargetHoverZPos = 12.0f;
	HoverSpeedY = 2.5f;
	HoverSpeedZ = 1.5f;
	StartingHoverYPos = -7.5f;
	StartingHoverZPos = -12.0f;
}

void AWeaponBase::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

	// If the weapon is active
	if (bIsActive)
	{
		// Handle all aspects
		HandleFiring(DeltaTime);
		HandleMovement(DeltaTime);
		HandleTransform(DeltaTime);
	}
	else
	{
		SubtractOverheat(fOverheatCoolAmount * DeltaTime);
	}
}

void AWeaponBase::Destroyed()
{
	for (TObjectIterator<ANewProjectileBase> it; it; ++it)
	{
		it->Destroy();
	}

	for (TQueue<ANewProjectileBase*>* it : Queues)
	{
		it = nullptr;
	}
	Queues.Empty();
}

void AWeaponBase::HandleFiring(float DeltaTime)
{
	fTimeSincePrimaryFire += DeltaTime;
	fTimeSinceSecondaryFire += DeltaTime;

	if (!bIsOverheated)
	{
		if (bIsPrimaryFiring)
		{
			if (fTimeSincePrimaryFire >= (1.0f / fPrimaryFireRate))
			{
				FirePrimary();

				fTimeSincePrimaryFire = 0.0f;
				return;
			}
			else
			{
				SubtractOverheat(fOverheatCoolAmount * DeltaTime);
			}
		}
		else if (bIsSecondaryFiring)
		{
			if (fTimeSinceSecondaryFire >= (1.0f / fSecondaryFireRate))
			{
				FireSecondary();

				fTimeSinceSecondaryFire = 0.0f;
				return;
			}
			else
			{
				SubtractOverheat(fOverheatCoolAmount * DeltaTime);
			}
		}
		else
		{
			SubtractOverheat(fOverheatCoolAmount * DeltaTime);
		}
	}
	else
	{
		SubtractOverheat(fOverheatCoolAmount * DeltaTime);
	}
}

void AWeaponBase::HandleTransform(float DeltaTime)
{
	// Find new location 
	FVector NewLocation = GetOwner()->GetActorLocation();
	NewLocation.Z += 100.0f;

	// Find new rotation by lerping
	FVector MeshToScreenCenter = GetCenterScreenLocation() - GetActorLocation();
	MeshToScreenCenter.Normalize();
	FRotator TargetRotation = UKismetMathLibrary::Conv_VectorToRotator(MeshToScreenCenter);
	FRotator NewRotation = FMath::RInterpTo(GetActorRotation(), TargetRotation, DeltaTime, 10.0f);

	// Set the rotation and location of the actor
	SetActorLocationAndRotation(NewLocation, NewRotation);
}

void AWeaponBase::HandleMovement(float DeltaTime)
{
	CurrentHoverYPos = ParabolicInterp(CurrentHoverYPos, TargetHoverYPos, StartingHoverYPos);
	if (FMath::IsNearlyEqual(CurrentHoverYPos, TargetHoverYPos, 0.4f))
	{
		StartingHoverYPos = TargetHoverYPos;
		TargetHoverYPos = TargetHoverYPos * -1.0f;
	}

	CurrentHoverZPos = ParabolicInterp(CurrentHoverZPos, TargetHoverZPos, StartingHoverZPos);
	if (FMath::IsNearlyEqual(CurrentHoverZPos, TargetHoverZPos, 0.2f))
	{
		StartingHoverZPos = TargetHoverZPos;
		TargetHoverZPos = TargetHoverZPos * -1.0f;
	}

	WeaponBody->SetRelativeLocation(FVector(0.0f, CurrentHoverYPos, CurrentHoverZPos));
}

void AWeaponBase::OnHitActor(AActor * actor, FHitScanFireParamaters params, FHitResult hit)
{
	EHitType HitType = Cast<ICanTakeDamage>(actor)->Execute_ObjectTakeDamage(actor, params.Damage, 1.0f, hit, params.SourceName);
	HitShot(HitMarkerFromHitType(HitType));
	SpawnDamageNumberMarker(hit.ImpactPoint, params.Damage, HitType);
}

void AWeaponBase::OnHitComponent(UPrimitiveComponent * component, FHitScanFireParamaters params, FHitResult hit)
{
	EHitType HitType = Cast<ICanTakeDamage>(component)->Execute_ObjectTakeDamage(component, params.Damage, 1.0f, hit, params.SourceName);
	HitShot(HitMarkerFromHitType(HitType));
	SpawnDamageNumberMarker(hit.ImpactPoint, params.Damage, HitType);
}

bool AWeaponBase::IsUpgradeRandomProc()
{
	float rand = FMath::RandRange(0.0f, 1.0f); 
	if (rand < fUpgradeProcChance)
	{
		return true;
	}
	return false;
}

ANewProjectileBase* AWeaponBase::FireBullet(FBulletFireParamaters params, bool counts)
{
	// Get fire direction
	FVector FireDirection = GetCenterScreenLocation() - WeaponBody->GetSocketLocation(sBarrelSocketName);
	FireDirection.Normalize();

	FireDirection += params.LaunchOffset;
	FireDirection += GenerateSprayVector(params.SprayAmount);

	// Get the position to start from
	FVector FireFromPosition = WeaponBody->GetSocketLocation(sBarrelSocketName);

	// Get a new bullet from the bullet pool
	ANewProjectileBase* bullet = GetBullet(params.BulletToSpawn, params.PoolIndex);
	
	// Check to make sure the bullet exists, then fire it
	if (bullet)
	{
		bullet->Initialize(FireFromPosition, FireDirection, params.LifeTime, TypesToIgnore, (Queues.Num() > params.PoolIndex) ? Queues[params.PoolIndex] : nullptr);
		if (counts) {
			TookShot();
		}
		return bullet;
	}

	return nullptr;
}

FHitResult AWeaponBase::PerformHitScan(FHitScanFireParamaters params)
{
	// Get start point
	FVector FireFromPosition = Cast<AIntrospectCharacter>(GetOwner())->GetFollowCamera()->GetComponentLocation();

	// Get end point
	FRotator RotationXVector = GetWorld()->GetFirstPlayerController()->PlayerCameraManager->GetCameraRotation();
	FVector RotationVector = UKismetMathLibrary::Conv_RotatorToVector(RotationXVector);
	FVector EndPoint = FireFromPosition + (RotationVector * params.TraceDistance);

	TookShot();

	return HitScanLineTrace(FireFromPosition, EndPoint, params);
}

FHitResult AWeaponBase::HitScanLineTrace(FVector start, FVector end, FHitScanFireParamaters params)
{
	// Set line trace parameters
	TArray<FHitResult> hits;
	FCollisionObjectQueryParams ObjectList;
	ObjectList.AddObjectTypesToQuery(ECC_WorldStatic);
	ObjectList.AddObjectTypesToQuery(ECC_WorldDynamic);
	FCollisionQueryParams TraceParams = FCollisionQueryParams(FName(TEXT("TraceParams")), false, this);

	// Perform the line trace using the start and end vectors
	if (GetWorld()->LineTraceMultiByObjectType(hits, start, end, ObjectList, TraceParams))
	{
		for (FHitResult hit : hits)
		{
			// Check for ignored actors
			bool b = true;
			for (TSubclassOf<AActor> a : TypesToIgnore)
			{
				if (hit.GetActor()->GetClass() == a)
				{
					b = false;
					break;
				}
			}

			// If we didn't hit an ignored actor, proceed
			if (b)
			{
				ICanTakeDamage* component = Cast<ICanTakeDamage>(hit.GetComponent());
				ICanTakeDamage* actor = Cast<ICanTakeDamage>(hit.GetActor());

				// Test to see if we hit a boss
				if (component)
				{
					OnHitComponent(hit.GetComponent(), params, hit);
					GEngine->AddOnScreenDebugMessage(-1, 2.0f, FColor::Purple, "Hit Comp");
				}
				else if (actor)
				{
					OnHitActor(hit.GetActor(), params, hit);
					GEngine->AddOnScreenDebugMessage(-1, 2.0f, FColor::Purple, "Hit Actor");
				}

				// Spawn Decal
				SpawnDecal(hit, params.HitDecal);

				return hit;
			}
		}
	}
	return FHitResult();
}

void AWeaponBase::FirePrimary()
{
	AddOverheat(fPrimaryOverheatAmount);
	ReticleBounce(true);
}

void AWeaponBase::FireSecondary()
{
	AddOverheat(fSecondaryOverheatAmount);
}

FVector AWeaponBase::GenerateSprayVector(float SprayAmount)
{
	FVector v1 = WeaponBody->GetRightVector().GetSafeNormal() * FMath::RandRange(-SprayAmount, SprayAmount);
	FVector v2 = WeaponBody->GetUpVector().GetSafeNormal() * FMath::RandRange(-SprayAmount, SprayAmount);
	return v1 + v2;
}

ANewProjectileBase* AWeaponBase::GetBullet(TSubclassOf<class ANewProjectileBase> type, int PoolIndex)
{
	ANewProjectileBase* bullet;

	// Prevent crash trying to access array
	if (PoolIndex < Queues.Num())
	{
		// If queue is not empty
		if (!Queues[PoolIndex]->IsEmpty())
		{
			// Pop and return the first item of the queue
			if (Queues[PoolIndex]->Dequeue(bullet))
			{
				return bullet;
			}
		}
	}

	// If nothing in queue, spawn new bullet
	FActorSpawnParameters SpawnInfo;
	SpawnInfo.SpawnCollisionHandlingOverride = ESpawnActorCollisionHandlingMethod::AlwaysSpawn;
	SpawnInfo.Owner = this;
	bullet = GetWorld()->SpawnActor<ANewProjectileBase>(type, FVector(0.0f, 0.0f, -10000.0f), FRotator::ZeroRotator, SpawnInfo);

	return bullet;
}

FVector AWeaponBase::GetCenterScreenLocation()
{
	FRotator RotationXVector = GetWorld()->GetFirstPlayerController()->PlayerCameraManager->GetCameraRotation();
	FVector RotationVector = UKismetMathLibrary::Conv_RotatorToVector(RotationXVector);
	FVector FollowCameraLocation = Cast<AIntrospectCharacter>(GetOwner())->GetFollowCamera()->GetComponentLocation();

	FVector TraceStart = FollowCameraLocation + (RotationVector * 10.f);
	FVector TraceEnd = FollowCameraLocation + (RotationVector * 4500.0f);

	// Set line trace parameters
	FHitResult hit;
	FCollisionObjectQueryParams ObjectList;
	ObjectList.AddObjectTypesToQuery(ECC_WorldStatic);
	ObjectList.AddObjectTypesToQuery(ECC_WorldDynamic);
	FCollisionQueryParams TraceParams = FCollisionQueryParams(FName(TEXT("TraceParams")), false, this);

	// Perform the line trace using the start and end vectors
	if (GetWorld()->LineTraceSingleByObjectType(hit, TraceStart, TraceEnd, ObjectList, TraceParams))
	{
		return hit.ImpactPoint;
	}
	return TraceEnd;
}

void AWeaponBase::ApplyDeathPhysics()
{
	WeaponBody->SetSimulatePhysics(true);
	WeaponBody->SetCollisionEnabled(ECollisionEnabled::QueryAndPhysics);
}

float AWeaponBase::ParabolicInterp(float CurrentValue, float TargetValue, float StartingValue)
{
	float MidPoint = (TargetValue + StartingValue) / 2.0f;
	if (MidPoint < TargetValue)
	{
		if (CurrentValue >= MidPoint)
		{
			return FMath::FInterpTo(CurrentValue, TargetValue, 0.033f, 3.0f);
		}
		else
		{
			return InterpToOppositeTarget(CurrentValue, StartingValue);
		}
	}
	else
	{
		if (CurrentValue <= MidPoint)
		{
			return FMath::FInterpTo(CurrentValue, TargetValue, 0.033f, 3.0f);
		}
		else
		{
			return InterpToOppositeTarget(CurrentValue, StartingValue);
		}
	}
}

float AWeaponBase::InterpToOppositeTarget(float Current, float Opposite)
{
	float Target = Opposite + (FMath::Sign(Opposite) * -0.01f);
	return (Current - FMath::FInterpTo(Current, Target, 0.033, 3)) + Current;
}

void AWeaponBase::AddOverheat(float Value)
{
	fCurrentOverheat += Value;
	fCurrentOverheat = FMath::Clamp(fCurrentOverheat, 0.0f, 1.0f);
	if (fCurrentOverheat == 1.0f)
	{
		OnOverheatBegin();
	}
}

void AWeaponBase::SubtractOverheat(float Value)
{
	fCurrentOverheat -= Value;
	fCurrentOverheat = FMath::Clamp(fCurrentOverheat, 0.0f, 1.0f);
	if (fCurrentOverheat == 0.0f)
	{
		OnOverheatEnd();
	}
}

void AWeaponBase::OnOverheatBegin()
{
	bIsOverheated = true;
	bIsPrimaryFiring = false;
	bIsSecondaryFiring = false;
	ToggleOverheatVFX();
}

void AWeaponBase::OnOverheatEnd()
{
	if (bIsOverheated)
	{
		bIsOverheated = false;

		if (bIsPrimaryQueued)
		{
			bIsPrimaryFiring = true;
		}
		else if (bIsSecondaryQueued)
		{
			bIsSecondaryFiring = true;
		}
		ToggleOverheatVFX();
	}
}

void AWeaponBase::SetPrimaryIsFiring(bool b)
{
	if (b)
	{
		if (!bIsOverheated)
		{
			bIsPrimaryFiring = true;
			bIsSecondaryFiring = false;
		}

		bIsPrimaryQueued = true;
	}
	else
	{
		bIsPrimaryFiring = false;
		bIsPrimaryQueued = false;

		if (bIsSecondaryQueued)
		{
			bIsSecondaryFiring = true;
		}
	}
}

void AWeaponBase::SetSecondaryIsFiring(bool b)
{
	if (b)
	{
		if (!bIsOverheated)
		{
			bIsSecondaryFiring = true;
			bIsPrimaryFiring = false;
		}

		bIsSecondaryQueued = true;
	}
	else
	{
		bIsSecondaryFiring = false;
		bIsSecondaryQueued = false;

		if (bIsPrimaryQueued)
		{
			bIsPrimaryFiring = true;
		}
	}
}

void AWeaponBase::Equip()
{
	// Activate
	bIsActive = true;

	// Set position
	SetActorLocation(GetOwner()->GetActorLocation());
	SpringArm->bEnableCameraLag = true;

	// Detach from component
	FDetachmentTransformRules rules(EDetachmentRule::KeepRelative, EDetachmentRule::KeepRelative, EDetachmentRule::KeepRelative, false);
	RootComponent->DetachFromComponent(rules);
}

void AWeaponBase::Unequip()
{
	// Deactivate
	bIsActive = false;

	// Set Position
	SetActorLocation(FVector::ZeroVector);
	SpringArm->bEnableCameraLag = false;

	bIsPrimaryFiring = false;
	bIsSecondaryFiring = false;
	bIsPrimaryQueued = false;
	bIsSecondaryQueued = false;
}

void AWeaponBase::Upgrade(EUpgradeType UpgradeType)
{
	if (!HasUpgrade(UpgradeType))
	{
		Upgrades.Add(UpgradeType);
	}
}

bool AWeaponBase::HasUpgrade(EUpgradeType UpgradeType)
{
	return Upgrades.Contains(UpgradeType);
}

UTexture2D * AWeaponBase::HitMarkerFromHitType(EHitType HitType)
{
	switch (HitType)
	{
	case EHitType::ERegular:
	{
		UGameplayStatics::PlaySound2D(GetWorld(), HitSoundRegular);
		return HitMarkerRegular;
	}
	case EHitType::ECritical:
	{
		UGameplayStatics::PlaySound2D(GetWorld(), HitSoundCritical);
		return HitMarkerCritical;
	}
	case EHitType::EArmour:
	{
		UGameplayStatics::PlaySound2D(GetWorld(), HitSoundArmour);
		return HitMarkerArmour;
	}
	case EHitType::EMiss:
	{
		return nullptr;
	}
	default:
	{
		return nullptr;
	}
	}
}

void AWeaponBase::SpawnDecal(FHitResult Hit, TSubclassOf<ADecalActor> HitDecal)
{
	ADecalActor* decal = GetWorld()->SpawnActor<ADecalActor>(HitDecal, Hit.ImpactPoint, Hit.Normal.Rotation());
	if (decal)
	{
		decal->GetDecal()->AddRelativeRotation(FRotator(90.0f, 0.0f, 0.0f));
		decal->GetDecal()->AttachToComponent(Hit.GetComponent(), FAttachmentTransformRules(EAttachmentRule::KeepWorld, EAttachmentRule::KeepWorld, EAttachmentRule::KeepWorld, true), Hit.BoneName);
	}
}

TMap<ICanTakeDamage*, UObject*> AWeaponBase::GetDamageableObjectsInRange(UPrimitiveComponent* range)
{
	TMap<ICanTakeDamage*, UObject*> all;

	TSet<AActor*> a;
	range->GetOverlappingActors(a);
	TArray<UPrimitiveComponent*> b;
	range->GetOverlappingComponents(b);

	for (AActor* aa : a)
	{
		ICanTakeDamage* aaa = Cast<ICanTakeDamage>(aa);
		if (aaa)
		{
			all.Add(aaa, aa);
		}
	}

	for (UPrimitiveComponent* bb : b)
	{
		ICanTakeDamage* bbb = Cast<ICanTakeDamage>(bb);
		if (bbb)
		{
			all.Add(bbb, bb);
		}
	}

	return all;
}