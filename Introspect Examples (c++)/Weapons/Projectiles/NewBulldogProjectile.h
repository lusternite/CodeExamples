// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameEntities/Projectiles/NewBullets/NewProjectileBase.h"
#include "NewBulldogProjectile.generated.h"

/**
 * 
 */
UCLASS()
class INTROSPECT_API ANewBulldogProjectile : public ANewProjectileBase
{
	GENERATED_BODY()
	
public:

	// Methods

	ANewBulldogProjectile();
	virtual void Initialize(FVector Position, FVector Velocity, float LifeTime, TArray<TSubclassOf<AActor>> IgnoredClasses, TQueue<ANewProjectileBase*>* Return = nullptr) override;

protected:

	// Methods

	virtual void ResetBullet() override;

	// Variables

	UPROPERTY(EditAnywhere, Category = "Components")
		UParticleSystemComponent* LazerParticle;
	
};
