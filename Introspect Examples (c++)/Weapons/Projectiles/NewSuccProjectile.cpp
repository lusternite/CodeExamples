// Fill out your copyright notice in the Description page of Project Settings.

#include "Introspect.h"
#include "GameEntities/Weapons/WeaponBase.h"
#include "Succ.h"
#include "NewSuccProjectile.h"

ANewSuccProjectile::ANewSuccProjectile()
	: Super()
{

}

void ANewSuccProjectile::OnHitActor(AActor * actor, const FHitResult & SweepResult)
{
	if (actor->GetClass() != SuccBP)
	{
		if (SuccBP)
		{
			FActorSpawnParameters f;
			f.Owner = this->GetOwner();
			ASucc* succ = GetWorld()->SpawnActor<ASucc>(SuccBP, this->GetActorLocation(), this->GetActorRotation(), f);
			succ->Attach(SweepResult.GetComponent(), SweepResult.BoneName);
			m_bWasHitSuccessful = true;
		}		
		if (m_bWasHitSuccessful)
		{
			Cast<AWeaponBase>(GetOwner())->HitShot(Cast<AWeaponBase>(GetOwner())->HitMarkerFromHitType(EHitType::ERegular));
		}

		OnLifetimeExpired();
	}
}

void ANewSuccProjectile::OnHitComponent(UPrimitiveComponent * component, const FHitResult & SweepResult)
{
	if (SweepResult.GetActor()->GetClass() != SuccBP)
	{
		if (SuccBP)
		{
			FActorSpawnParameters f;
			f.Owner = this->GetOwner();
			ASucc* succ = GetWorld()->SpawnActor<ASucc>(SuccBP, this->GetActorLocation(), this->GetActorRotation(), f);
			succ->Attach(SweepResult.GetComponent(), SweepResult.BoneName);
			m_bWasHitSuccessful = true;
		}
		if (m_bWasHitSuccessful)
		{
			Cast<AWeaponBase>(GetOwner())->HitShot(Cast<AWeaponBase>(GetOwner())->HitMarkerFromHitType(EHitType::ERegular));
		}
		
		OnLifetimeExpired();
	}
}

void ANewSuccProjectile::OnHitTerrain(const FHitResult & SweepResult)
{
	OnLifetimeExpired();
}

void ANewSuccProjectile::Initialize(FVector Position, FVector Velocity, float LifeTime, TArray<TSubclassOf<AActor>> IgnoredClasses, TQueue<ANewProjectileBase*>* Return)
{
	Super::Initialize(Position, Velocity, LifeTime, IgnoredClasses, Return);

	m_bWasHitSuccessful = false;
}