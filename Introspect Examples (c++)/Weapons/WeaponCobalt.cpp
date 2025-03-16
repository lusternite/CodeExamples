// Fill out your copyright notice in the Description page of Project Settings.

#include "Introspect.h"
#include "Engine.h"
#include "Kismet/KismetMathLibrary.h"
#include "GameEntities/Projectiles/NewBullets/CobaltHomingBullet.h"
#include "WeaponCobalt.h"
#include "GameEntities/Weapons/CobaltAnimInstance.h"

AWeaponCobalt::AWeaponCobalt() 
	: Super()
{
	Lazer = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("LazerMesh"));
	Lazer->SetupAttachment(WeaponBody);
	
	MuzzleFlash = CreateDefaultSubobject<UParticleSystemComponent>(TEXT("MuzzleFlash"));
	MuzzleFlash->SetupAttachment(WeaponBody);

	LazerCharge = CreateDefaultSubobject<UParticleSystemComponent>(TEXT("LazerCharge"));
	LazerCharge->SetupAttachment(WeaponBody);

	ChargingSound = CreateDefaultSubobject<UAudioComponent>(TEXT("ChargingSound"));
	ChargingSound->SetupAttachment(WeaponBody);

	LazerSound = CreateDefaultSubobject<UAudioComponent>(TEXT("LazerSound"));
	LazerSound->SetupAttachment(WeaponBody);
}

void AWeaponCobalt::SetSecondaryIsFiring(bool b)
{
	if (!bLazerFiring)
	{
		Super::SetSecondaryIsFiring(b);
		Cast<UCobaltAnimInstance>(WeaponBody->GetAnimInstance())->bShouldChargeSecondary = b;

		if (b)
		{
			if (!bIsOverheated)
			{
				LazerCharge->Activate();
				ChargingSound->Play();
				OverheatOnBeginCharge = fCurrentOverheat;
			}
		}
		else
		{
			if (fCurrentOverheat > 0.1f)
			{
				StartLazer();
			}
			LazerCharge->Deactivate();
			ChargingSound->Stop();
		}
	}
	else
	{
		bIsSecondaryQueued = b;
	}
}

void AWeaponCobalt::SetPrimaryIsFiring(bool b)
{
	if (!bIsSecondaryFiring && !bLazerFiring)
	{
		Super::SetPrimaryIsFiring(b);
	}
	else
	{
		bIsPrimaryQueued = b;
	}
}

void AWeaponCobalt::Upgrade(EUpgradeType UpgradeType)
{
	Super::Upgrade(UpgradeType);

	if (UpgradeType == EUpgradeType::EGreen)
	{
		Lazer->SetMaterial(0, PiercingLazerMat);
	}
}

void AWeaponCobalt::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

	if (bLazerFiring)
	{
		CurrentTimeBetweenLazerTick += DeltaTime;
		if (CurrentTimeBetweenLazerTick >= TimeBetweenLazerTicks)
		{
			CurrentTimeBetweenLazerTick = 0.0f;

			if (HasUpgrade(EUpgradeType::EGreen))
			{
				PerformPiercingHitScan();
			}
			else
			{
				FireNormalLazer();
			}

			ReticleBounce(false);
		}

		if (bIsSecondaryFiring)
		{
			SubtractOverheat(fOverheatCoolAmount * DeltaTime);
		}
	}
	else
	{
		if (bIsSecondaryFiring)
		{
			// add overheat and charge
			AddOverheat(DeltaTime * fSecondaryOverheatAmount);
		}
	}
}

void AWeaponCobalt::BeginPlay()
{
	Super::BeginPlay();

	Lazer->SetMaterial(0, LazerMat);
	Lazer->SetVisibility(false, true);
	MuzzleFlash->Deactivate();
	LazerCharge->Deactivate();
}

void AWeaponCobalt::FirePrimary()
{
	if (!bLazerFiring)
	{
		Super::FirePrimary();

		// Particle, sound and animation
		Cast<UCobaltAnimInstance>(WeaponBody->GetAnimInstance())->PlayPrimaryFire();
		UGameplayStatics::SpawnEmitterAtLocation(GetWorld(), PrimaryParams.MuzzleParticle, WeaponBody->GetSocketLocation(sBarrelSocketName), WeaponBody->GetSocketRotation(sBarrelSocketName));
		UGameplayStatics::SpawnEmitterAtLocation(GetWorld(), PrimaryTrail, WeaponBody->GetSocketLocation(sBarrelSocketName), WeaponBody->GetSocketRotation(sBarrelSocketName));
		UGameplayStatics::SpawnSoundAtLocation(GetWorld(), PrimaryParams.SoundOnShoot, WeaponBody->GetSocketLocation(sBarrelSocketName));	

		isPiercing = false;
		PerformHitScan(PrimaryParams);
		
	}
}

void AWeaponCobalt::FireSecondary()
{

}

void AWeaponCobalt::OnOverheatBegin()
{
	if (bIsSecondaryFiring)
	{
		StartLazer();
	}

	Super::OnOverheatBegin();

	if (HasUpgrade(EUpgradeType::ERed))
	{
		ShootTargets();
	}
}

void AWeaponCobalt::Unequip()
{
	Super::Unequip();

	StopLazer();
	LazerCharge->Deactivate();
	ChargingSound->Stop();
}

void AWeaponCobalt::SubtractOverheat(float Value)
{
	if (!bIsSecondaryFiring)
	{
		Super::SubtractOverheat(Value);
	}

	if (fCurrentOverheat <= OverheatOnBeginCharge)
	{
		StopLazer();
	}

	if (fCurrentOverheat == 0.0f && HasUpgrade(EUpgradeType::ERed))
	{
		ClearArray();
	}
}

void AWeaponCobalt::OnHitActor(AActor * actor, FHitScanFireParamaters params, FHitResult hit)
{
	if (!isPiercing)
	{
		Super::OnHitActor(actor, params, hit);
	}
	else 
	{
		EHitType h = Cast<ICanTakeDamage>(actor)->Execute_ObjectTakeDamage(actor, params.Damage, 1.0f, hit, params.SourceName);

		if (Highest == EHitType::EMiss) {
			Highest = h;
		}
		if (Highest == EHitType::ERegular && (h == EHitType::EArmour || h == EHitType::ECritical))
		{
			Highest = h;
		}
		if (Highest == EHitType::EArmour && h == EHitType::ECritical)
		{
			Highest = h;
		}
	}


	if (HasUpgrade(EUpgradeType::EBlue) && IsUpgradeRandomProc() && params == PrimaryParams)
	{
		SpawnHomingBullets(hit);
	}
}

void AWeaponCobalt::OnHitComponent(UPrimitiveComponent * component, FHitScanFireParamaters params, FHitResult hit)
{
	if (!isPiercing)
	{
		Super::OnHitComponent(component, params, hit);
	}
	else
	{
		EHitType h = Cast<ICanTakeDamage>(component)->Execute_ObjectTakeDamage(component, params.Damage, 1.0f, hit, params.SourceName);

		if (Highest == EHitType::EMiss) {
			Highest = h;
		}
		if (Highest == EHitType::ERegular && (h == EHitType::EArmour || h == EHitType::ECritical))
		{
			Highest = h;
		}
		if (Highest == EHitType::EArmour && h == EHitType::ECritical)
		{
			Highest = h;
		}
	}

	if (HasUpgrade(EUpgradeType::ERed))
	{
		AddTargetToArray(component);
	}

	if (HasUpgrade(EUpgradeType::EBlue) && IsUpgradeRandomProc() && params == PrimaryParams)
	{
		SpawnHomingBullets(hit);
	}
}

void AWeaponCobalt::StopLazer()
{
	Cast<UCobaltAnimInstance>(WeaponBody->GetAnimInstance())->bShouldFireSecondary = false;
	bLazerFiring = false;
	Lazer->SetVisibility(false, true);
	MuzzleFlash->Deactivate();
	LazerSound->Stop();

	if (!bIsOverheated)
	{
		if (bIsPrimaryQueued)
		{
			bIsPrimaryFiring = true;
		}
		else if (bIsSecondaryQueued)
		{
			bIsSecondaryFiring = true;
			LazerCharge->Activate();
			ChargingSound->Play();
		}
	}
}

void AWeaponCobalt::StartLazer()
{
	if (!bLazerFiring)
	{
		Cast<UCobaltAnimInstance>(WeaponBody->GetAnimInstance())->bShouldFireSecondary = true;
		bLazerFiring = true;

		Lazer->SetVisibility(true, true);
		MuzzleFlash->Activate();
		LazerSound->Play();

		ChargingSound->Stop();
		LazerCharge->Deactivate();

		isPiercing = false;
		if (HasUpgrade(EUpgradeType::EGreen))
		{
			Lazer->SetWorldScale3D(FVector(PiercingLazerParam.TraceDistance, 70.0f, 70.0f));
			isPiercing = true;
			Highest = EHitType::EMiss;
		}
	}
}

void AWeaponCobalt::PerformPiercingHitScan()
{
	// Get start point
	FVector FireFromPosition = Cast<AIntrospectCharacter>(GetOwner())->GetFollowCamera()->GetComponentLocation();

	// Get end point
	FRotator RotationXVector = GetWorld()->GetFirstPlayerController()->PlayerCameraManager->GetCameraRotation();
	FVector RotationVector = UKismetMathLibrary::Conv_RotatorToVector(RotationXVector);
	FVector EndPoint = FireFromPosition + (RotationVector * PiercingLazerParam.TraceDistance);

	// Set line trace parameters
	TArray<FHitResult> hits;
	FCollisionObjectQueryParams ObjectList;
	ObjectList.AddObjectTypesToQuery(ECC_WorldDynamic);
	ObjectList.AddObjectTypesToQuery(ECC_WorldStatic);
	FCollisionQueryParams TraceParams = FCollisionQueryParams(FName(TEXT("TraceParams")), false, this);

	TookShot();
	bool hit = false;

	// Perform the line trace using the start and end vectors
	if (GetWorld()->LineTraceMultiByObjectType(hits, FireFromPosition, EndPoint, ObjectList, TraceParams))
	{
		for (auto a : hits)
		{
			// Check for ignored actors
			bool b = true;
			for (TSubclassOf<AActor> c : TypesToIgnore)
			{
				if (a.GetActor()->GetClass() == c)
				{
					b = false;
					break;
				}
			}

			if (b)
			{
				ICanTakeDamage* component = Cast<ICanTakeDamage>(a.GetComponent());
				ICanTakeDamage* actor = Cast<ICanTakeDamage>(a.GetActor());

				// Test to see if we hit a boss
				if (component)
				{
					hit = true;
					OnHitComponent(a.GetComponent(), PiercingLazerParam, a);
				}
				else if (actor)
				{
					hit = true;
					OnHitActor(a.GetActor(), PiercingLazerParam, a);
				}
			}
		}
		if (hit)
		{
			HitShot(HitMarkerFromHitType(Highest));
		}
	}
}

void AWeaponCobalt::FireNormalLazer()
{
	FHitResult f = PerformHitScan(SecondaryParams);
	if (f.bBlockingHit)
	{
		Lazer->SetWorldScale3D(FVector(f.Distance * 2.0f, 70.0f, 70.0f));
	}
	else
	{
		Lazer->SetWorldScale3D(FVector(SecondaryParams.TraceDistance, 70.0f, 70.0f));
	}
}

void AWeaponCobalt::SpawnHomingBullets(FHitResult Hit)
{
	if (Hit.GetComponent())
	{
		HomingBulletParams.LaunchOffset = WeaponBody->GetUpVector().GetSafeNormal();
		ANewProjectileBase* bullet1 = FireBullet(HomingBulletParams, false);
		if (bullet1)
		{
			Cast<ACobaltHomingBullet>(bullet1)->SetHomingTarget(Hit.GetComponent());
		}

		HomingBulletParams.LaunchOffset = WeaponBody->GetRightVector().GetSafeNormal();
		ANewProjectileBase* bullet2 = FireBullet(HomingBulletParams, false);
		if (bullet2)
		{
			Cast<ACobaltHomingBullet>(bullet2)->SetHomingTarget(Hit.GetComponent());
		}

		HomingBulletParams.LaunchOffset = (WeaponBody->GetRightVector() * -1).GetSafeNormal();
		ANewProjectileBase* bullet3 = FireBullet(HomingBulletParams, false);
		if (bullet3)
		{
			Cast<ACobaltHomingBullet>(bullet3)->SetHomingTarget(Hit.GetComponent());
		}
	}
}

void AWeaponCobalt::AddTargetToArray(UPrimitiveComponent* Target)
{
	if (OnOverheatTargetedComponents.Num() < MaxTargetedComponents)
	{
		if (Target && !OnOverheatTargetedComponents.Contains(Target))
		{
			OnOverheatTargetedComponents.Add(Target);
		}
	}
}

void AWeaponCobalt::ShootTargets()
{
	// Get start point
	FVector FireFromPosition = WeaponBody->GetComponentToWorld().GetLocation();

	for (UPrimitiveComponent* a : OnOverheatTargetedComponents)
	{
		if (a)
		{
			FVector EndPoint = a->GetComponentToWorld().GetLocation();
			FVector Direction = EndPoint - FireFromPosition;
			FRotator Rotation = Direction.Rotation();

			// Get angle between lookat and fire direction
			FVector f = GetCenterScreenLocation() - FireFromPosition;
			float Angle = FMath::RadiansToDegrees(acosf(FVector::DotProduct(f.GetSafeNormal(), Direction.GetSafeNormal())));
			if (Angle < 70.0f)
			{
				UGameplayStatics::SpawnEmitterAtLocation(GetWorld(), PrimaryParams.MuzzleParticle, WeaponBody->GetSocketLocation(sBarrelSocketName), Rotation);
				UGameplayStatics::SpawnEmitterAtLocation(GetWorld(), PrimaryTrail, WeaponBody->GetSocketLocation(sBarrelSocketName), Rotation);

				HitScanLineTrace(FireFromPosition, EndPoint, PrimaryParams);
				GEngine->AddOnScreenDebugMessage(-1, 2.0f, FColor::Yellow, "Shot");
			}
		}
	}

	UGameplayStatics::SpawnSoundAtLocation(GetWorld(), PrimaryParams.SoundOnShoot, WeaponBody->GetSocketLocation(sBarrelSocketName));
	ClearArray();
}

void AWeaponCobalt::ClearArray()
{
	for (auto a : OnOverheatTargetedComponents)
	{
		a = nullptr;
	}

	OnOverheatTargetedComponents.Empty();
}