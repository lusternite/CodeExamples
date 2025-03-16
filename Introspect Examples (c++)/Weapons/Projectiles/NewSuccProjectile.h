// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameEntities/Projectiles/NewBullets/NewProjectileBase.h"
#include "NewSuccProjectile.generated.h"

/**
 * 
 */
UCLASS()
class INTROSPECT_API ANewSuccProjectile : public ANewProjectileBase
{
	GENERATED_BODY()
	
	
public:

	// Methods

	ANewSuccProjectile();

	// Variables



protected:

	// Methods

	virtual void OnHitActor(AActor* actor, const FHitResult &SweepResult) override;
	virtual void OnHitComponent(UPrimitiveComponent* component, const FHitResult &SweepResult) override;
	virtual void OnHitTerrain(const FHitResult &SweepResult) override;

	virtual void Initialize(FVector Position, FVector Velocity, float LifeTime, TArray<TSubclassOf<AActor>> IgnoredClasses, TQueue<ANewProjectileBase*>* Return) override;


	// Variables

	UPROPERTY(EditAnywhere, Category = "Attributes")
		TSubclassOf<AActor> SuccBP;

	UPROPERTY(EditAnywhere, Category = "Components")
		bool m_bWasHitSuccessful;

private:

	// Methods



	// Variables

	

};
