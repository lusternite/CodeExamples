// Fill out your copyright notice in the Description page of Project Settings.

#include "Introspect.h"
#include "Engine.h"
#include "WeaponHornet.h"

AWeaponHornet::AWeaponHornet()
	: Super()
{
	Queues.Add(new TQueue<ANewProjectileBase*>);
	iCurrentGrenadeDischarge = 0;
}

void AWeaponHornet::Unequip()
{
	Super::Unequip();

	GetWorldTimerManager().ClearTimer(BetweenDischargeHandle);
}

void AWeaponHornet::FirePrimary()
{
	Super::FirePrimary();
	
	// Play sound, animation, and particle effect
	WeaponBody->PlayAnimation(PrimaryAnim, false);
	UGameplayStatics::SpawnEmitterAtLocation(GetWorld(), PrimaryParams.MuzzleParticle, WeaponBody->GetSocketLocation(sBarrelSocketName), WeaponBody->GetSocketRotation(sBarrelSocketName));
	UGameplayStatics::SpawnSoundAtLocation(GetWorld(), PrimaryParams.SoundOnShoot, WeaponBody->GetSocketLocation(sBarrelSocketName));

	FireBullet(PrimaryParams);

	iGrenadesToDischarge += 1;
}

void AWeaponHornet::FireSecondary()
{
	Super::FireSecondary();
	
	// Play sound, animation, and particle effect
	WeaponBody->PlayAnimation(SecondaryAnim, false);
	UGameplayStatics::SpawnEmitterAtLocation(GetWorld(), SecondaryParams.MuzzleParticle, WeaponBody->GetSocketLocation(sBarrelSocketName), WeaponBody->GetSocketRotation(sBarrelSocketName));
	UGameplayStatics::SpawnSoundAtLocation(GetWorld(), SecondaryParams.SoundOnShoot, WeaponBody->GetSocketLocation(sBarrelSocketName));
	ReticleBounce(false);

	FireBullet(SecondaryParams);

	iGrenadesToDischarge += 1;
	FMath::Clamp<int>(iGrenadesToDischarge, 0, iMaxDischargeAmount);
}

void AWeaponHornet::OnOverheatBegin()
{
	Super::OnOverheatBegin();
	
	if (HasUpgrade(EUpgradeType::ERed))
	{
		GetWorldTimerManager().SetTimer(BetweenDischargeHandle, this, &AWeaponHornet::DischargeGrenade, fTimeBetweenDischarges, false);
	}
}

void AWeaponHornet::OnOverheatEnd()
{
	Super::OnOverheatEnd();

	iGrenadesToDischarge = 0;
}

void AWeaponHornet::DischargeGrenade()
{
	if (iCurrentGrenadeDischarge < iGrenadesToDischarge)
	{
		// Fire Bullet
		WeaponBody->PlayAnimation(SecondaryAnim, false);
		UGameplayStatics::SpawnEmitterAtLocation(GetWorld(), PrimaryParams.MuzzleParticle, WeaponBody->GetSocketLocation(sBarrelSocketName), WeaponBody->GetSocketRotation(sBarrelSocketName));
		UGameplayStatics::SpawnSoundAtLocation(GetWorld(), PrimaryParams.SoundOnShoot, WeaponBody->GetSocketLocation(sBarrelSocketName));
		FireBullet(PrimaryParams);

		GetWorldTimerManager().SetTimer(BetweenDischargeHandle, this, &AWeaponHornet::DischargeGrenade, fTimeBetweenDischarges, false);
		iCurrentGrenadeDischarge += 1;
	}
	else
	{
		iGrenadesToDischarge = 0;
		iCurrentGrenadeDischarge = 0;
	}
}