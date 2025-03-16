// Fill out your copyright notice in the Description page of Project Settings.

#include "Introspect.h"
#include "NewGrenadeProjectile.h"
#include "GameEntities/Weapons/WeaponBase.h"
#include "Succ.h"

ANewGrenadeProjectile::ANewGrenadeProjectile()
	: Super()
{
	CollisionRadius = CreateDefaultSubobject<USphereComponent>(TEXT("Collision"));
	CollisionRadius->SetupAttachment(RootComponent);
	CollisionRadius->SetCollisionEnabled(ECollisionEnabled::NoCollision);

	TrailSystem = CreateDefaultSubobject<UParticleSystemComponent>(TEXT("Trail"));
	TrailSystem->SetupAttachment(StaticMeshComponent);
}

void ANewGrenadeProjectile::OnHitActor(AActor * actor, const FHitResult & SweepResult)
{
	CollisionRadius->SetCollisionEnabled(ECollisionEnabled::QueryAndPhysics);
	DamageAllObjectsInRange(SweepResult);
	SpawnSuccOnProc(actor, SweepResult.GetComponent(), SweepResult.BoneName);
	OnLifetimeExpired();
}

void ANewGrenadeProjectile::OnHitComponent(UPrimitiveComponent * component, const FHitResult & SweepResult)
{
	CollisionRadius->SetCollisionEnabled(ECollisionEnabled::QueryAndPhysics);
	DamageAllObjectsInRange(SweepResult);
	SpawnSuccOnProc(SweepResult.GetActor(), component, SweepResult.BoneName);
	OnLifetimeExpired();
}

void ANewGrenadeProjectile::OnHitTerrain(const FHitResult & SweepResult)
{
	CollisionRadius->SetCollisionEnabled(ECollisionEnabled::QueryAndPhysics);
	DamageAllObjectsInRange(SweepResult);
	OnLifetimeExpired();
}

void ANewGrenadeProjectile::OnLifetimeExpired()
{
	Super::OnLifetimeExpired();
}

void ANewGrenadeProjectile::ResetBullet()
{
	Super::ResetBullet();
	TrailSystem->DeactivateSystem();
}

void ANewGrenadeProjectile::DamageAllObjectsInRange(const FHitResult &SweepResult)
{
	EHitType Hit = EHitType::EMiss;
	for (auto& a : GetDamageableObjectsInRange(CollisionRadius))
	{
		EHitType H = a.Key->Execute_ObjectTakeDamage(a.Value, iDamageToDeal, 1.0f, SweepResult, SourceName);

		GEngine->AddOnScreenDebugMessage(-1, 2.0f, FColor::Black, a.Value->GetName());

		if (Hit == EHitType::EMiss)
		{
			Hit = H;
		}
		if (Hit != EHitType::ECritical && (H == EHitType::ECritical || H == EHitType::EArmour))
		{
			Hit = H;
		}

		// Figure out object type for damage marker spawning
		if (Cast<AActor>(a.Value))
		{
			Cast<AWeaponBase>(GetOwner())->SpawnDamageNumberMarker(GetActorLocation(), iBaseDamage, H);
		}
		else if (Cast<UPrimitiveComponent>(a.Value))
		{
			Cast<AWeaponBase>(GetOwner())->SpawnDamageNumberMarker((Cast<UPrimitiveComponent>(a.Value)->GetComponentLocation()), iBaseDamage, H);
		}
		else
		{
			GEngine->AddOnScreenDebugMessage(-1, 3.0f, FColor::Cyan, "Could not cast grenade hit type");
		}

		m_bWasHitSuccessful = true;
	}
	if (m_bWasHitSuccessful)
	{
		Cast<AWeaponBase>(GetOwner())->HitShot(Cast<AWeaponBase>(GetOwner())->HitMarkerFromHitType(Hit));
	}
}

void ANewGrenadeProjectile::SpawnSuccOnProc(AActor* AttachActor, UPrimitiveComponent* AttachComponent, FName BoneName)
{
	if (AttachActor->GetClass() != SuccBP)
	{
		AWeaponBase* HornetOwner = Cast<AWeaponBase>(GetOwner());
		if (HornetOwner->IsUpgradeRandomProc() && HornetOwner->HasUpgrade(EUpgradeType::EBlue))
		{
			//GEngine->AddOnScreenDebugMessage(-1, 3.0f, FColor::Cyan, "ShouldSpawnSucc");

			if (SuccBP)
			{
				FActorSpawnParameters f;
				f.Owner = this->GetOwner();
				ASucc* succ = GetWorld()->SpawnActor<ASucc>(SuccBP, this->GetActorLocation(), this->GetActorRotation(), f);
				succ->Attach(AttachComponent, BoneName);
			}
		}
	}
}

void ANewGrenadeProjectile::Initialize(FVector Position, FVector Velocity, float LifeTime, TArray<TSubclassOf<AActor>> IgnoredClasses, TQueue<ANewProjectileBase*>* Return)
{
	Super::Initialize(Position, Velocity, LifeTime, IgnoredClasses, Return);

	m_bWasHitSuccessful = false;
	TrailSystem->ActivateSystem();
}