// Fill out your copyright notice in the Description page of Project Settings.

#include "Introspect.h"
#include "WeaponBulldog.h"

AWeaponBulldog::AWeaponBulldog() 
	: Super()
{
	Queues.Add(new TQueue<ANewProjectileBase*>);
	Queues.Add(new TQueue<ANewProjectileBase*>);
}

void AWeaponBulldog::SetPrimaryIsFiring(bool b)
{
	Super::SetPrimaryIsFiring(b);

	if (!b)
	{
		fPrimaryFireRate = BaseFireRate;
	}
}

void AWeaponBulldog::BeginPlay()
{
	Super::BeginPlay();

	BaseFireRate = fPrimaryFireRate;
}

float AWeaponBulldog::GetBaseFireRate()
{
	return BaseFireRate;
}

float AWeaponBulldog::GetPrimaryFireRate()
{
	return fPrimaryFireRate;
}

void AWeaponBulldog::FirePrimary()
{
	Super::FirePrimary();
	
	fPrimaryFireRate += (FireRateAddPerTick);
	fPrimaryFireRate = FMath::Clamp(fPrimaryFireRate, BaseFireRate, FireRateMaximum);

	// Play sound, animation, and particle effect
	WeaponBody->PlayAnimation(PrimaryAnimation, false);
	UGameplayStatics::SpawnEmitterAtLocation(GetWorld(), PrimaryParams.MuzzleParticle, WeaponBody->GetSocketLocation(sBarrelSocketName), WeaponBody->GetSocketRotation(sBarrelSocketName));
	UGameplayStatics::SpawnSoundAtLocation(GetWorld(), PrimaryParams.SoundOnShoot, WeaponBody->GetSocketLocation(sBarrelSocketName));

	bool b = HasUpgrade(EUpgradeType::EBlue) && IsUpgradeRandomProc();
	ANewProjectileBase* bullet = FireBullet(b? ProcBulletParams : PrimaryParams);

	// Handle red upgrade functionality
	if (HasUpgrade(EUpgradeType::ERed))
	{
		if (fCurrentOverheat >= OverheatBonusThreshhold && bullet)
		{
			bullet->AddDamageMultiplier(OverheatBonusDamageMultiplier);
		}
	}
}

void AWeaponBulldog::FireSecondary()
{
	Super::FireSecondary();

	// Play sound, animation, and particle effect
	WeaponBody->PlayAnimation(SecondaryAnimation, false);
	UGameplayStatics::SpawnEmitterAtLocation(GetWorld(), SecondaryParams.MuzzleParticle, WeaponBody->GetSocketLocation(sBarrelSocketName), WeaponBody->GetSocketRotation(sBarrelSocketName));
	UGameplayStatics::SpawnSoundAtLocation(GetWorld(), SecondaryParams.SoundOnShoot, WeaponBody->GetSocketLocation(sBarrelSocketName));
	ReticleBounce(false);

	if (HasUpgrade(EUpgradeType::EGreen))
	{
		for (int i = 0; i < iNumShotsForSecondaryUpgraded; i++)
		{
			FireBullet(GreenUpgradeBulletParams);
		}
	}
	else
	{
		for (int i = 0; i < iNumShotsForSecondary; i++)
		{
			FireBullet(SecondaryParams);
		}
	}
}

void AWeaponBulldog::OnOverheatBegin()
{
	Super::OnOverheatBegin();

	fPrimaryFireRate = BaseFireRate;
}
