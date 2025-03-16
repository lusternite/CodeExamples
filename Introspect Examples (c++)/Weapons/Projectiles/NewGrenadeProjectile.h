// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameEntities/Projectiles/NewBullets/NewProjectileBase.h"
#include "GameEntities/Weapons/CanTakeDamage.h"
#include "Components/SphereComponent.h"
#include "NewGrenadeProjectile.generated.h"

/**
 * 
 */
UCLASS()
class INTROSPECT_API ANewGrenadeProjectile : public ANewProjectileBase
{
	GENERATED_BODY()

public:

	// Methods

	ANewGrenadeProjectile();

	virtual void OnHitActor(AActor* actor, const FHitResult &SweepResult) override;
	virtual void OnHitComponent(UPrimitiveComponent* component, const FHitResult &SweepResult) override;
	virtual void OnHitTerrain(const FHitResult &SweepResult) override;

	virtual void OnLifetimeExpired() override;
	virtual void ResetBullet() override;
	virtual void Initialize(FVector Position, FVector Velocity, float LifeTime, TArray<TSubclassOf<AActor>> IgnoredClasses, TQueue<ANewProjectileBase*>* Return) override;

	// Variables

	UPROPERTY(EditAnywhere, Category = "Upgrade Attributes")
		TSubclassOf<AActor> SuccBP;

protected:

	// Methods

	void DamageAllObjectsInRange(const FHitResult &SweepResult);

	// Variables

	UPROPERTY(EditAnywhere, Category = "Components")
		USphereComponent* CollisionRadius;
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Components")
		UParticleSystemComponent* TrailSystem;

	UPROPERTY(EditAnywhere, Category = "Components")
		bool m_bWasHitSuccessful;

private:

	// Methods

	void SpawnSuccOnProc(AActor* AttachActor, UPrimitiveComponent* AttachComponent, FName BoneName);

	// Variables



};
